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
            const int threshold = 50;

            const int stagesCount = 20;

            //var startDate = PlayerRankingDto.RankingStart[Game.GoldenEye];
            var years = 10;
            var today = _clockProvider.Today;
            var startDate = today.AddDays(-1).AddMonths(-years * 12);

            var totalDaysCount = (int)Math.Floor((today - startDate).TotalDays);

            var currentColIndex = 0;
            var currentDate = startDate;
            int startAtMonth = currentDate.Month;
            int span = 0;
            var col = 1;
            do
            {
                var nextDate = currentDate.AddMonths(1);

                var periodDays = (int)Math.Floor((nextDate - currentDate).TotalDays);

                var daysRate = periodDays / (decimal)totalDaysCount;

                var stagesStats = _eliteProvider.GetStagesEntriesCount(Game.GoldenEye, currentDate, nextDate, startDate, today);

                Dispatcher.Invoke(() => MainGrid.ColumnDefinitions.Add(new ColumnDefinition()));
                currentColIndex++;

                foreach (var stageStats in stagesStats)
                {
                    var avgCountExpected = stageStats.TotalEntriesCount * daysRate;
                    var avgCountExpected2 = stageStats.AllStagesEntriesCount / (decimal)stagesCount;

                    var rate = avgCountExpected == 0 ? 0 : stageStats.PeriodEntriesCount / avgCountExpected;
                    var rate2 = avgCountExpected2 == 0 ? 0 : stageStats.PeriodEntriesCount / avgCountExpected2;

                    var finalRate = (rate * rate2 * rate2 * rate2 * rate2) > threshold ? threshold : (rate * rate2 * rate2 * rate2 * rate2);

                    var noRedValue = (byte)(255 - Math.Floor((255 * finalRate) / threshold));

                    Dispatcher.Invoke(() =>
                    {
                        var rectangle = new Canvas
                        {
                            Background = new SolidColorBrush(Color.FromRgb(255, noRedValue, 0)),
                            VerticalAlignment = VerticalAlignment.Stretch,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            ToolTip = $"{stageStats.Stage} - {stageStats.StartDate.ToString("yyyy-MM-dd")} - {stageStats.EndDate.ToString("yyyy-MM-dd")} - {stageStats.PeriodEntriesCount} runs"
                        };

                        rectangle.SetValue(Grid.RowProperty, ((int)stageStats.Stage) - 1);
                        rectangle.SetValue(Grid.ColumnProperty, currentColIndex);

                        MainGrid.Children.Add(rectangle);
                    });
                }

                bool willBreak = nextDate.AddDays(1) >= today;
                if (currentDate.Month == 12 || willBreak)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var label = new Label
                        {
                            Content = currentDate.Year,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            Background = Brushes.White,
                            BorderThickness = new Thickness(1),
                            BorderBrush = Brushes.DarkBlue
                        };

                        col += span;

                        label.SetValue(Grid.RowProperty, stagesCount);
                        label.SetValue(Grid.ColumnProperty, col);

                        span = willBreak ? currentDate.Month : 12 - (startAtMonth - 1);

                        label.SetValue(Grid.ColumnSpanProperty, span);

                        MainGrid.Children.Add(label);
                    });
                    startAtMonth = 1;
                }

                currentDate = nextDate;
            }
            while (currentDate.AddDays(1) < today);
        }
    }
}
