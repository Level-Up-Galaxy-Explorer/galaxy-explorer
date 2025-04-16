using Xunit;
using Moq;
using FluentAssertions;
using galaxy_api.Models;
using galaxy_api.Repositories;
using galaxy_api.Services;
using galaxy_api.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace galaxy_api.Tests
{
    public class MissionServiceTests
    {
        private readonly Mock<IMissionRepository> _mockRepo;
        private readonly MissionService _missionService;

        public MissionServiceTests()
        {
            _mockRepo = new Mock<IMissionRepository>();
            _missionService = new MissionService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllMissionsAsync_ReturnsOrderedMissions()
        {
            // Arrange
            var mockMissions = new List<Missions>
            {
                new Missions { Mission_Id = 2, Name = "Mars Rover" },
                new Missions { Mission_Id = 1, Name = "Moon Landing" }
            };

            _mockRepo.Setup(repo => repo.GetAllMissionsAsync()).ReturnsAsync(mockMissions);

            // Act
            var result = await _missionService.GetAllMissionsAsync();

            // Assert
            result.Should().BeInAscendingOrder(m => m.Mission_Id);
            _mockRepo.Verify(repo => repo.GetAllMissionsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetMissionByIdAsync_Exists_ReturnsMission()
        {
            // Arrange
            var mockMission = new Missions { Mission_Id = 1, Name = "Moon Landing" };
            _mockRepo.Setup(repo => repo.GetMissionByIdAsync(1)).ReturnsAsync(mockMission);

            // Act
            var result = await _missionService.GetMissionByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Moon Landing");
        }

        [Fact]
        public async Task GetMissionByIdAsync_NotFound_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetMissionByIdAsync(999)).ReturnsAsync((Missions?)null);

            // Act
            var result = await _missionService.GetMissionByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateMissionAsync_CallsRepository()
        {
            // Arrange
            var newMission = new Missions { Name = "New Mission" };
            _mockRepo.Setup(repo => repo.CreateMissionAsync(It.IsAny<Missions>())).Returns(Task.CompletedTask);

            // Act
            await _missionService.CreateMissionAsync(newMission);

            // Assert
            _mockRepo.Verify(repo => repo.CreateMissionAsync(newMission), Times.Once);
        }

        [Fact]
        public async Task UpdateMissionDetailsAsync_CallsRepository()
        {
            // Arrange
            var updatedMission = new Missions { Mission_Id = 1, Name = "Updated Mission" };
            _mockRepo.Setup(repo => repo.UpdateMissionDetailsAsync(1, It.IsAny<Missions>())).Returns(Task.CompletedTask);

            // Act
            await _missionService.UpdateMissionDetailsAsync(1, updatedMission);

            // Assert
            _mockRepo.Verify(repo => repo.UpdateMissionDetailsAsync(1, updatedMission), Times.Once);
        }

        [Fact]
        public async Task ProvideMissionFeedbackAsync_CallsRepository()
        {
            // Arrange
            var missionWithFeedback = new Missions { Mission_Id = 1, Feedback = "Great job!" };
            _mockRepo.Setup(repo => repo.ProvideMissionFeedbackAsync(1, It.IsAny<Missions>())).Returns(Task.CompletedTask);

            // Act
            await _missionService.ProvideMissionFeedbackAsync(1, missionWithFeedback);

            // Assert
            _mockRepo.Verify(repo => repo.ProvideMissionFeedbackAsync(1, missionWithFeedback), Times.Once);
        }

        [Fact]
        public async Task UpdateMissionStatusAsync_CallsRepository()
        {
            // Arrange
            var missionWithNewStatus = new Missions { Mission_Id = 1, Status_Id = 2 }; // Using Status_Id instead of Status
            _mockRepo.Setup(repo => repo.UpdateMissionStatusAsync(1, It.IsAny<Missions>())).Returns(Task.CompletedTask);

            // Act
            await _missionService.UpdateMissionStatusAsync(1, missionWithNewStatus);

            // Assert
            _mockRepo.Verify(repo => repo.UpdateMissionStatusAsync(1, missionWithNewStatus), Times.Once);
        }

        [Fact]
        public async Task RewardCreditMissionAsync_CallsRepository()
        {
            // Arrange
            var missionWithReward = new Missions { Mission_Id = 1, Reward_Credit = "100" }; // Using Reward_Credit instead of CreditsAwarded
            _mockRepo.Setup(repo => repo.RewardCreditMissionAsync(1, It.IsAny<Missions>())).Returns(Task.CompletedTask);

            // Act
            await _missionService.RewardCreditMissionAsync(1, missionWithReward);

            // Assert
            _mockRepo.Verify(repo => repo.RewardCreditMissionAsync(1, missionWithReward), Times.Once);
        }
    }
}