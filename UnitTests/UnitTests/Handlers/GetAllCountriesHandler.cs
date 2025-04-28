using CountryExplorer.Application.CQRS.Handlers;
using CountryExplorer.Application.CQRS.Queries;
using CountryExplorer.Core.Models;
using CountryExplorer.Infrastructure.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Handlers
{
	public class GetAllCountriesHandlerTests
	{
		private readonly Mock<ICountryRepository> _mockRepository;
		private readonly GetAllCountriesHandler _handler;
		private readonly CancellationToken _cancellationToken = CancellationToken.None;

		public GetAllCountriesHandlerTests()
		{
			_mockRepository = new Mock<ICountryRepository>();

			_handler = new GetAllCountriesHandler(_mockRepository.Object);
		}

		[Fact]
		public async Task Handle_ShouldReturnMappedCountrySummaries()
		{
			// Arrange
			var countries = new List<Country>
			{
				new Country
				{
					CommonName = "United States",
					OfficialName = "United States of America",
					Alpha3Code = "USA",
					FlagUrl = "https://example.com/us.png",
					Population = 331000000
				},
				new Country
				{
					CommonName = "Canada",
					OfficialName = "Canada",
					Alpha3Code = "CAN",
					FlagUrl = "https://example.com/canada.png",
					Population = 38000000
				}
			};

			_mockRepository.Setup(r => r.GetAllCountriesAsync())
				.ReturnsAsync(countries);

			var query = new GetAllCountriesQuery();

			// Act
			var result = await _handler.Handle(query, _cancellationToken);

			// Assert
			Assert.NotNull(result);
			var summaries = result.ToList();
			Assert.Equal(2, summaries.Count);

			// Verify first country
			Assert.Equal("United States", summaries[0].Name);
			Assert.Equal("USA", summaries[0].Alpha3Code);
			Assert.Equal("https://example.com/us.png", summaries[0].FlagUrl);

			// Verify second country
			Assert.Equal("Canada", summaries[1].Name);
			Assert.Equal("CAN", summaries[1].Alpha3Code);
			Assert.Equal("https://example.com/canada.png", summaries[1].FlagUrl);

			// Verify repository was called once
			_mockRepository.Verify(r => r.GetAllCountriesAsync(), Times.Once);
		}

		[Fact]
		public async Task Handle_WithEmptyRepository_ShouldReturnEmptyCollection()
		{
			// Arrange
			_mockRepository.Setup(r => r.GetAllCountriesAsync())
				.ReturnsAsync(new List<Country>());

			var query = new GetAllCountriesQuery();

			// Act
			var result = await _handler.Handle(query, _cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result);

			// Verify repository was called once
			_mockRepository.Verify(r => r.GetAllCountriesAsync(), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
		{
			// Arrange
			var expectedException = new Exception("Repository error");

			_mockRepository.Setup(r => r.GetAllCountriesAsync())
				.ThrowsAsync(expectedException);

			var query = new GetAllCountriesQuery();

			// Act & Assert
			var exception = await Assert.ThrowsAsync<Exception>(() =>
				_handler.Handle(query, _cancellationToken));

			Assert.Equal(expectedException, exception);

			// Verify repository was called once
			_mockRepository.Verify(r => r.GetAllCountriesAsync(), Times.Once);
		}

		[Fact]
		public async Task Handle_ShouldMapOnlyRequiredProperties()
		{
			// Arrange
			var countries = new List<Country>
			{
				new Country
				{
					CommonName = "Japan",
					OfficialName = "Japan",
					Alpha3Code = "JPN",
					Alpha2Code = "JP",
					FlagUrl = "https://example.com/japan.png",
					Population = 126000000,
					Capital = "Tokyo",
					Region = "Asia",
					Subregion = "Eastern Asia",
					Languages = new Dictionary<string, string> { { "jpn", "Japanese" } },
					Currencies = new Dictionary<string, Currency> { { "JPY", new Currency { Name = "Japanese yen", Symbol = "¥" } } }
				}
			};

			_mockRepository.Setup(r => r.GetAllCountriesAsync())
				.ReturnsAsync(countries);

			var query = new GetAllCountriesQuery();

			// Act
			var result = await _handler.Handle(query, _cancellationToken);

			// Assert
			var summary = result.Single();
			Assert.Equal("Japan", summary.Name);
			Assert.Equal("JPN", summary.Alpha3Code);
			Assert.Equal("https://example.com/japan.png", summary.FlagUrl);

			// Verify only these three properties are mapped
			var propertyCount = typeof(CountrySummary).GetProperties().Length;
			Assert.Equal(3, propertyCount);
		}
	}
}

