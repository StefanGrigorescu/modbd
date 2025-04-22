namespace MODBD_Api.Common.Web;

public static class EShopCors
{
    public const string AllowEShopAppOriginOnLocalHost = "AllowEShopAppOriginOnLocalHost";

    public static IServiceCollection AddEShopCorsOnLocalHost(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy(
                AllowEShopAppOriginOnLocalHost,
                builder => builder
                    //.AllowAnyOrigin(
                    .WithOrigins(
                        // APP Origins
                        "https://localhost:4200",
                        "http://localhost:4200",
                        // API Origins
                        "https://localhost:44370",
                        "http://localhost:44370"
                    ).AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            );
        });
}
