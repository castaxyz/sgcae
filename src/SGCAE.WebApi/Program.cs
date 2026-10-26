using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SGCAE.Application;
using SGCAE.Infrastructure;
using SGCAE.Infrastructure.Persistence;
using SGCAE.Infrastructure.Persistence.Seed;
using SGCAE.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro de servicios por capas (Clean Architecture)
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 2. Controladores con serialización de Enums como cadenas de texto
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 3. Documentación Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SGCAE API - Sistema de Gestión y Control de Acceso de Empleados",
        Version = "v1",
        Description = "API REST empresarial desarrollada con Clean Architecture, DDD, CQRS, MediatR y EF Core sobre SQL Server.\n\n" +
                      "Módulos:\n" +
                      "- RF-01: Gestión de Empleados\n" +
                      "- RF-02: Gestión de Credenciales RFID\n" +
                      "- RF-03: Registro de Entradas y Salidas\n" +
                      "- RF-04: Consulta del Historial de Accesos y Auditoría\n" +
                      "- RF-06: Administración de Puntos de Acceso"
    });

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// 4. Middleware de captura global de excepciones (RFC 7807)
app.UseGlobalExceptionHandler();

// 5. Configuración de Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGCAE API v1");
    c.RoutePrefix = string.Empty; // Disponible en la raíz
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 6. Aplicar migraciones y sembrado de datos iniciales
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        logger.LogInformation("Aplicando migraciones pendientes en SQL Server...");
        await context.Database.MigrateAsync();

        logger.LogInformation("Ejecutando sembrado de datos iniciales de prueba...");
        var seeder = services.GetRequiredService<ApplicationDbContextSeed>();
        await seeder.SeedSampleDataAsync();

        logger.LogInformation("Inicialización de la base de datos completada.");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "No se pudo conectar a SQL Server al iniciar la aplicación. " +
                              "Las migraciones se ejecutarán cuando el servidor esté disponible.");
    }
}

app.Run();

// Para integración con WebApplicationFactory en pruebas
public partial class Program { }
