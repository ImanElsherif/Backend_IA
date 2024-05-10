namespace WebApplication1.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int ProposalId { get; set; }
        public int SenderId { get; set; } // ID of the user sending the message
        public int ReceiverId { get; set; } // ID of the user receiving the message
        public string Message { get; set; } // The message content
        public DateTime SentAt { get; set; } // Timestamp of when the message was sent
                                             // Add any other properties needed for the chat entity

    }
}
