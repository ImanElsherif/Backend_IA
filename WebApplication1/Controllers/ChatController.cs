using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dtos;
using WebApplication1.Models;
using System;
using System.Threading.Tasks;
using WebApplication1.Data;
using System.Linq.Expressions;
using System.Linq;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IDataRepository<Chat> _chatRepository;
        private readonly IDataRepository<Job> _jobRepository;
        private readonly IDataRepository<Proposal> _proposalRepository;
        public ChatController(IDataRepository<Chat> chatRepository, IDataRepository<Job> jobRepository, IDataRepository<Proposal> proposalRepository)
        {
            _chatRepository = chatRepository;
            _jobRepository = jobRepository;
            _proposalRepository = proposalRepository;
        }
        [HttpPost("{proposalId}/join")]
        public async Task<IActionResult> JoinChat(int proposalId, [FromBody] JoinChatRequestDto request)
        {
            if (proposalId != request.ProposalId)
            {
                return BadRequest("Invalid request");
            }

            // Check if the user has already joined the chat
            var existingMessages = await _chatRepository.GetAllAsync(c => c.ProposalId == proposalId && c.SenderId == request.SenderId);

            // If the user has already joined the chat, return without sending any message
            if (existingMessages.Any())
            {
                return Ok(); // or any other appropriate response
            }

            return Ok(); // Return early without creating a chat or saving anything
        }





        [HttpGet("{proposalId}/messages")]
        public async Task<IActionResult> GetMessages(int proposalId)
        {
            try
            {
                var messages = await _chatRepository.GetAllAsync(c => c.ProposalId == proposalId);
                var messageDtos = messages.Select(m => new ChatDto
                {
                    Id = m.Id,
                    ProposalId = m.ProposalId,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Message = m.Message,
                    SentAt = m.SentAt,
                    IsSentMessage = false // Set IsSentMessage to false for received messages
                }).ToList();
                return Ok(messageDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{proposalId}/message")]
        public async Task<IActionResult> SendMessage(int proposalId, [FromBody] SendMessageRequestDto request)
        {
            try
            {
                var chat = new Chat
                {
                    ProposalId = proposalId,
                    SenderId = request.SenderId,
                    ReceiverId = request.ReceiverId,
                    Message = request.Message,
                    SentAt = DateTime.UtcNow
                };

                await _chatRepository.AddAsync(chat);
                await _chatRepository.Save();

                var chatDto = new ChatDto
                {
                    Id = chat.Id,
                    ProposalId = chat.ProposalId,
                    SenderId = chat.SenderId,
                    ReceiverId = chat.ReceiverId,
                    Message = chat.Message,
                    SentAt = chat.SentAt,
                    IsSentMessage = true // Set IsSentMessage to true for sent messages
                };

                return Ok(chatDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
