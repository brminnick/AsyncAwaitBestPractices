using System;

namespace HackerNews.Shared
{
    public class StoryModel
    {
        public StoryModel(long id, string by, long score, long time, string title, string url)
        {
            Id = id;
            Author = by;
            Score = score;
            CreatedAt_UnixTime = time;
            CreatedAt_DateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(CreatedAt_UnixTime);
            Title = title;
            Url = url;
        }

        public long Id { get; }
        public string Author { get; }
        public long Score { get; }
        public long CreatedAt_UnixTime { get; }
        public DateTimeOffset CreatedAt_DateTimeOffset { get; }
        public string Title { get; }
        public string Url { get; }
    }
}
