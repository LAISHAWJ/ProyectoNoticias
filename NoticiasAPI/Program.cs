using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar DbContext
builder.Services.AddDbContext<NoticiasDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignorar ciclos de referencia al serializar
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Configurar CORS para permitir acceso desde el SPA
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSPA", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",  // Vite (React) puerto por defecto
                "http://localhost:3000",  // React Create App
                "http://localhost:5284",  // Tu puerto HTTP
                "https://localhost:7231"  // Tu puerto HTTPS
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("X-Total-Count", "X-Page", "X-Per-Page", "X-Total-Pages"); // Exponer headers de paginación
    });
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Noticias API",
        Version = "v1",
        Description = "API REST para gestión de noticias"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Noticias API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();

// IMPORTANTE: CORS debe ir ANTES de Authorization
app.UseCors("AllowSPA");

app.UseAuthorization();

app.MapControllers();

// Mensaje de inicio
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("========================================");
    Console.WriteLine("🚀 Noticias API está corriendo en:");
    Console.WriteLine("   HTTP:  http://localhost:5284");
    Console.WriteLine("   HTTPS: https://localhost:7231");
    Console.WriteLine("   Swagger: https://localhost:7231");
    Console.WriteLine("========================================");
});

app.Run();