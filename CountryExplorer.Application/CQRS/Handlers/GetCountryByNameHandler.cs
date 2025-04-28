using CountryExplorer.Application.CQRS.Queries;
using CountryExplorer.Infrastructure.Interfaces;
using CountryExplorer.Core.Models;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;


namespace CountryExplorer.Application.CQRS.Handlers
{
	public class GetCountryByNameHandler : IRequestHandler<GetCountryByNameQuery, Country>
	{
		private readonly ICountryRepository _countryRepository;
		private readonly IDistributedCache _cache;


		//public GetCountryByNameHandler(ICountryRepository countryRepository)
		//{
		//	_countryRepository = countryRepository;
		//}

		public GetCountryByNameHandler(ICountryRepository countryRepository, IDistributedCache cache)
		{
			_countryRepository = countryRepository;
			_cache = cache;
		}

		//public async Task<Country> Handle(GetCountryByNameQuery request, CancellationToken cancellationToken)
		//{
		//	if (string.IsNullOrWhiteSpace(request.Name))
		//	{
		//		throw new ArgumentException("Country name cannot be null or empty.", nameof(request.Name));
		//	}

		//	try
		//	{
		//		return await _countryRepository.GetCountryByNameAsync(request.Name);
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine($"Error in GetCountryByNameHandler: {ex.Message}");
		//		throw;
		//	}
		//}

		public async Task<Country> Handle(GetCountryByNameQuery request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(request.Name))
			{
				throw new ArgumentException("Country name cannot be null or empty.", nameof(request.Name));
			}

			// Check if the country is in the cache
			var cacheKey = $"Country_{request.Name.ToLower()}";
			var cachedCountry = await _cache.GetStringAsync(cacheKey, cancellationToken);

			if (!string.IsNullOrEmpty(cachedCountry))
			{
				// Deserialize and return the cached country
				return JsonSerializer.Deserialize<Country>(cachedCountry);
			}

			// Fetch the country from the repository
			var country = await _countryRepository.GetCountryByNameAsync(request.Name);

			if (country != null)
			{
				// Cache the result for future requests
				var cacheOptions = new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // Cache for 30 minutes
				};

				var serializedCountry = JsonSerializer.Serialize(country);
				await _cache.SetStringAsync(cacheKey, serializedCountry, cacheOptions, cancellationToken);
			}

			return country;
		}
	}
}