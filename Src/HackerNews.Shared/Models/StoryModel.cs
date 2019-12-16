using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace HackerNews.Shared
{
    public class StoryModel
    {
        public StoryModel(long id, string by, long score, long time, string title, string url) =>
            (Id, Author, Score, CreatedAt_UnixTime, Title, Url) = (id, by, score, time, title, url);

        public DateTimeOffset CreatedAt_DateTimeOffset => UnixTimeStampToDateTimeOffset(CreatedAt_UnixTime);

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("by")]
        public string Author { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("time")]
        public long CreatedAt_UnixTime { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        DateTimeOffset UnixTimeStampToDateTimeOffset(long unixTimeStamp)
        {
            var dateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, default);
            return dateTimeOffset.AddSeconds(unixTimeStamp);
        }
    }
}
