using CountryExplorer.Application.CQRS.Queries;
using CountryExplorer.Infrastructure.Interfaces;
using CountryExplorer.Core.Models;
using MediatR;

namespace CountryExplorer.Application.CQRS.Handlers
{
	public class GetCountryByNameHandler : IRequestHandler<GetCountryByNameQuery, Country>
	{
		private readonly ICountryRepository _countryRepository;

		public GetCountryByNameHandler(ICountryRepository countryRepository)
		{
			_countryRepository = countryRepository;
		}

		public async Task<Country> Handle(GetCountryByNameQuery request, CancellationToken cancellationToken)
		{
			return await _countryRepository.GetCountryByNameAsync(request.Name);
		}
	}
}