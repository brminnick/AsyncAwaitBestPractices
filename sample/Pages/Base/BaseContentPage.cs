namespace HackerNews;

abstract partial class BaseContentPage<TViewModel> : ContentPage where TViewModel : BaseViewModel
{
	protected BaseContentPage(TViewModel viewModel, string pageTitle)
	{
		Title = pageTitle;
		base.BindingContext = viewModel;
	}

	protected new TViewModel BindingContext => (TViewModel)base.BindingContext;
}