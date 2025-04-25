using CountryExplorer.Core.Models;
using CountryExplorer.Infrastructure.Repositories;

namespace CountryExplorer.Core.Interfaces
{
	public interface ICountryMapper
	{
		Country MapToCountry(CountryApiResponse response);
	}
}
