using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TheEliteUI.Dtos;
using TheEliteUI.Providers;

namespace TheEliteUI
{
    /// <summary>
    /// Logique d'interaction pour LevelHeatMapWindow.xaml
    /// </summary>
    public partial class LevelHeatMapWindow : Window
    {
        private readonly IEliteProvider _eliteProvider;
        private readonly IClockProvider _clockProvider;

        public LevelHeatMapWindow() : this(new EliteProvider(), new ClockProvider()) { }

        public LevelHeatMapWindow(IEliteProvider eliteProvider, IClockProvider clockProvider)
        {
            InitializeComponent();

            _eliteProvider = eliteProvider;
            _clockProvider = clockProvider;

            Task.Run(() => Initialize());
        }

        private void Initialize()
        {
            var threshold = 8;

            //var startDate = PlayerRankingDto.RankingStart[Game.GoldenEye];
            var today = _clockProvider.Today.AddDays(-1);
            var startDate = today.AddMonths(-20 * 12);

            var totalDaysCount = (int)Math.Floor((today.AddDays(1) - startDate).TotalDays);

            var currentColIndex = 1;
            var currentDate = startDate;
            while (currentDate < today)
            {
                var nextDate = currentDate.AddMonths(1);

                var periodDays = (int)Math.Floor((nextDate - currentDate).TotalDays);

                var daysRate = periodDays / (decimal)totalDaysCount;

                var stagesStats = _eliteProvider.GetStagesEntriesCount(Game.GoldenEye, currentDate, nextDate);

                Dispatcher.Invoke(() =>  MainGrid.ColumnDefinitions.Add(new ColumnDefinition()));
                currentColIndex++;

                foreach (var stageStats in stagesStats)
                {
                    var avgCountExpected = stageStats.TotalEntriesCount * daysRate;

                    var rate = stageStats.PeriodEntriesCount / avgCountExpected;

                    rate = rate > threshold ? threshold : rate;

                    var noRedValue = (byte)(255 - Math.Floor((255 * rate) / threshold));

                    Dispatcher.Invoke(() =>
                    {
                        var rectangle = new Rectangle
                        {
                            Fill = new SolidColorBrush(Color.FromRgb(255, noRedValue, noRedValue)),
                            Width = 7,
                            Height = 50,
                            ToolTip = $"{stageStats.Stage} - {stageStats.StartDate.ToString("yyyy-MM-dd")} - {stageStats.EndDate.ToString("yyyy-MM-dd")} - {stageStats.PeriodEntriesCount} runs"
                        };

                        rectangle.SetValue(Grid.RowProperty, ((int)stageStats.Stage) - 1);
                        rectangle.SetValue(Grid.ColumnProperty, currentColIndex);

                        MainGrid.Children.Add(rectangle);
                    });
                }

                currentDate = nextDate;
            }

            Dispatcher.Invoke(() =>
            {
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
                for (int i = 0; i < 20; i++)
                {
                    var year = i + 2002;
                    var label = new Label
                    {
                        Content = year,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Background = Brushes.White
                    };

                    label.SetValue(Grid.RowProperty, 20);
                    label.SetValue(Grid.ColumnProperty, (i * 12) + 1);
                    label.SetValue(Grid.ColumnSpanProperty, 12);

                    MainGrid.Children.Add(label);
                }
            });
        }
    }
}
