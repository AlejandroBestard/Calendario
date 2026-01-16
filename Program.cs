using Calendario.Components;
using Calendario.Data;
using Calendario.Servicios;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE BASE DE DATOS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("No se encontró 'DefaultConnection'.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- 2. SERVICIOS DE INTERFAZ ---
builder.Services.AddMudServices();

// --- 3. SERVICIOS LÓGICOS ---
// ¡¡ESTA ES LA LÍNEA QUE FALTABA!! 
// Sin esto, ClimaService no puede conectarse a internet y la app explota.
builder.Services.AddHttpClient(); 

builder.Services.AddScoped<CalendarEngine>();
builder.Services.AddScoped<ClimaService>();

// --- 4. API Y SWAGGER ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Calendario API", 
        Version = "v1",
        Description = "API de gestión de calendarios"
    });
});

// --- 5. BLAZOR ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// --- 6. MIDDLEWARE ---

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Calendario API v1");
        c.RoutePrefix = "swagger"; 
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();