using CountryExplorer.Infrastructure.Interfaces;
using CountryExplorer.Core.Models;
using CountryExplorer.Core.Mappers;
using System.Net.Http.Json;
using System.Text.Json;
using CountryExplorer.Core.Interfaces;

namespace CountryExplorer.Infrastructure.Repositories
{
	public class CountryRepository : ICountryRepository
	{
		private readonly HttpClient _httpClient;
		private readonly ICountryMapper _countryMapper;
		private const string BaseUrl = $"https://restcountries.com/v3.1";

		public CountryRepository(HttpClient httpClient, ICountryMapper countryMapper)
		{
			_httpClient = httpClient;
			_countryMapper = countryMapper;
			httpClient.BaseAddress = new Uri(BaseUrl);
		}

		public async Task<IEnumerable<Country>> GetAllCountriesAsync()
		{
			var response = await _httpClient.GetFromJsonAsync<List<CountryApiResponse>>("/all");
			return response?.Select(_countryMapper.MapToCountry) ?? new List<Country>();
		}

		public async Task<Country?> GetCountryByNameAsync(string name)
		{
			try
			{
				var response = await _httpClient.GetFromJsonAsync<List<CountryApiResponse>>(BaseUrl + $"/name/{name}");
				return response?.FirstOrDefault() != null
					? _countryMapper.MapToCountry(response.First())
					: null;
			}
			catch (HttpRequestException)
			{
				return null;
			}
		}
	}
}