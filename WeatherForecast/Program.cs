using DotEnv.Core;
using WeatherForecast.Api;
using WeatherForecast.Clients;
using WeatherForecast.Clients.GoogleWeather;
using WeatherForecast.Clients.OpenWeather;
using WeatherForecast.Controllers;
using WeatherForecast.Factories;
using WeatherForecast.Services;

var builder = WebApplication.CreateBuilder(args);

// Loading configuration as environment variables from the .env file.
new EnvLoader().Load();
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<OpenWeatherDataClient>();
builder.Services.AddHttpClient<GoogleWeatherDataClient>();
builder.Services.AddSingleton<IWeatherProviderFactory, WeatherProviderFactory>();
builder.Services.AddSingleton<ICurrentWeatherController, CurrentWeatherController>();
builder.Services.AddSingleton<IForecastController, ForecastController>();
builder.Services.AddSingleton<IMultipleLocationController, MultipleLocationController>();
builder.Services.AddSingleton<ILocationResolver, LocationResolver>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.MapGroup("/api/v1").MapCurrentWeatherApi();

app.Run();