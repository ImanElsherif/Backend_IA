namespace WebApplication1.Dtos
{
    public class ChatDto
    {
        public int Id { get; set; }
        public int ProposalId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSentMessage { get; set; } // Add this property

        // Other properties and methods
    }
}
