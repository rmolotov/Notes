using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Notes.WebApi;

public class SwaggerConfigureOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo
                {
                    Title = $"Notes API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString()
                });
            
            options.AddSecurityDefinition(
                $"Auth token {description.ApiVersion}",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Authorization token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = $"AuthToken {description.ApiVersion}"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            options.CustomOperationIds(apiDescription =>
                apiDescription.TryGetMethodInfo(out var methodInfo)
                    ? methodInfo.Name
                    : null
            );
        }
    }
}