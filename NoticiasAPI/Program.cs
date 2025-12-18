using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Data;

var builder = WebApplication.CreateBuilder(args);

//DbContext
builder.Services.AddDbContext<NoticiasDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignorar ciclos de referencia al serializar
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Configuración de CORS para permitir acceso desde el SPA
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSPA", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
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


app.UseCors("AllowSPA");

app.UseAuthorization();

app.MapControllers();

// Mensaje de inicio
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("========================================");
    Console.WriteLine("Noticias API está corriendo en:");
    Console.WriteLine("   HTTP:  http://localhost:5284");
    Console.WriteLine("   HTTPS: https://localhost:7231");
    Console.WriteLine("   Swagger: https://localhost:7231");
    Console.WriteLine("========================================");
});

app.Run();