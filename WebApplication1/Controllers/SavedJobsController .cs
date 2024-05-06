using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SavedJobsController : ControllerBase
    {
        private readonly IDataRepository<SavedJob> _savedJobRepository;

        public SavedJobsController(IDataRepository<SavedJob> savedJobRepository)
        {
            _savedJobRepository = savedJobRepository;
        }

        [HttpPost]
        public async Task<IActionResult> SaveJob(SavedJobDto savedJobDto)
        {
            try
            {
                var existingSavedJob = await _savedJobRepository.GetByCustomCriteria(
                    job => job.JobSeekerId == savedJobDto.UserId && job.JobId == savedJobDto.JobId);

                if (existingSavedJob != null)
                {
                    return BadRequest("You have already saved this job.");
                }

                var savedJob = new SavedJob
                {
                    JobSeekerId = savedJobDto.UserId,
                    JobId = savedJobDto.JobId,
                    DateSaved = DateTime.Now
                };

                await _savedJobRepository.AddAsync(savedJob);
                await _savedJobRepository.Save();

                return Ok("Job saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to save job: {ex.Message}");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetSavedJobs(int userId)
        {
            try
            {
                var savedJobs = await _savedJobRepository.GetAllAsync(job => job.JobSeekerId == userId);
                return Ok(savedJobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve saved jobs: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSavedJob(int id)
        {
            try
            {
                var savedJob = await _savedJobRepository.GetByIdAsync(id);
                if (savedJob == null)
                {
                    return NotFound("Saved job not found.");
                }

                await _savedJobRepository.DeleteAsync(savedJob);
                await _savedJobRepository.Save();

                return Ok("Saved job deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to delete the saved job: {ex.Message}");
            }
        }
    }
}
