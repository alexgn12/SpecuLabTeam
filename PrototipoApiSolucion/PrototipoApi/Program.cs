using GammaAI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.Application.Behaviors;
using PrototipoApi.BaseDatos;
using PrototipoApi.Repositories;
using PrototipoApi.Repositories.Interfaces;
using System.Reflection;
using FluentValidation;
using PrototipoApi.Infrastructure.RealTime;
using Serilog;

// Crea el constructor de la aplicaci�n web
var builder = WebApplication.CreateBuilder(args);

// Agrega la carga de secretos de usuario para OpenAIService
builder.Configuration.AddUserSecrets<GammaAI.Services.OpenAIService>();

// Configura Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Agregar servicio de Serilog
builder.Logging.AddSerilog();

// Agrega servicios a la aplicaci�n

builder.Services.AddControllers().AddNewtonsoftJson(); // Habilita el patr�n Modelo-Vista-Controlador (MVC) para exponer controladores de API

// Configura Swagger/OpenAPI para documentar y probar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura la conexi�n a la base de datos usando Entity Framework Core y SQL Server
builder.Services.AddDbContext<ContextoBaseDatos>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Configura CORS para frontend y APIs externas
builder.Services.AddCors(options =>
{
    // Política para frontend (Netlify y localhost)
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(
                "https://speculab.netlify.app",
                "http://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
    // Política abierta para APIs externas (solo si es necesario, úsala con precaución)
    options.AddPolicy("ExternalApi", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registra MediatR para la inyecci�n de dependencias y manejo de solicitudes (CQRS, Mediator Pattern)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// Registra el repositorio gen�rico para inyecci�n de dependencias
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Registro del servicio externo de edificios (ignora validaci�n SSL solo en desarrollo)
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpClient<PrototipoApi.Services.IExternalBuildingService, PrototipoApi.Services.ExternalBuildingService>()
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });
}
else
{
    builder.Services.AddHttpClient<PrototipoApi.Services.IExternalBuildingService, PrototipoApi.Services.ExternalBuildingService>();
}

// Registro del servicio externo de apartamentos (igual, si lo necesitas)
builder.Services.AddHttpClient<PrototipoApi.Services.IExternalApartmentService, PrototipoApi.Services.ExternalApartmentService>();

// Registro de AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddSignalR();

// Registramos nuestro publicador de eventos en tiempo real
builder.Services.AddSingleton<PrototipoApi.Infrastructure.RealTime.IRealTimeNotifier, PrototipoApi.Infrastructure.RealTime.RealTimeNotifier>();

// Registro del servicio OpenAIService en el contenedor DI para IOpenAIService
builder.Services.AddScoped<GammaAI.Core.Interfaces.IOpenAIService, GammaAI.Services.OpenAIService>();

// Construye la aplicaci�n web
var app = builder.Build();

var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
};
var httpClient = new HttpClient(handler);


// Inicializa la base de datos con datos semilla al iniciar la aplicaci�n
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ContextoBaseDatos>();

    if (app.Environment.IsDevelopment())
    {
        context.Database.Migrate();
    }

    await DbInitializer.SeedAsync(context); // M�todo as�ncrono para poblar la base de datos si es necesario
}

// Configura el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    // Habilita Swagger solo en entorno de desarrollo
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Middleware global de excepciones
app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        Log.Warning(ex, "Validation exception: {@Errors}", errors);
        await Results.ValidationProblem(errors).ExecuteAsync(ctx); // 400 con ProblemDetails
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Unhandled exception");
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsJsonAsync(new { error = "Ocurrió un error inesperado." });
    }
});

// Habilita CORS antes de los controladores
// Usa la política de frontend por defecto
app.UseCors("Frontend");

// Si necesitas exponer endpoints públicos para APIs externas, aplica la política "ExternalApi" en los controladores/endpoints correspondientes con [EnableCors("ExternalApi")]
// Redirige autom�ticamente las solicitudes HTTP a HTTPS
app.UseHttpsRedirection();

// Habilita la autorizaci�n (pero no la autenticaci�n)
app.UseAuthorization();

// Mapea los controladores a las rutas correspondientes
app.MapControllers();

// Hub en /hubs/live
app.MapHub<LiveHub>("/hubs/live");

// Ejecuta la aplicaci�n
app.Run();