using CountryExplorer.Core.Interfaces;
using CountryExplorer.Core.Models;
using CountryExplorer.Infrastructure.Repositories;


namespace CountryExplorer.Core.Mappers
{
	public class CountryMapper: ICountryMapper
	{
		public Country MapToCountry(CountryApiResponse response)
		{
			var country = new Country
			{
				CommonName = response.Name.Common,
				OfficialName = response.Name.Official,
				Alpha2Code = response.Cca2,
				Alpha3Code = response.Cca3,
				Population = response.Population,
				Capital = response.Capital?.FirstOrDefault() ?? "N/A",
				Region = response.Region,
				Subregion = response.Subregion,
				FlagUrl = response.Flags.Png
			};

			if (response.Languages != null)
			{
				foreach (var lang in response.Languages)
				{
					country.Languages.Add(lang.Key, lang.Value);
				}
			}

			if (response.Currencies != null)
			{
				foreach (var curr in response.Currencies)
				{
					country.Currencies.Add(curr.Key, new Currency
					{
						Name = curr.Value.Name,
						Symbol = curr.Value.Symbol
					});
				}
			}

			return country;
		}

		public CountryMapper()
		{
		}
	}
}
