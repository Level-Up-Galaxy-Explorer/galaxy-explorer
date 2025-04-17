using Xunit;
using Moq;
using FluentAssertions;
using galaxy_api.DTOs;
using galaxy_api.Models;
using galaxy_api.Repositories;
using galaxy_api.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlanetServiceTests
{
    private readonly Mock<IPlanetRepository> _mockRepo;
    private readonly PlanetService _planetService;

    public PlanetServiceTests()
    {
        _mockRepo = new Mock<IPlanetRepository>();
        _planetService = new PlanetService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllPlanetsAsync_ReturnsPlanetDTOs()
    {
        // Arrange
        var mockPlanets = new List<Planet>
        {
            new Planet { Id = 1, Name = "Earth", Galaxy = "Milky Way", PlanetType = "Terrestrial", HasLife = true, Coordinates = "X1Y2Z3" },
            new Planet { Id = 2, Name = "Mars", Galaxy = "Milky Way", PlanetType = "Terrestrial", HasLife = false, Coordinates = "X4Y5Z6" }
        };

        _mockRepo.Setup(repo => repo.GetAllPlanetsAsync()).ReturnsAsync(mockPlanets);

        // Act
        var result = await _planetService.GetAllPlanetsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Earth");
        _mockRepo.Verify(repo => repo.GetAllPlanetsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPlanetAsync_NotFound_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetPlanetByIdAsync(It.IsAny<int>())).ReturnsAsync((Planet?)null);

        // Act
        var result = await _planetService.GetPlanetAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddPlanetAsync_ValidDto_ReturnsMappedDTO()
    {
        // Arrange
        var planetDto = new PlanetDTO { Name = "Venus", Galaxy = "Milky Way", PlanetType = "Terrestrial", HasLife = false, Coordinates = "X7Y8Z9" };
        var createdPlanet = new Planet { Id = 3, Name = "Venus", Galaxy = "Milky Way", PlanetType = "Terrestrial", HasLife = false, Coordinates = "X7Y8Z9" };

        _mockRepo.Setup(repo => repo.AddPlanetAsync(It.IsAny<Planet>())).ReturnsAsync(createdPlanet);

        // Act
        var result = await _planetService.AddPlanetAsync(planetDto);

        // Assert
        result.Id.Should().Be(3);
        _mockRepo.Verify(repo => repo.AddPlanetAsync(It.Is<Planet>(p => p.Name == "Venus")), Times.Once);
    }

    [Fact]
    public async Task UpdatePlanetAsync_Fails_ReturnsFalse()
    {
        // Arrange
        var planetDto = new PlanetDTO { Name = "Updated Earth" };
        _mockRepo.Setup(repo => repo.UpdatePlanetAsync(It.IsAny<int>(), It.IsAny<Planet>())).ReturnsAsync(false);

        // Act
        var result = await _planetService.UpdatePlanetAsync(1, planetDto);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SearchPlanetsAsync_FiltersByName()
    {
        // Arrange
        var mockPlanets = new List<Planet>
        {
            new Planet { Id = 1, Name = "Earth" }
        };

        _mockRepo.Setup(repo => repo.SearchPlanetsAsync("earth")).ReturnsAsync(mockPlanets);

        // Act
        var result = await _planetService.SearchPlanetsAsync("earth");

        // Assert
        result.Should().ContainSingle(p => p.Name == "Earth");
    }

    [Fact]
    public async Task GetPlanetTypesAsync_ReturnsList()
    {
        // Arrange
        var mockTypes = new List<string> { "Terrestrial", "Gas Giant" };
        _mockRepo.Setup(repo => repo.GetPlanetTypesAsync()).ReturnsAsync(mockTypes);

        // Act
        var result = await _planetService.GetPlanetTypesAsync();

        // Assert
        result.Should().Equal("Terrestrial", "Gas Giant");
    }
}