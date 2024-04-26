using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IDataRepository<Job> _jobRepository;

        public JobsController(IDataRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobRepository.GetAllAsync();
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] JobDto jobDto)
        {
            var job = new Job
            {
                EmployerId = jobDto.EmployerId,
                JobTitle = jobDto.JobTitle,
                JobDescription = jobDto.JobDescription,
                JobType = jobDto.JobType,
                JobBudget = jobDto.JobBudget,
                PostCreationDate = jobDto.PostCreationDate,
                NumProposals = jobDto.NumProposals,
                Location = jobDto.Location,
                Status = jobDto.Status
            };

            await _jobRepository.AddAsync(job);
            await _jobRepository.Save();
            return CreatedAtAction(nameof(GetJobById), new { id = job.JobId }, job);
        }

        // Additional methods (Update, Delete) can be added here
    }
}
