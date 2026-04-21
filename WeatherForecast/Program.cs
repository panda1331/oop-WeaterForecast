using DotEnv.Core;
using WeatherForecast.Api;
using WeatherForecast.Clients;
using WeatherForecast.Clients.GoogleWeather;
using WeatherForecast.Clients.OpenWeather;
using WeatherForecast.Controllers;
using WeatherForecast.Factories;

var builder = WebApplication.CreateBuilder(args);

// Loading configuration as environment variables from the .env file.
new EnvLoader().Load();
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "WeatherExampleAPI";
    config.Title = "Weather Example API";
    config.Version = "v1";
});
builder.Services.AddHttpClient<OpenWeatherDataClient>();
builder.Services.AddHttpClient<GoogleWeatherDataClient>();
builder.Services.AddSingleton<IWeatherProviderFactory, WeatherProviderFactory>();
builder.Services.AddSingleton<ICurrentWeatherController, CurrentWeatherController>();
builder.Services.AddSingleton<IForecastController, ForecastController>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
    app.UseDeveloperExceptionPage();
}

app.MapGroup("/api/v1").MapCurrentWeatherApi();

app.Run();