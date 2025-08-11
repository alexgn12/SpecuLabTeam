using MediatR;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.BaseDatos;
using PrototipoApi.Repositories;
using PrototipoApi.Repositories.Interfaces;
using System.Reflection;

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

// Registra MediatR para la inyecci�n de dependencias y manejo de solicitudes (CQRS, Mediator Pattern)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// Registra el repositorio gen�rico para inyecci�n de dependencias
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Construye la aplicaci�n web
var app = builder.Build();

// Inicializa la base de datos con datos semilla al iniciar la aplicaci�n
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ContextoBaseDatos>();
    await DbInitializer.SeedAsync(context); // M�todo as�ncrono para poblar la base de datos si es necesario
}

// Configura el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    // Habilita Swagger solo en entorno de desarrollo
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirige autom�ticamente las solicitudes HTTP a HTTPS
app.UseHttpsRedirection();

// Habilita la autorizaci�n (pero no la autenticaci�n)
app.UseAuthorization();

// Mapea los controladores a las rutas correspondientes
app.MapControllers();

// Ejecuta la aplicaci�n
app.Run();