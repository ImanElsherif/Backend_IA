namespace WebApplication1.Dtos
{
    public class SendMessageRequestDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; }
    }
}
