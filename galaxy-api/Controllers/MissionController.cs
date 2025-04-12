using galaxy_api.DTOs;
using galaxy_api.Models;
using galaxy_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace galaxy_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MissionController : ControllerBase
    {
        private readonly IMissionService _missionService;

        public MissionController(IMissionService missionService)
        {
            _missionService = missionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMissions()
        {
            var missions = await _missionService.GetAllMissionsAsync();
            return Ok(missions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMissionById(int id)
        {
            var mission = await _missionService.GetMissionByIdAsync(id);
            if (mission == null)
                return NotFound();

            return Ok(mission);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMission([FromBody] Missions mission)
        {
            mission.Status_Id = 1;
            await _missionService.CreateMissionAsync(mission);
            return CreatedAtAction(nameof(GetMissionById), new { id = mission.Mission_Id }, mission);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMissionDetails(int id, [FromBody] Missions mission)
        {
            await _missionService.UpdateMissionDetailsAsync(id, mission);
            return NoContent();
        }

       [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] MissionStatusUpdateDto dto)
        {
            var existingMission = await _missionService.GetMissionByIdAsync(id);
            if (existingMission == null)
            {
                return NotFound(new { error = "Mission not found." });
            }


            if (dto.Status_Id == 3 && existingMission.Status_Id != 2)
            {
                return BadRequest(new { error = "Mission can only be marked as Completed if it is currently Launched." });
            }

            var mission = new Missions
            {
                Status_Id = dto.Status_Id,
                Feedback = dto.Feedback ?? string.Empty,
                Reward_Credit = dto.Reward_Credit ?? string.Empty
            };

            if (dto.Status_Id == 3) 
            {
                if (string.IsNullOrWhiteSpace(dto.Feedback))
                {
                    return BadRequest(new { error = "Feedback is required when marking a mission as Completed." });
                }

                if (string.IsNullOrWhiteSpace(dto.Reward_Credit))
                {
                    return BadRequest(new { error = "Reward credit is required when marking a mission as Completed." });
                }

                await _missionService.UpdateMissionStatusAsync(id, mission);
                await _missionService.ProvideMissionFeedbackAsync(id, mission);
                await _missionService.RewardCreditMissionAsync(id, mission);
            }
            else if (dto.Status_Id == 4) 
            {
                if (string.IsNullOrWhiteSpace(dto.Feedback))
                {
                    return BadRequest(new { error = "Feedback is required when marking a mission as Aborted." });
                }

                await _missionService.UpdateMissionStatusAsync(id, mission);
                await _missionService.ProvideMissionFeedbackAsync(id, mission);
            }
            else 
            {
                await _missionService.UpdateMissionStatusAsync(id, mission);
            }

            return NoContent();
        }
    }
}
