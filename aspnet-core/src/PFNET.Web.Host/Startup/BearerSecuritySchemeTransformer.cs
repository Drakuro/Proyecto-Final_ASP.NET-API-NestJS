using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PFNET.Web.Host.Startup
{
    // Declares in the OpenAPI document that the API uses JWT Bearer authentication.
    internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            if (document.Components == null)
            {
                document.Components = new OpenApiComponents();
            }

            if (document.Components.SecuritySchemes == null)
            {
                document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>();
            }

            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste the access_token returned by /auth/login"
            };

            document.SecurityRequirements.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }] = Array.Empty<string>()
            });

            // Servers come with a trailing slash ("http://localhost:3000/"), which makes
            // Scalar build URLs like "http://localhost:3000//auth/register".
            if (document.Servers != null)
            {
                foreach (var server in document.Servers)
                {
                    server.Url = server.Url.TrimEnd('/');
                }
            }

            return Task.CompletedTask;
        }
    }
}
