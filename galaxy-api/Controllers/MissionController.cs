using ErrorOr;
using galaxy_api.Delegates;
using galaxy_api.DTOs;
using galaxy_api.DTOs.Missions;
using galaxy_api.Helpers;
using galaxy_api.Models;
using galaxy_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace galaxy_api.Controllers
{

    public class MissionController : ApiController
    {
        private readonly IMissionService _missionService;
        private readonly FeedbackValidator _feedbackValidator;


        public MissionController(IMissionService missionService)
        {
            _missionService = missionService;
            _feedbackValidator = FeedbackValidation.IsValidFeedback;

        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Missions>>>> GetAllMissions()
        {
            var missions = await _missionService.GetAllMissionsAsync();
            return Ok(new ApiResponse<IEnumerable<Missions>>
            {
                Success = true,
                Message = $"Retrieved {missions.Count()} missions.",
                Data = missions
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<Missions>>> GetMissionById(int id)
        {
            var mission = await _missionService.GetMissionByIdAsync(id);
            if (mission == null)
            {
                return NotFound(new ApiResponse<Missions>
                {
                    Success = false,
                    Message = $"No mission found with ID {id}",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Missions>
            {
                Success = true,
                Message = "Mission retrieved successfully",
                Data = mission
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Missions>>> CreateMission([FromBody] Missions mission)
        {
            mission.Status_Id = 1;
            await _missionService.CreateMissionAsync(mission);
            return CreatedAtAction(nameof(GetMissionById), new { id = mission.Mission_Id }, new ApiResponse<Missions>
            {
                Success = true,
                Message = "Mission created successfully",
                Data = mission
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateMissionDetails(int id, [FromBody] Missions mission)
        {
            await _missionService.UpdateMissionDetailsAsync(id, mission);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Mission detail/s updated successfully",
                Data = null
            });
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateStatus(int id, [FromBody] MissionStatusUpdateDto dto)
        {
            var existingMission = await _missionService.GetMissionByIdAsync(id);
            if (existingMission == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Mission not found.",
                    Data = null
                });
            }

            if (dto.Status_Id == 3 && existingMission.Status_Id != 2)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Mission can only be marked as Completed if it is currently Launched.",
                    Data = null
                });
            }

            var mission = new Missions
            {
                Status_Id = dto.Status_Id,
                Feedback = dto.Feedback ?? string.Empty,
                Reward_Credit = dto.Reward_Credit ?? string.Empty
            };

            if (dto.Status_Id == 3)
            {
                if (dto.Feedback is not { } feedback || !_feedbackValidator(dto.Feedback))
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Feedback must be at least 10 characters.",
                        Data = null
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Reward_Credit))
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Reward credit is required when marking a mission as Completed.",
                        Data = null
                    });
                }

                await _missionService.UpdateMissionStatusAsync(id, mission);
                await _missionService.ProvideMissionFeedbackAsync(id, mission);
                await _missionService.RewardCreditMissionAsync(id, mission);
            }
            else if (dto.Status_Id == 4)
            {
                if (dto.Feedback is not { } feedback || !_feedbackValidator(dto.Feedback))
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Feedback must be at least 10 characters.",
                        Data = null
                    });
                }

                await _missionService.UpdateMissionStatusAsync(id, mission);
                await _missionService.ProvideMissionFeedbackAsync(id, mission);
            }
            else
            {
                await _missionService.UpdateMissionStatusAsync(id, mission);
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Mission status updated successfully",
                Data = null
            });
        }

        [HttpGet("report")]
        public async Task<ActionResult<ApiResponse<object>>> GetMissionStatusReport(
            [FromQuery] string? missionType,
            [FromQuery] string? status,
            [FromQuery] string? groupBy)
        {
            var report = await _missionService.GetMissionStatusReportAsync(missionType, status, groupBy);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Mission status report generated successfully",
                Data = report
            });
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetMissionDetailsWithCrewHistory(int id)
        {
            ErrorOr<MissionDetailsWithCrewHistoryDTO> result = await _missionService.GetMissionDetailsWithCrewHistoryAsync(id);

            return result.Match(
                missionData => Ok(missionData),
                errors => Problem(errors[0].Description)
            );

        }

        [HttpPost("{id}/assign-crew")]
        public async Task<IActionResult> AssignCrewToMission(int id, [FromBody] AssignCrewRequestDTO request)
        {
            if (id <= 0) return BadRequest("A valid Mission ID must be provided in the route.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            ErrorOr<Success> result = await _missionService.AssignCrewToMissionAsync(id, request.CrewId);


            return result.Match(
                success => NoContent(),
                Problem
            );
        }
    }
}
