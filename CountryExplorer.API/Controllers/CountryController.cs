// CountryExplorer.API/Controllers/CountriesController.cs
using CountryExplorer.Application.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CountryExplorer.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CountriesController : ControllerBase
	{
		private readonly IMediator _mediator;

		public CountriesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<CountrySummary>>> GetAllCountries()
		{
			var query = new GetAllCountriesQuery();
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpGet("{name}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Core.Models.Country>> GetCountryByName(string name)
		{
			var query = new GetCountryByNameQuery(name);
			var result = await _mediator.Send(query);

			if (result == null)
				return NotFound();

			return Ok(result);
		}
	}
}