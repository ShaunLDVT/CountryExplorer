// CountryExplorer.API/Program.cs
using CountryExplorer.Application.CQRS.Handlers;
using CountryExplorer.Core.Interfaces;
using CountryExplorer.Core.Mappers;
using CountryExplorer.Infrastructure.Interfaces;
using CountryExplorer.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Country API",
		Version = "v1.0.0",
		Description = "API for retrieving country information"
	});
});

// Add MediatR with auto registration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllCountriesHandler).Assembly));

builder.Services.AddHttpClient();

// Register repositories
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryMapper, CountryMapper>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngularApp",
		builder => builder
			.WithOrigins("http://localhost:59923")
			.AllowAnyMethod()
			.AllowAnyHeader());
});

// Ensure the following line is present in the code
builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "localhost:6379";
	options.InstanceName = "CountryExplorer_";
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();

app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async context =>
	{
		context.Response.StatusCode = 500;
		context.Response.ContentType = "application/json";

		var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
		if (exception != null)
		{
			var errorResponse = new { Message = exception.Message };
			await context.Response.WriteAsJsonAsync(errorResponse);
		}
	});
});


app.Run();

// Export for testing
public partial class Program { }