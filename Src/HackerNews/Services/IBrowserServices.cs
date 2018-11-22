using System.Threading.Tasks;

namespace HackerNews
{
    public interface IBrowserServices
    {
        Task OpenBrowser(string url);
    }
}

