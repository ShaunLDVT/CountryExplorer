namespace CountryExplorer.Core.Models
{
	public class Country
	{
		public string CommonName { get; set; }
		public string OfficialName { get; set; }
		public string Alpha2Code { get; set; }
		public string Alpha3Code { get; set; }
		public int Population { get; set; }
		public string Capital { get; set; }
		public string Region { get; set; }
		public string Subregion { get; set; }
		public string FlagUrl { get; set; }
		public Dictionary<string, string> Languages { get; set; } = new Dictionary<string, string>();
		public Dictionary<string, Currency> Currencies { get; set; } = new Dictionary<string, Currency>();
	}

	public class Currency
	{
		public string Name { get; set; }
		public string Symbol { get; set; }
	}
}