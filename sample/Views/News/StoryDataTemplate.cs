using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace HackerNews;

partial class StoryDataTemplate : DataTemplate
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

		ColumnDefinitions = Columns.Define(
			(Column.LeftPadding, 6),
			(Column.Content, Star),
			(Column.RightPadding, 6)),

		Children =
		{
			new Label { MaxLines = 1, LineBreakMode = LineBreakMode.TailTruncation }
				.Row(Row.Title).Column(Column.Content).Top()
				.Font(size: 16).TextColor(ColorConstants.TextCellTextColor)
				.Bind(Label.TextProperty, static (StoryModel m) => m.Title, mode: BindingMode.OneTime),

			new Label()
				.Row(Row.Description).Column(Column.Content)
				.Font(size: 13).TextColor(ColorConstants.TextCellDetailColor)
				.Bind(Label.TextProperty, static (StoryModel m) => m.Description, mode: BindingMode.OneTime)
		}
	};

	enum Row { Title, Description, BottomPadding }
	enum Column { LeftPadding, Content, RightPadding }
}