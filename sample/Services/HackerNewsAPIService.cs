using System.Collections.Frozen;

namespace HackerNews;

class HackerNewsAPIService
{
	readonly IHackerNewsAPI _hackerNewsClient;

	public HackerNewsAPIService(IHackerNewsAPI hackerNewslient) => _hackerNewsClient = hackerNewslient;

	public Task<StoryModel> GetStory(long storyId, CancellationToken token) => _hackerNewsClient.GetStory(storyId, token);
	public async Task<FrozenSet<long>> GetTopStoryIDs(CancellationToken token)
	{
		var topStoryIds = await _hackerNewsClient.GetTopStoryIDs(token).ConfigureAwait(false);
		return topStoryIds.ToFrozenSet();
	}
}