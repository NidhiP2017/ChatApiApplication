namespace ChatApiApplication.Hubs
{
    public interface IClientChat
    {
        Task ReceiveMessage(string message);

        Task<string> GetUserId();
        Task SendMessage(string user, string receiverConnectionId, string message);
    }
}
