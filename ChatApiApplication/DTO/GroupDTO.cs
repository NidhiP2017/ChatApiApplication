using ChatApiApplication.Model;

namespace ChatApiApplication.DTO
{
    public class GroupDTO
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public ICollection<Messages> Messages { get; set; }
    }
}
