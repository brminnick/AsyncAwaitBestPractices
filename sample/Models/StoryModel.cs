using System.Text.Json.Serialization;

namespace HackerNews;

record StoryModel(
		[property: JsonPropertyName("by")] string Author,
		[property: JsonPropertyName("id")] long Id,
		[property: JsonPropertyName("score")] int Score,
		[property: JsonPropertyName("time")] long CreatedAt_UnixTime,
		[property: JsonPropertyName("title")] string Title,
		[property: JsonPropertyName("type")] string Type,
		[property: JsonPropertyName("url")] string Url)
{
	public DateTimeOffset CreatedAt_DateTimeOffset { get; } = DateTimeOffset.FromUnixTimeSeconds(CreatedAt_UnixTime);

	public string Description => ToString();

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