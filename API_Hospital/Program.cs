using API_Hospital.Middlewares;
using Hospital.Application;
using Hospital.Infrastructure;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging con Serilog: consola + archivo (configurado en la sección "Serilog" de appsettings.json)
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddDependencys(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Hospital - Gestión de Órdenes",
        Version = "v1",
        Description = "API para crear y consultar órdenes de servicios hospitalarios. Las órdenes se crean en estado Pendiente y un Worker Service las procesa de forma asíncrona."
    });
});

var app = builder.Build();

// Registra cada petición HTTP: método, ruta, código de respuesta y duración
app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
