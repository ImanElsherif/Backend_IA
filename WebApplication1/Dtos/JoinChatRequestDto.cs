namespace WebApplication1.Dtos
{
    public class JoinChatRequestDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int ProposalId { get; set; }
    }
}
