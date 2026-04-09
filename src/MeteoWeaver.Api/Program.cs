using MeteoWeaver.Api.Data;
using MeteoWeaver.Api.Endpoints;
using MeteoWeaver.Api.External;
using MeteoWeaver.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<OpenMeteoClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.Add("User-Agent", "MeteoWeaverApi/1.0");
});

builder.Services.AddScoped<WeatherImportService>();
builder.Services.AddScoped<WeatherQueryService>();
builder.Services.AddSingleton<ComfortScoreCalculator>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => Results.Ok(new
{
    name = "MeteoWeaver API",
    version = "1.0.0",
    description = "Imports weather data from Open-Meteo, stores snapshots in SQLite and exposes custom comfort analysis."
}));

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapWeatherEndpoints();

app.Run();

public partial class Program;
