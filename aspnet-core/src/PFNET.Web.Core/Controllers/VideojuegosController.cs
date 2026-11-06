using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Mvc;
using PFNET.Authorization;
using PFNET.Authorization.Users;
using PFNET.MultiTenancy;
using PFNET.Videojuegos;
using Abp.Web.Models;
using Abp.Authorization;
using Abp.Domain.Uow;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PFNET.Controllers
{
    [Route("videojuegos")]
    [DontWrapResult]
    public class VideojuegosController : ControllerBase
    {
        private readonly Abp.Domain.Repositories.IRepository<Videojuego, int> _repository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public VideojuegosController(Abp.Domain.Repositories.IRepository<Videojuego, int> repository, IUnitOfWorkManager unitOfWorkManager)
        {
            _repository = repository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [AbpAllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repository.GetAllListAsync();
            return Ok(list);
        }

        [AbpAllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var v = await _repository.FirstOrDefaultAsync(id);
            if (v == null) return NotFound();
            return Ok(v);
        }

        [AbpAuthorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PFNET.Videojuegos.Dto.CreateVideojuegoDto input)
        {
            var v = new Videojuego
            {
                NombreDelJuego = input.NombreDelJuego,
                ImagenDelJuego = input.ImagenDelJuego
            };
            await _repository.InsertAsync(v);
            await _unitOfWorkManager.Current.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = v.Id }, v);
        }

        [AbpAuthorize]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PFNET.Videojuegos.Dto.UpdateVideojuegoDto input)
        {
            var v = await _repository.FirstOrDefaultAsync(id);
            if (v == null) return NotFound();

            if (input.NombreDelJuego != null) v.NombreDelJuego = input.NombreDelJuego;
            if (input.ImagenDelJuego != null) v.ImagenDelJuego = input.ImagenDelJuego;

            await _repository.UpdateAsync(v);
            return Ok(v);
        }

        [AbpAuthorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await _repository.FirstOrDefaultAsync(id);
            if (v == null) return NotFound();
            await _repository.DeleteAsync(v);
            return Ok();
        }
    }
}



