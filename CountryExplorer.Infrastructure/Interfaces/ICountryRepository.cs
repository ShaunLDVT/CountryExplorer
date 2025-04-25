using CountryExplorer.Core.Models;

namespace CountryExplorer.Infrastructure.Interfaces
{
	public interface ICountryRepository
	{
		Task<IEnumerable<Country>> GetAllCountriesAsync();
		Task<Country> GetCountryByNameAsync(string name);
	}
}