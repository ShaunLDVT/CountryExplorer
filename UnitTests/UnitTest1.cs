using CountryExplorer.Application.CQRS.Handlers;
using CountryExplorer.Application.CQRS.Queries;
using CountryExplorer.Core.Models;
using CountryExplorer.Infrastructure.Interfaces;
using Moq;

namespace UnitTests
{
		public class GetAllCountriesHandlerTests
		{
			[Fact]
			public async Task Handle_ShouldReturnAllCountriesSummaries()
			{
				// Arrange
				var mockRepo = new Mock<ICountryRepository>();
				mockRepo.Setup(repo => repo.GetAllCountriesAsync())
					.ReturnsAsync(new List<Country>
					{
					new Country
					{
						CommonName = "United States",
						OfficialName = "United States of America",
						Alpha3Code = "USA",
						FlagUrl = "https://flags.com/usa.png",
						Population = 328000000,
						Capital = "Washington D.C."
					},
					new Country
					{
						CommonName = "Canada",
						OfficialName = "Canada",
						Alpha3Code = "CAN",
						FlagUrl = "https://flags.com/canada.png",
						Population = 38000000,
						Capital = "Ottawa"
					}
					});

				var handler = new GetAllCountriesHandler(mockRepo.Object);
				var query = new GetAllCountriesQuery();

				// Act
				var result = await handler.Handle(query, CancellationToken.None);

				// Assert
				Assert.NotNull(result);
				Assert.Equal(2, result.Count());

				var usa = result.FirstOrDefault(c => c.Alpha3Code == "USA");
				Assert.NotNull(usa);
				Assert.Equal("United States", usa.Name);
				Assert.Equal("https://flags.com/usa.png", usa.FlagUrl);

				var canada = result.FirstOrDefault(c => c.Alpha3Code == "CAN");
				Assert.NotNull(canada);
				Assert.Equal("Canada", canada.Name);
				Assert.Equal("https://flags.com/canada.png", canada.FlagUrl);
			}
		}
	}
