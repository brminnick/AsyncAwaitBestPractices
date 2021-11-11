using HackerNews.Shared;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Markup;
using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;

namespace HackerNews
{
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
				new TitleLabel().Row(Row.Title)
					.Bind(Label.TextProperty, nameof(StoryModel.Title)),
				new DescriptionLabel().Row(Row.Description)
					.Bind(Label.TextProperty, nameof(StoryModel.Description))
			}
		};

		enum Row { Title, Description, BottomPadding }

		class TitleLabel : Label
		{
			public TitleLabel()
			{
				FontSize = 16;
				TextColor = ColorConstants.TextCellTextColor;

				VerticalTextAlignment = TextAlignment.Start;

				Padding = new Thickness(10, 0);
			}
		}

		class DescriptionLabel : Label
		{
			public DescriptionLabel()
			{
				FontSize = 13;
				TextColor = ColorConstants.TextCellDetailColor;

				Padding = new Thickness(10, 0, 10, 5);
			}
		}
	}
}