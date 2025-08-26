using MediatR;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.Application.Behaviors;
using PrototipoApi.BaseDatos;
using PrototipoApi.Repositories;
using PrototipoApi.Repositories.Interfaces;
using System.Reflection;
using FluentValidation;

// Crea el constructor de la aplicación web
var builder = WebApplication.CreateBuilder(args);

// Agrega servicios a la aplicación

builder.Services.AddControllers(); // Habilita el patrón Modelo-Vista-Controlador (MVC) para exponer controladores de API

// Configura Swagger/OpenAPI para documentar y probar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura la conexión a la base de datos usando Entity Framework Core y SQL Server
builder.Services.AddDbContext<ContextoBaseDatos>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Configura CORS para permitir cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registra MediatR para la inyección de dependencias y manejo de solicitudes (CQRS, Mediator Pattern)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// Registra el repositorio genérico para inyección de dependencias
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Registro del servicio externo de edificios
builder.Services.AddHttpClient<PrototipoApi.Services.IExternalBuildingService, PrototipoApi.Services.ExternalBuildingService>();

// Registro del loguer
builder.Services.AddSingleton<PrototipoApi.Logging.ILoguer, PrototipoApi.Logging.Loguer>();

// Registro de AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Construye la aplicación web
var app = builder.Build();

// Inicializa la base de datos con datos semilla al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ContextoBaseDatos>();

    if (app.Environment.IsDevelopment())
    {
        context.Database.Migrate();
    }

    await DbInitializer.SeedAsync(context); // Método asíncrono para poblar la base de datos si es necesario
    
}

// Configura el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    // Habilita Swagger solo en entorno de desarrollo
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        await Results.ValidationProblem(errors).ExecuteAsync(ctx); // 400 con ProblemDetails
    }
});

// Habilita CORS antes de los controladores
app.UseCors("AllowAll");

// Redirige automáticamente las solicitudes HTTP a HTTPS
app.UseHttpsRedirection();

// Habilita la autorización (pero no la autenticación)
app.UseAuthorization();

// Mapea los controladores a las rutas correspondientes
app.MapControllers();

// Ejecuta la aplicación
app.Run();