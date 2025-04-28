using CountryExplorer.Application.CQRS.Queries;
using CountryExplorer.Infrastructure.Interfaces;
using MediatR;

namespace CountryExplorer.Application.CQRS.Handlers
{
	public class GetAllCountriesHandler : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountrySummary>>
	{
		private readonly ICountryRepository _countryRepository;

		public GetAllCountriesHandler(ICountryRepository countryRepository)
		{
			_countryRepository = countryRepository;
		}

		public async Task<IEnumerable<CountrySummary>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
		{
			try
			{
				var countries = await _countryRepository.GetAllCountriesAsync();
				return countries.Select(c => new CountrySummary
				{
					Name = c.CommonName,
					Alpha3Code = c.Alpha3Code,
					FlagUrl = c.FlagUrl
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetAllCountriesHandler: {ex.Message}");
				throw;
			}
		}
	}
}