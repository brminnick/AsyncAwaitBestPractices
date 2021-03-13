using HackerNews.Shared;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Markup;
using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;

namespace HackerNews
{
    public class StoryDataTemplate : DataTemplate
    {
        public StoryDataTemplate() : base(CreateGrid)
        {

        }

        public static GridLength AbsoluteGridLength(double value) => new GridLength(value, GridUnitType.Absolute);

        static Grid CreateGrid() => new()
        {
            RowSpacing = 1,

            RowDefinitions = Rows.Define(
                (Row.Title, AbsoluteGridLength(20)),
                (Row.Description, AbsoluteGridLength(20)),
                (Row.BottomPadding, AbsoluteGridLength(1))),

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