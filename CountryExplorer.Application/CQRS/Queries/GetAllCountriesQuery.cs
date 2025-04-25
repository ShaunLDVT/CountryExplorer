using CountryExplorer.Core.Models;
using MediatR;

namespace CountryExplorer.Application.CQRS.Queries
{
	public class GetAllCountriesQuery : IRequest<IEnumerable<CountrySummary>>
	{
	}

	public class CountrySummary
	{
		public string Name { get; set; }
		public string Alpha3Code { get; set; }
		public string FlagUrl { get; set; }
	}
}