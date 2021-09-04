using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            const int stagesCount = 20;
            const int years = 20;
            var endDateExclusive = new DateTime(2021, 9, 1);
            var startDateInclusive = endDateExclusive.AddYears(-years);
            var totalMonthsCount = years * 12;

            //var avg1 = 1 / (double)stagesCount;
            //var avg2 = 1 / (double)totalMonthsCount;
            //var avg = Math.Sqrt(avg1 * avg1 + avg2 * avg2) / Math.Sqrt(2);

            int startAtMonth = startDateInclusive.Month;
            int span = 0;
            var col = 1;
            var currentColIndex = 0;

            var currentDate = startDateInclusive;
            for (int i = 0; i < totalMonthsCount; i++)
            {
                var nextDate = currentDate.AddMonths(1);

                Dispatcher.Invoke(() => MainGrid.ColumnDefinitions.Add(new ColumnDefinition()));
                currentColIndex++;

                var stagesStats = _eliteProvider.GetStagesEntriesCount(Game.GoldenEye, currentDate, nextDate, startDateInclusive, endDateExclusive);

                foreach (var stageStats in stagesStats)
                {
                    var rate1 = stageStats.PeriodEntriesCount / (double)stageStats.AllStagesEntriesCount;
                    var rate2 = stageStats.PeriodEntriesCount / (double)stageStats.TotalEntriesCount;

                    var rate = Math.Sqrt(rate1 * rate1 + rate2 * rate2) / Math.Sqrt(2);

                    System.Diagnostics.Debug.WriteLine(rate);

                    var rateToScale = rate == 0 ? 0 : rate / Math.Sqrt(rate);

                    var notRedBytes = Convert.ToByte(255 - (rateToScale * 255));

                    Dispatcher.Invoke(() =>
                    {
                        var rectangle = new Canvas
                        {
                            Background = new SolidColorBrush(Color.FromRgb(255, notRedBytes, notRedBytes)),
                            VerticalAlignment = VerticalAlignment.Stretch,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            ToolTip = $"{stageStats.Stage} - {stageStats.PeriodEntriesCount} runs"
                        };

                        rectangle.SetValue(Grid.RowProperty, ((int)stageStats.Stage) - 1);
                        rectangle.SetValue(Grid.ColumnProperty, currentColIndex);

                        MainGrid.Children.Add(rectangle);
                    });
                }

                var isLastLoop = i == totalMonthsCount - 1;
                if (currentDate.Month == 12 || isLastLoop)
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

                        span = isLastLoop ? currentDate.Month : 12 - (startAtMonth - 1);

                        label.SetValue(Grid.ColumnSpanProperty, span);

                        MainGrid.Children.Add(label);
                    });
                    startAtMonth = 1;
                }

                currentDate = nextDate;
            }
        }
    }
}
