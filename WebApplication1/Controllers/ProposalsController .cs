using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;
using System.Threading.Tasks;
using System.IO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProposalsController : ControllerBase
    {
        private readonly IDataRepository<Proposal> _proposalRepository;
        private readonly IDataRepository<Job> _jobRepository;


        public ProposalsController(IDataRepository<Job> jobRepository, IDataRepository<Proposal> proposalRepository)
        {
            _jobRepository = jobRepository;
            _proposalRepository = proposalRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProposal([FromForm] ProposalDto proposalDto)
        {
            // Check if a proposal already exists for this job from the same employer
            var existingProposal = await _proposalRepository.GetByCustomCriteria(p =>
                p.JobId == proposalDto.JobId && p.JobSeekerId == proposalDto.JobSeekerId);

            if (existingProposal != null)
            {
                return BadRequest("A proposal from this job seeker for this job already exists.");
            }

            string filePath = null;  // Declare filePath outside the if block

            if (proposalDto.Attachment != null && proposalDto.Attachment.Length > 0)
            {
                // Define the path where you want to save the file
                var folderName = Path.Combine("Resources", "Attachments");
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Create a unique name for the file
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(proposalDto.Attachment.FileName);
                filePath = Path.Combine(directoryPath, fileName);  // Assign file path here

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await proposalDto.Attachment.CopyToAsync(fileStream);
                }

                // Clear the IFormFile object as it's not needed anymore
                proposalDto.Attachment = null;

                // Increment the number of proposals for the corresponding job
                var job = await _jobRepository.GetByIdAsync(proposalDto.JobId);
                if (job != null)
                {
                    job.NumProposals++;
                    await _jobRepository.UpdateAsync(job);
                    await _jobRepository.Save(); // Save changes to the database
                }
            }

            // Create a new Proposal object
            var newProposal = new Proposal
            {
                JobId = proposalDto.JobId,
                JobSeekerId = proposalDto.JobSeekerId,
                EmployerId = proposalDto.EmployerId,
                Attachment = filePath, // Save the file path in the database
                Status = "Pending" // Set the status of the proposal
            };

            // Add the new proposal to the repository and save changes
            await _proposalRepository.AddAsync(newProposal);
            await _proposalRepository.Save();

            return Ok("Proposal created successfully.");
        }





        [HttpGet("{id}")]
        public async Task<IActionResult> GetProposalById(int id)
        {
            var proposal = await _proposalRepository.GetByIdAsync(id);
            if (proposal == null)
            {
                return NotFound($"No proposal found with ID {id}");
            }

            return Ok(proposal);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProposals()
        {
            var proposals = await _proposalRepository.GetAllAsync();
            return Ok(proposals);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateProposalStatus(int id, [FromBody] string status)
        {
            var proposal = await _proposalRepository.GetByIdAsync(id);
            if (proposal == null)
            {
                return NotFound($"No proposal found with ID {id}");
            }

            proposal.Status = status;
            await _proposalRepository.UpdateAsync(proposal);
            await _proposalRepository.Save();

            return Ok(proposal);
        }
        [HttpGet("attachment/{proposalId}")]
        public async Task<IActionResult> GetAttachment(int proposalId)
        {
            var proposal = await _proposalRepository.GetByIdAsync(proposalId);
            if (proposal == null)
            {
                return NotFound($"No proposal found with ID {proposalId}");
            }

            if (string.IsNullOrEmpty(proposal.Attachment))
            {
                return NotFound("Attachment not found for this proposal");
            }

            // Get the file content
            var fileBytes = await System.IO.File.ReadAllBytesAsync(proposal.Attachment);
            var fileName = Path.GetFileName(proposal.Attachment);

            // Return the file
            return File(fileBytes, "application/octet-stream", fileName);
        }



        [HttpGet("employer/{employerId}")]
        public async Task<IActionResult> GetProposalsByEmployerId(int employerId)
        {
            var proposals = await _proposalRepository.GetAllAsync(p => p.EmployerId == employerId);
            return Ok(proposals);
        }

        [HttpGet("notaccepted")]
        public async Task<IActionResult> GetProposalsNotAccepted()
        {
            var proposals = await _proposalRepository.GetAllAsync(p => p.Status != "Accepted");

            // Filter out duplicate job IDs
            var uniqueProposals = proposals.GroupBy(p => p.JobId).Select(group => group.First()).ToList();

            return Ok(uniqueProposals);
        }



        // You can add more methods here for updating, deleting, etc., based on your requirements.
    }


}
