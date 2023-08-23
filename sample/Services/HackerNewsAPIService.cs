namespace HackerNews;

class HackerNewsAPIService
{
	readonly IHackerNewsAPI _hackerNewsClient;

	public HackerNewsAPIService(IHackerNewsAPI hackerNewslient) => _hackerNewsClient = hackerNewslient;

	public Task<StoryModel> GetStory(long storyId) => _hackerNewsClient.GetStory(storyId);
	public Task<IReadOnlyList<long>> GetTopStoryIDs() => _hackerNewsClient.GetTopStoryIDs();
}