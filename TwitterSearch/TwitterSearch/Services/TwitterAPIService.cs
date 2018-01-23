using System.Threading.Tasks;

namespace TwitterSearch
{
    public abstract class TwitterAPIService : BaseHttpClientService
    {
        public static Task<TwitterSearchResult> GetSearchResult(string searchQuery) =>
            GetDataObjectFromAPI<TwitterSearchResult>(TwitterConstants.SearchAPIUrl + searchQuery);
    }
}
