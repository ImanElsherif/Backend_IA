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

        public ProposalsController(IDataRepository<Proposal> proposalRepository)
        {
            _proposalRepository = proposalRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProposal([FromForm] ProposalDto proposalDto)
        {
            // Check if a proposal already exists for this job from the same employer
            var existingProposal = await _proposalRepository.GetByCustomCriteria(p =>
                p.JobId == proposalDto.JobId && p.EmployerId == proposalDto.EmployerId);

            if (existingProposal != null)
            {
                return BadRequest("A proposal from this employer for this job already exists.");
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
            }

            var proposal = new Proposal
            {
                JobId = proposalDto.JobId,
                JobSeekerId = proposalDto.JobSeekerId,
                EmployerId = proposalDto.EmployerId,
                Attachment = filePath,  // filePath is now accessible here
                Status = proposalDto.Status
            };

            await _proposalRepository.AddAsync(proposal);
            await _proposalRepository.Save();

            return CreatedAtAction(nameof(GetProposalById), new { id = proposal.ProposalId }, proposal);
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
