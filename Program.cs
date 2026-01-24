using Calendario.Components;
using Calendario.Servicios;
using MudBlazor.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Servicios sin base de datos - usa almacenamiento en archivo JSON
builder.Services.AddSingleton<RepositorioCalendario>();
builder.Services.AddScoped<MotorCalendario>();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ClimaService>();
builder.Services.AddScoped<FestivosService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Calendario API",
        Version = "v1",
        Description = "API de gestión de calendarios"
    });
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();


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