namespace ChatApiApplication.DTO
{
    public class ThreadMessageDTO
    {
        public Guid ParentMessageId { get; set; }
        public string Content { get; set; }
    }
}
