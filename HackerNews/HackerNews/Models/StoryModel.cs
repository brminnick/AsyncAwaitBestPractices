using System.Collections.Generic;

using Newtonsoft.Json;

namespace HackerNews
{
    public class StoryModel
    {
        [JsonProperty("by")]
        public string By { get; set; }

        [JsonProperty("descendants")]
        public long Descendants { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("kids")]
        public List<long> Kids { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string PurpleType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
