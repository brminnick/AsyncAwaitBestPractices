using Xamarin.Forms;

namespace HackerNews
{
    public class StoryViewCell : TextCell
    {
        public StoryViewCell()
        {
            this.SetBinding(TextProperty, nameof(StoryModel.Title));
            this.SetBinding(DetailProperty, nameof(StoryModel.By));
        }
    }
}
