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
		private readonly IDistributedCache? _cache;

		public GetCountryByNameHandler(ICountryRepository countryRepository, IDistributedCache? cache = null)
		{
			_countryRepository = countryRepository;
			_cache = cache;
		}

		public async Task<Country> Handle(GetCountryByNameQuery request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(request.Name))
			{
				throw new ArgumentException("Country name cannot be null or empty.", nameof(request.Name));
			}

			var cacheKey = $"Country_{request.Name.ToLower()}";

			// Attempt to retrieve from cache with a fallback
			Country? country = null;
			if (_cache != null)
			{
				try
				{
					var cachedCountry = await _cache.GetStringAsync(cacheKey, cancellationToken);
					if (!string.IsNullOrEmpty(cachedCountry))
					{
						// Deserialize and return the cached country
						return JsonSerializer.Deserialize<Country>(cachedCountry);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Redis unavailable: {ex.Message}");
				}
			}

			// Fetch the country from the repository
			country = await _countryRepository.GetCountryByNameAsync(request.Name);

			if (country != null && _cache != null)
			{
				try
				{
					var cacheOptions = new DistributedCacheEntryOptions
					{
						AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // Cache for 30 minutes
					};

					var serializedCountry = JsonSerializer.Serialize(country);
					await _cache.SetStringAsync(cacheKey, serializedCountry, cacheOptions, cancellationToken);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to cache data in Redis: {ex.Message}");
				}
			}

			return country;
		}
	}
}
