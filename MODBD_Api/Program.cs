using MODBD_Api.Common.DI;
using MODBD_Api.Common.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddModbdServices(builder.Configuration, builder.Environment);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseCors(EShopCors.AllowEShopAppOriginOnLocalHost);
}

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();  // Logging needs to be enriched at this point to log exceptions

app.MapControllers();

app.Run();
