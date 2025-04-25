// CountryExplorer.API/Program.cs
using CountryExplorer.Application.CQRS.Handlers;
using CountryExplorer.Core.Interfaces;
using CountryExplorer.Core.Mappers;
using CountryExplorer.Infrastructure.Interfaces;
using CountryExplorer.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Country Explorer API", Version = "v1" });
});

// Add MediatR with auto registration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllCountriesHandler).Assembly));

// Register HttpClient for repository
var OpenApiUri = builder.Configuration["Urls:OpenApi"];
if (string.IsNullOrEmpty(OpenApiUri))
{
	throw new InvalidOperationException("The OpenApi connection string is not configured.");
}

builder.Services.AddHttpClient();

// Register repositories
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryMapper, CountryMapper>();

// Add CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngularApp",
		builder => builder
			.WithOrigins("http://localhost:59923")
			.AllowAnyMethod()
			.AllowAnyHeader());
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

app.Run();

// Export for testing
public partial class Program { }