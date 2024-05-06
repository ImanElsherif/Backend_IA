using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IDataRepository<Proposal> _proposalRepository;

        public JobsController(IDataRepository<Job> jobRepository, IDataRepository<Proposal> proposalRepository)
        {
            _jobRepository = jobRepository;
            _proposalRepository = proposalRepository;
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
        [Authorize(Roles = "employer")]
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

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateJobStatus(int id, [FromBody] string status)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound($"No job found with ID {id}");
            }

            // Check if the job is already in the desired state
            if (job.Status == status)
            {
                return BadRequest("The job is already in the specified status.");
            }

            // Check if the job is in a state that can be transitioned to the given status


            // Validate the input status


            job.Status = status;  // Update the status

            _jobRepository.UpdateAsync(job);  // Assuming UpdateAsync handles the update operation
            await _jobRepository.Save();

            return NoContent();  // HTTP 204 - No content to send in response, signifies successful update
        }

        [HttpPatch("{id}/increment-proposals")]
        public async Task<IActionResult> IncrementProposals(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound($"No job found with ID {id}");
            }

            job.NumProposals++;  // Increment the number of proposals

            _jobRepository.UpdateAsync(job);  // Save the updated job
            await _jobRepository.Save();

            return NoContent();  // Return HTTP 204 No Content to indicate success without sending data back
        }

        [HttpGet("accepted-jobs-with-no-accepted-proposals")]
        public async Task<IActionResult> GetAcceptedJobsWithNoAcceptedProposals()
        {
            try
            {
                // Retrieve all jobs with "Accepted" status
                var acceptedJobs = await _jobRepository.GetAllAsync(job => job.Status == "Accepted");
                if (acceptedJobs == null)
                {
                    return StatusCode(500, "Error fetching accepted jobs.");
                }

                // Get job IDs of accepted jobs
                var acceptedJobIds = acceptedJobs.Select(job => job.JobId).ToList();

                // Retrieve proposals associated with accepted jobs
                var acceptedProposals = await _proposalRepository.GetAllAsync(proposal => acceptedJobIds.Contains(proposal.JobId) && proposal.Status == "Accepted");
                if (acceptedProposals == null)
                {
                    return StatusCode(500, "Error fetching accepted proposals.");
                }

                // Get job IDs of accepted proposals
                var acceptedProposalJobIds = acceptedProposals.Select(proposal => proposal.JobId).ToList();

                // Retrieve jobs with "Accepted" status but no accepted proposals
                var jobsWithNoAcceptedProposals = acceptedJobs.Where(job => !acceptedProposalJobIds.Contains(job.JobId));

                return Ok(jobsWithNoAcceptedProposals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching jobs with no accepted proposals: {ex.Message}");
            }
        }





    }
}
