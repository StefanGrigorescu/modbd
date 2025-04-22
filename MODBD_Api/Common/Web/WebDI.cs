using System.Text.Json;
using System.Text.Json.Serialization;

namespace MODBD_Api.Common.Web;

public static class WebDI
{
    /// <summary>
    /// Adds and configures web related services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<GlobalExceptionHandlerMiddleware>();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(new AppSwaggerConfigurationFactory().SwaggerSetupAction);

        services.AddEShopCorsOnLocalHost();    // It won't be used in the request pipeline if the environment is not Development.

        //services.AddSpaStaticFiles(config =>
        //{
        //    config.RootPath = "wwwroot";
        //});

        return services;
    }
}
