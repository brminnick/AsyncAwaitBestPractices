using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace HackerNews;

class StoryDataTemplate : DataTemplate
{
	public StoryDataTemplate() : base(CreateGrid)
	{

	}

	static Grid CreateGrid() => new()
	{
		RowSpacing = 1,

		RowDefinitions = Rows.Define(
			(Row.Title, 20),
			(Row.Description, 20),
			(Row.BottomPadding, 1)),

		Children =
		{
			new Label()
				.Row(Row.Title).Top()
				.Font(size: 16).TextColor(ColorConstants.TextCellTextColor)
				.Bind(Label.TextProperty, static (StoryModel m) => m.Title, mode: BindingMode.OneTime),

			new Label()
				.Row(Row.Description)
				.Font(size: 13).TextColor(ColorConstants.TextCellDetailColor)
				.Bind(Label.TextProperty, static (StoryModel m) => m.Description, mode: BindingMode.OneTime)
		}
	};

	enum Row { Title, Description, BottomPadding }
}