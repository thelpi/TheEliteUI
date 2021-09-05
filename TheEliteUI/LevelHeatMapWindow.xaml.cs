using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private readonly Timer _timer;

        public LevelHeatMapWindow() : this(new EliteProvider(), new ClockProvider()) { }

        public LevelHeatMapWindow(IEliteProvider eliteProvider, IClockProvider clockProvider)
        {
            InitializeComponent();

            _eliteProvider = eliteProvider;
            _clockProvider = clockProvider;
            _timer = new Timer(2000);
            _timer.AutoReset = false;
            _timer.Elapsed += (a, b) =>
            {
                Dispatcher.Invoke(() =>
                {
                    GridScreenshot();
                    MainGrid.Children.OfType<TextBox>().ToList().ForEach(t => t.Text = string.Empty);
                });
                System.Threading.Thread.Sleep(2000);
                Dispatcher.Invoke(() =>
                {
                    GridScreenshot();
                });
            };

            Task.Run(() => Initialize());
        }

        private void Initialize()
        {
            const int stagesCount = 20;
            const int years = 22;
            var endDateExclusive = new DateTime(2021, 9, 1);
            var startDateInclusive = endDateExclusive.AddYears(-years);
            var totalMonthsCount = years * 12;

            var entries = new Dictionary<int, List<Dtos.StageEntryCountDto>>();

            var entriesMaxRate = 0.07; // arbitrary but seems OK
            var currentDate = startDateInclusive;
            for (var i = 0; i < totalMonthsCount; i++)
            {
                var nextDate = currentDate.AddMonths(1);

                var stagesStats = _eliteProvider
                    .GetStagesEntriesCount(Game.GoldenEye, currentDate, nextDate, startDateInclusive, endDateExclusive, true)
                    .OrderBy(_ => (int)_.Stage)
                    .ThenBy(_ => (int)_.Level.Value)
                    .ToList();

                /* it works, but Bunker2 in august 2021 fucks up the scale */
                //var localMax = stagesStats.Max(_ => _.EntryRate);
                //if (double.IsNaN(entriesMaxRate) || localMax > entriesMaxRate)
                //{
                //   entriesMaxRate = localMax;
                //}

                entries.Add(i, stagesStats);

                currentDate = nextDate;
            }

            var startAtMonth = startDateInclusive.Month;
            var span = 0;
            var col = 1;
            var currentColIndex = 0;

            currentDate = startDateInclusive;
            for (var i = 0; i < totalMonthsCount; i++)
            {
                var nextDate = currentDate.AddMonths(1);

                Dispatcher.Invoke(() => MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) }));
                currentColIndex++;

                foreach (var stageStats in entries[i])
                {
                    var rateConsidered = double.IsNaN(stageStats.EntryRate)
                        ? 0
                        : (stageStats.EntryRate > entriesMaxRate
                            ? entriesMaxRate
                            : stageStats.EntryRate);
                    var notRedBytes = Convert.ToByte(255 - ((rateConsidered / entriesMaxRate) * 255));

                    Dispatcher.Invoke(() =>
                    {
                        var rectangle = new TextBox
                        {
                            Background = new SolidColorBrush(Color.FromRgb(255, notRedBytes, notRedBytes)),
                            VerticalAlignment = VerticalAlignment.Stretch,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            Text = $"{stageStats.PeriodEntriesCount}",
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            FontSize = 10
                        };

                        var baseRowIndex = ((int)stageStats.Stage - 1) * 3;
                        var addRowIndex = (int)stageStats.Level.Value - 1;

                        rectangle.SetValue(Grid.RowProperty, baseRowIndex + addRowIndex);
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

                        label.SetValue(Grid.RowProperty, stagesCount * 3);
                        label.SetValue(Grid.ColumnProperty, col);

                        span = isLastLoop ? currentDate.Month : 12 - (startAtMonth - 1);

                        label.SetValue(Grid.ColumnSpanProperty, span);

                        MainGrid.Children.Add(label);
                    });
                    startAtMonth = 1;
                }

                currentDate = nextDate;
            }

            Dispatcher.Invoke(() =>
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                var colIndex = MainGrid.ColumnDefinitions.Count - 1;
                for (int i = 0; i < stagesCount; i++)
                {
                    var img = new Image
                    {
                        Source = new BitmapImage(new Uri($"pack://application:,,,/TheEliteUI;component/Resources/Stages/{i + 1}.jpg"))
                    };

                    img.SetValue(Grid.RowProperty, i * 3);
                    img.SetValue(Grid.RowSpanProperty, 3);
                    img.SetValue(Grid.ColumnProperty, colIndex);

                    MainGrid.Children.Add(img);
                }
            });

            _timer.Start();
        }

        private void GridScreenshot()
        {
            try
            {
                var width = (int)MainGrid.ActualWidth + 10;
                var height = (int)MainGrid.ActualHeight + 10;

                var renderBitmap = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Pbgra32);
                MainGrid.Measure(new Size(width, height));
                MainGrid.Arrange(new Rect(new Size(width, height)));

                renderBitmap.Render(MainGrid);

                var pngImage = new PngBitmapEncoder();
                pngImage.Frames.Add(BitmapFrame.Create(renderBitmap));
                using (var fileStream = File.Create($@"C:\Users\LPI\Desktop\souk_again\yo\{DateTime.Now.ToString("yyyyMMddhhmmss")}.jpg"))
                {
                    pngImage.Save(fileStream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while screenshoting : " + ex.Message);
            }
        }
    }
}
