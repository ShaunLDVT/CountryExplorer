using CountryExplorer.Application.CQRS.Handlers;
using CountryExplorer.Application.CQRS.Queries;
using CountryExplorer.Core.Interfaces;
using CountryExplorer.Core.Mappers;
using CountryExplorer.Infrastructure.Interfaces;
using CountryExplorer.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Moq;


namespace CountryExplorer.UnitTests.IntegrationTests.Handlers
{
	public class GetCountryByNameHandlerIntegrationTests: IDisposable
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ServiceCollection _services;

		public GetCountryByNameHandlerIntegrationTests()
		{
			_services = new ServiceCollection();

			_services.AddMemoryCache();
			_services.AddDistributedMemoryCache(); 
			_services.AddHttpClient();
			_services.AddScoped<ICountryMapper, CountryMapper>();
			_services.AddScoped<ICountryRepository, CountryRepository>();
			_services.AddScoped<GetCountryByNameHandler>();

			_serviceProvider = _services.BuildServiceProvider();
		}

		[Fact]
		public async Task Handle_WithRealDependencies_ShouldRetrieveCountry()
		{
			// Arrange
			var handler = _serviceProvider.GetRequiredService<GetCountryByNameHandler>();
			var query = new GetCountryByNameQuery("Canada");

			// Act
			var result = await handler.Handle(query, default);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Canada", result.CommonName);
			Assert.NotEmpty(result.Alpha3Code);
		}

		[Fact]
		public async Task Handle_WithNonExistentCountry_ShouldReturnNull()
		{
			// Test external services
			if (!IsNetworkAvailable())
				return;

			// Arrange
			var handler = _serviceProvider.GetRequiredService<GetCountryByNameHandler>();
			var query = new GetCountryByNameQuery("NonExistentCountry123456789");

			// Act
			var result = await handler.Handle(query, default);

			// Assert
			Assert.Null(result);
		}

		private bool IsNetworkAvailable()
		{
			try
			{
				using (var client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromSeconds(5);
					var response = client.GetAsync("https://restcountries.com").Result;
					return response.IsSuccessStatusCode;
				}
			}
			catch
			{
				return false;
			}
		}

		public void Dispose()
		{
			if (_serviceProvider is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}
}
