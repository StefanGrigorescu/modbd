using Microsoft.OpenApi.Models;
using MODBD_Common.Text;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MODBD_Api.Common.Web;

public class AppSwaggerConfigurationFactory
{
    public Action<SwaggerGenOptions> SwaggerSetupAction =>
        options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "EShop", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                                Enter 'Bearer' [space] and then your token in the text input below.
                                \r\n\r\nExample: 'Bearer 12345aBcd!ef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            options.ParameterFilter<SwaggerCamelCaseQueryParamsFilter>();
        };
}

public sealed class SwaggerCamelCaseQueryParamsFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (parameter.In == ParameterLocation.Query)
        {
            parameter.Name = StringFormatters.FormatFromPascalToCamelCase(parameter.Name);
        }
    }
}
