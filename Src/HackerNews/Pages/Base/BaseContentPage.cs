using Xamarin.Forms;

namespace HackerNews
{
    public abstract class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
    {
        #region Constructors
        protected BaseContentPage(string pageTitle)
        {
            BindingContext = ViewModel;
            Title = pageTitle;
        }
        #endregion

        #region Properties
        protected T ViewModel { get; } = new T();
        #endregion
    }
}
