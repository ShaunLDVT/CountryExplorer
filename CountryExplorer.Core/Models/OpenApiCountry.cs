using CountryExplorer.Core.Models;

namespace CountryExplorer.Infrastructure.Repositories
{
	// Models to deserialize the API response
	public class CountryApiResponse
	{
		public CountryName Name { get; set; }
		public string[] Capital { get; set; }
		public int Population { get; set; }
		public string Region { get; set; }
		public string Subregion { get; set; }
		public string Cca2 { get; set; }
		public string Cca3 { get; set; }
		public CountryFlag Flags { get; set; }
		public Dictionary<string, string> Languages { get; set; }
		public Dictionary<string, CurrencyInfo> Currencies { get; set; }
	}
}

	public class CountryName
	{
		public string Common { get; set; }
		public string Official { get; set; }
	}

	public class CountryFlag
	{
		public string Png { get; set; }
		public string Svg { get; set; }
	}

	public class CurrencyInfo
	{
		public string Name { get; set; }
		public string Symbol { get; set; }
	}
