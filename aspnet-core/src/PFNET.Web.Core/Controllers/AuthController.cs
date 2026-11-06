using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.UI;
using Abp.Runtime.Session;
using Abp.Web.Models;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PFNET.Authorization;
using PFNET.Authorization.Users;
using PFNET.MultiTenancy;
using PFNET.Authentication.JwtBearer;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PFNET.Controllers
{
    [Route("auth")]
    [DontWrapResult]
    public class AuthController : ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IAbpSession _abpSession;

        public AuthController(
            UserManager userManager,
            LogInManager logInManager,
            ITenantCache tenantCache,
            TokenAuthConfiguration configuration,
            IAbpSession abpSession)
        {
            _userManager = userManager;
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _configuration = configuration;
            _abpSession = abpSession;
        }

        [HttpPost("register")]
        [AbpAllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Validacion basica (paridad con NestJS class-validator)
            if (string.IsNullOrWhiteSpace(model.Email) || model.Password == null || model.Password.Length < 8)
            {
                return BadRequest(new { message = "Validation failed" });
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(model.Email);
                if (addr.Address != model.Email) return BadRequest(new { message = "Validation failed" });
            }
            catch
            {
                return BadRequest(new { message = "Validation failed" });
            }

            var user = new User
            {
                TenantId = _abpSession.TenantId,
                UserName = model.Email,
                Name = model.Email,
                Surname = model.Email,
                EmailAddress = model.Email,
                IsEmailConfirmed = true
            };

            user.Password = _userManager.PasswordHasher.HashPassword(user, model.Password);
            user.SetNormalizedNames();

            try
            {
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    if (createResult.Errors.Any(e => e.Code.Contains("Duplicate", StringComparison.OrdinalIgnoreCase)))
                    {
                        return Conflict(new { message = "Email already registered" });
                    }
                    return BadRequest(createResult.Errors);
                }
            }
            catch (Abp.UI.UserFriendlyException)
            {
                return Conflict(new { message = "Email already registered" });
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Email already registered" });
            }

            return CreatedAtAction(nameof(Register), new { id = user.Id }, new { id = user.Id, email = user.EmailAddress });
        }

        [HttpPost("login")]
        [AbpAllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var tenancyName = GetTenancyNameOrNull();
            var loginResult = await _logInManager.LoginAsync(model.Email, model.Password, tenancyName);

            if (loginResult.Result != AbpLoginResultType.Success)
            {
                return Unauthorized();
            }

            var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
            return Ok(new { access_token = accessToken });
        }

        private string GetTenancyNameOrNull()
        {
            if (!_abpSession.TenantId.HasValue)
            {
                return null;
            }
            return _tenantCache.GetOrNull(_abpSession.TenantId.Value)?.TenancyName;
        }

        private string CreateAccessToken(System.Collections.Generic.IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static System.Collections.Generic.List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }
    }

    public class RegisterModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}











