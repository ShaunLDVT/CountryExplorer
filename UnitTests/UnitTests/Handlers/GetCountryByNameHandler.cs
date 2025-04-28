using CountryExplorer.Application.CQRS.Handlers;
using CountryExplorer.Application.CQRS.Queries;
using CountryExplorer.Core.Models;
using CountryExplorer.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;
using System.Text.Json;

namespace CountryExplorer.UnitTests.UnitTests.Handlers
{
	public class GetCountryByNameHandlerTests
	{
		private readonly Mock<ICountryRepository> _mockRepository;
		private readonly Mock<IDistributedCache> _mockCache;
		private readonly GetCountryByNameHandler _handler;
		private readonly CancellationToken _cancellationToken = CancellationToken.None;

		public GetCountryByNameHandlerTests()
		{
			_mockRepository = new Mock<ICountryRepository>();
			_mockCache = new Mock<IDistributedCache>();
			_handler = new GetCountryByNameHandler(_mockRepository.Object, _mockCache.Object);
		}

		[Fact]
		public async Task Handle_WithEmptyName_ShouldThrowArgumentException()
		{
			// Arrange
			var query = new GetCountryByNameQuery(string.Empty);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentException>(() =>
				_handler.Handle(query, _cancellationToken));
		}

		[Fact]
		public async Task Handle_WithCachedCountry_ShouldReturnCachedResult()
		{
			// Arrange
			var countryName = "Canada";
			var query = new GetCountryByNameQuery(countryName);

			var country = new Country
			{
				CommonName = "Canada",
				OfficialName = "Canada",
				Alpha3Code = "CAN",
				Population = 38000000,
				Capital = "Ottawa"
			};

			var serializedCountry = JsonSerializer.Serialize(country);
			var cacheKey = $"Country_{countryName.ToLower()}";

			_mockCache.Setup(c => c.GetAsync(cacheKey, _cancellationToken)).ReturnsAsync(Encoding.UTF8.GetBytes(serializedCountry));

			// Act
			var result = await _handler.Handle(query, _cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Canada", result.CommonName);
			Assert.Equal("CAN", result.Alpha3Code);

			// Verify the repository was not called
			_mockRepository.Verify(r => r.GetCountryByNameAsync(It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public async Task Handle_WithoutCachedCountry_ShouldCallRepositoryAndCacheResult()
		{
			// Arrange
			var countryName = "Canada";
			var query = new GetCountryByNameQuery(countryName);

			var country = new Country
			{
				CommonName = "Canada",
				OfficialName = "Canada",
				Alpha3Code = "CAN",
				Population = 38000000,
				Capital = "Ottawa"
			};

			var cacheKey = $"Country_{countryName.ToLower()}";

			_mockCache.Setup(c => c.GetAsync(cacheKey, _cancellationToken)).ReturnsAsync((byte[])null);

			_mockRepository.Setup(r => r.GetCountryByNameAsync(countryName)).ReturnsAsync(country);

			// Act
			var result = await _handler.Handle(query, _cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Canada", result.CommonName);

			// Verify the repository was called
			_mockRepository.Verify(r => r.GetCountryByNameAsync(countryName), Times.Once);

			// Verify the result was cached using SetAsync
			_mockCache.Verify(c => c.SetAsync(
				cacheKey,
				It.IsAny<byte[]>(),
				It.IsAny<DistributedCacheEntryOptions>(),
				_cancellationToken),
				Times.Once);
		}

		[Fact]
		public async Task Handle_WhenRepositoryReturnsNull_ShouldNotCache()
		{
			// Arrange
			var countryName = "NonExistingCountry";
			var query = new GetCountryByNameQuery(countryName);

			var cacheKey = $"Country_{countryName.ToLower()}";

			_mockCache.Setup(c => c.GetAsync(cacheKey, _cancellationToken)).ReturnsAsync((byte[])null);

			_mockRepository.Setup(r => r.GetCountryByNameAsync(countryName)).ReturnsAsync((Country)null);

			// Act
			var result = await _handler.Handle(query, _cancellationToken);

			// Assert
			Assert.Null(result);

			// Verify repository was called
			_mockRepository.Verify(r => r.GetCountryByNameAsync(countryName), Times.Once);

			// Verify result was not cached
			_mockCache.Verify(c => c.SetAsync(
				It.IsAny<string>(),
				It.IsAny<byte[]>(),
				It.IsAny<DistributedCacheEntryOptions>(),
				It.IsAny<CancellationToken>()),
				Times.Never);
		}
	}
}
