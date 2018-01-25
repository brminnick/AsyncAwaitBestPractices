using System;

using Xamarin.Forms;

namespace HackerNews
{
    public class StoryTextCell : TextCell
    {
        public StoryTextCell() => DetailColor = Color.FromHex("828282");

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is StoryModel story)
            {
                Text = story?.Title;
                Detail = $"{story.Score} Points by {story.Author} {GetAgeOfStory(story.CreatedAt_DateTimeOffset)} ago";
            }
        }

        string GetAgeOfStory(DateTimeOffset storyCreatedAt)
        {
            var timespanSinceStoryCreated = DateTimeOffset.UtcNow - storyCreatedAt;

            if (timespanSinceStoryCreated < TimeSpan.FromHours(1))
                return $"{Math.Ceiling(timespanSinceStoryCreated.TotalMinutes)} minutes";

            if (timespanSinceStoryCreated >= TimeSpan.FromHours(1) && timespanSinceStoryCreated < TimeSpan.FromHours(2))
                return $"{Math.Floor(timespanSinceStoryCreated.TotalHours)} hour";

            return $"{Math.Floor(timespanSinceStoryCreated.TotalHours)} hours";
        }
    }
}
