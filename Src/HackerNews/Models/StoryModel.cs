using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace HackerNews
{
    public class StoryModel
    {
        public DateTimeOffset CreatedAt_DateTimeOffset => UnixTimeStampToDateTimeOffset(CreatedAt_UnixTime);

        [JsonProperty("by")]
        public string Author { get; set; }

        [JsonProperty("descendants")]
        public long Descendants { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("kids")]
        public List<long> Kids { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("time")]
        public long CreatedAt_UnixTime { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string PurpleType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        DateTimeOffset UnixTimeStampToDateTimeOffset(long unixTimeStamp)
        {
            var dateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, default);
            return dateTimeOffset.AddSeconds(unixTimeStamp);
        }
    }
}
