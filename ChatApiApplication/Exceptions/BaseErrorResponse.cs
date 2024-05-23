using Newtonsoft.Json;

namespace ChatApiApplication.Exceptions
{
    public class BaseErrorResponse
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("traceId")]
        public string? TraceId { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("errors")]
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
