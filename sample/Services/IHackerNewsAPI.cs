using Refit;

namespace HackerNews;

interface IHackerNewsAPI
{
	[Get("/topstories.json?print=pretty")]
	Task<IReadOnlyList<long>> GetTopStoryIDs(CancellationToken token);

	[Get("/item/{storyId}.json?print=pretty")]
	Task<StoryModel> GetStory(long storyId, CancellationToken token);
}