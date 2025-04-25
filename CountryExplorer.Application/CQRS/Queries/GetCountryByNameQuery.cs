using CountryExplorer.Core.Models;
using MediatR;

namespace CountryExplorer.Application.CQRS.Queries
{
	public class GetCountryByNameQuery : IRequest<Country>
	{
		public string Name { get; set; }

		public GetCountryByNameQuery(string name)
		{
			Name = name;
		}
	}
}