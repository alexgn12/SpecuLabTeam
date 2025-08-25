using MediatR;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.Application.Behaviors;
using PrototipoApi.BaseDatos;
using PrototipoApi.Repositories;
using PrototipoApi.Repositories.Interfaces;
using System.Reflection;
using FluentValidation;

// Crea el constructor de la aplicaci�n web
var builder = WebApplication.CreateBuilder(args);

// Agrega servicios a la aplicaci�n

builder.Services.AddControllers(); // Habilita el patr�n Modelo-Vista-Controlador (MVC) para exponer controladores de API

// Configura Swagger/OpenAPI para documentar y probar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura la conexi�n a la base de datos usando Entity Framework Core y SQL Server
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

// Registra MediatR para la inyecci�n de dependencias y manejo de solicitudes (CQRS, Mediator Pattern)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// Registra el repositorio gen�rico para inyecci�n de dependencias
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Registro del servicio externo de edificios
builder.Services.AddHttpClient<PrototipoApi.Services.IExternalBuildingService, PrototipoApi.Services.ExternalBuildingService>();

// Registro del loguer
builder.Services.AddSingleton<PrototipoApi.Logging.ILoguer, PrototipoApi.Logging.Loguer>();

// Construye la aplicaci�n web
var app = builder.Build();

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

// Redirige autom�ticamente las solicitudes HTTP a HTTPS
app.UseHttpsRedirection();

// Habilita la autorizaci�n (pero no la autenticaci�n)
app.UseAuthorization();

// Mapea los controladores a las rutas correspondientes
app.MapControllers();

// Ejecuta la aplicaci�n
app.Run();