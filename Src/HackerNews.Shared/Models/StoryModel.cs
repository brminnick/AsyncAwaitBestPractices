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

        public string Description => ToString();

        public long Id { get; }
        public string Author { get; }
        public long Score { get; }
        public long CreatedAt_UnixTime { get; }
        public DateTimeOffset CreatedAt_DateTimeOffset { get; }
        public string Title { get; }
        public string Url { get; }

        public override string ToString() => $"{Score} Points by {Author}, {GetAgeOfStory(CreatedAt_DateTimeOffset)} ago";

        static string GetAgeOfStory(in DateTimeOffset storyCreatedAt)
        {
            var timespanSinceStoryCreated = DateTimeOffset.UtcNow - storyCreatedAt;

            return timespanSinceStoryCreated switch
            {
                TimeSpan storyAge when storyAge < TimeSpan.FromHours(1) => $"{Math.Ceiling(timespanSinceStoryCreated.TotalMinutes)} minutes",

                TimeSpan storyAge when storyAge >= TimeSpan.FromHours(1) && storyAge < TimeSpan.FromHours(2) => $"{Math.Floor(timespanSinceStoryCreated.TotalHours)} hour",

                TimeSpan storyAge when storyAge >= TimeSpan.FromHours(2) && storyAge < TimeSpan.FromHours(24) => $"{Math.Floor(timespanSinceStoryCreated.TotalHours)} hours",

                TimeSpan storyAge when storyAge >= TimeSpan.FromHours(24) && storyAge < TimeSpan.FromHours(48) => $"{Math.Floor(timespanSinceStoryCreated.TotalDays)} day",

                TimeSpan storyAge when storyAge >= TimeSpan.FromHours(48) => $"{Math.Floor(timespanSinceStoryCreated.TotalDays)} days",

                _ => string.Empty,
            };
        }
    }
}
