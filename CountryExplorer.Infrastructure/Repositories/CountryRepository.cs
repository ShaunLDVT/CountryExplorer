using CountryExplorer.Infrastructure.Interfaces;
using CountryExplorer.Core.Models;
using CountryExplorer.Core.Mappers;
using System.Net.Http.Json;
using System.Text.Json;
using CountryExplorer.Core.Interfaces;
using Microsoft.Extensions.Configuration;


namespace CountryExplorer.Infrastructure.Repositories
{
	public class CountryRepository : ICountryRepository
	{
		private readonly HttpClient _httpClient;
		private readonly ICountryMapper _countryMapper;
		private readonly string _baseUrl;

		public CountryRepository(HttpClient httpClient, ICountryMapper countryMapper, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_countryMapper = countryMapper;
			_baseUrl = configuration["Urls:CountryApiBaseUrl"];
			httpClient.BaseAddress = new Uri(_baseUrl);
		}

		public async Task<IEnumerable<Country>> GetAllCountriesAsync()
		{
			try
			{
				var response = await _httpClient.GetFromJsonAsync<List<CountryApiResponse>>(_baseUrl + "/all");
				if (response == null)
				{
					throw new Exception("API returned null response.");
				}
				return response.Select(_countryMapper.MapToCountry);
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"HTTP Request failed: {ex.Message}");
				throw new Exception("Failed to fetch countries. Please try again later.", ex);
			}
		}

		public async Task<Country> GetCountryByNameAsync(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Country name cannot be null or empty.", nameof(name));
			}

			try
			{
				var response = await _httpClient.GetFromJsonAsync<List<CountryApiResponse>>(_baseUrl + $"/name/{name}");

				return _countryMapper.MapToCountry(response.First());
			}
			catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				// Return null when country not found (404)
				Console.WriteLine($"Country not found: {name}");
				return null;
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"HTTP Request failed: {ex.Message}");
				throw new Exception("Failed to fetch country details. Please try again later.", ex);
			}
		}
	}
}