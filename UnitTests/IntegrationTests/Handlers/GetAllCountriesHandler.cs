using CountryExplorer.Application.CQRS.Handlers;
using CountryExplorer.Application.CQRS.Queries;
using CountryExplorer.Core.Interfaces;
using CountryExplorer.Core.Mappers;
using CountryExplorer.Infrastructure.Interfaces;
using CountryExplorer.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CountryExplorer.UnitTests.IntegrationTests.Handlers
{
	public class GetAllCountriesHandlerIntegrationTests : IDisposable
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ServiceCollection _services;

		public GetAllCountriesHandlerIntegrationTests()
		{
			_services = new ServiceCollection();

			_services.AddMemoryCache();
			_services.AddHttpClient();
			_services.AddScoped<ICountryMapper, CountryMapper>();
			_services.AddScoped<ICountryRepository, CountryRepository>();
			_services.AddScoped<GetAllCountriesHandler>();

			_serviceProvider = _services.BuildServiceProvider();
		}

		[Fact]
		public async Task Handle_WithRealDependencies_ShouldRetrieveCountries()
		{
			if (!IsNetworkAvailable())
				return;

			// Arrange
			var handler = _serviceProvider.GetRequiredService<GetAllCountriesHandler>();
			var query = new GetAllCountriesQuery();

			// Act
			var result = await handler.Handle(query, default);

			// Assert
			Assert.NotNull(result);
			Assert.NotEmpty(result);

			foreach (var country in result)
			{
				Assert.NotNull(country.Name);
				Assert.NotNull(country.Alpha3Code);
				Assert.Equal(3, country.Alpha3Code.Length);  // Alpha3 codes are always 3 characters
			}

			// Check for some well-known countries
			var countryNames = result.Select(c => c.Name).ToList();
			Assert.Contains(countryNames, name =>
				name.Contains("United States") ||
				name.Contains("Canada") ||
				name.Contains("Australia") ||
				name.Contains("Japan"));
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

