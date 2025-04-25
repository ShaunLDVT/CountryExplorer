using CountryExplorer.Application.CQRS.Queries;
using CountryExplorer.Core.Models;
using MediatR;
using Moq;
using CountryExplorer.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
		public class CountriesControllerTests
		{
			[Fact]
			public async Task GetAllCountries_ShouldReturnOkWithCountries()
			{
				// Arrange
				var mockMediator = new Mock<IMediator>();
				var countries = new List<CountrySummary>
			{
				new CountrySummary { Name = "United States", Alpha3Code = "USA", FlagUrl = "https://flags.com/usa.png" },
				new CountrySummary { Name = "Canada", Alpha3Code = "CAN", FlagUrl = "https://flags.com/canada.png" }
			};

				mockMediator.Setup(m => m.Send(It.IsAny<GetAllCountriesQuery>(), It.IsAny<CancellationToken>()))
					.ReturnsAsync(countries);

				var controller = new CountriesController(mockMediator.Object);

				// Act
				var result = await controller.GetAllCountries();

				// Assert
				var okResult = Assert.IsType<OkObjectResult>(result.Result);
				var returnedCountries = Assert.IsAssignableFrom<IEnumerable<CountrySummary>>(okResult.Value);
				Assert.Equal(2, returnedCountries.Count());
			}

			[Fact]
			public async Task GetCountryByName_WithValidName_ShouldReturnOkWithCountry()
			{
				// Arrange
				var mockMediator = new Mock<IMediator>();
				var country = new Country
				{
					CommonName = "Canada",
					OfficialName = "Canada",
					Alpha3Code = "CAN",
					Population = 38000000,
					Capital = "Ottawa"
				};

				mockMediator.Setup(m => m.Send(It.IsAny<GetCountryByNameQuery>(), It.IsAny<CancellationToken>()))
					.ReturnsAsync(country);

				var controller = new CountriesController(mockMediator.Object);

				// Act
				var result = await controller.GetCountryByName("Canada");

				// Assert
				var okResult = Assert.IsType<OkObjectResult>(result.Result);
				var returnedCountry = Assert.IsType<Country>(okResult.Value);
				Assert.Equal("Canada", returnedCountry.CommonName);
				Assert.Equal("CAN", returnedCountry.Alpha3Code);
			}

			[Fact]
			public async Task GetCountryByName_WithInvalidName_ShouldReturnNotFound()
			{
				// Arrange
				var mockMediator = new Mock<IMediator>();

				mockMediator.Setup(m => m.Send(It.IsAny<GetCountryByNameQuery>(), It.IsAny<CancellationToken>()))
					.ReturnsAsync((Country)null);

				var controller = new CountriesController(mockMediator.Object);

				// Act
				var result = await controller.GetCountryByName("NonExistingCountry");

				// Assert
				Assert.IsType<NotFoundResult>(result.Result);
			}
		}
	}
