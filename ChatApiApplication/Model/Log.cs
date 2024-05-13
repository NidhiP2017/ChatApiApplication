namespace ChatApiApplication.Model
{
    public class Log
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public string RequestBody { get; set; }
        public DateTime TimeOfCall { get; set; }
        public String UserName { get; set; }
    }

}
