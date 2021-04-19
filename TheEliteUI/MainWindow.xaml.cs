using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using TheEliteUI.Model;

namespace TheEliteUI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int TimerDelay = 200;
        private const Game SelectedGame = Game.GoldenEye;

        private readonly IBackProvider _provider;
        private readonly Timer _timer;
        private bool _inProgress;
        private DateTime _currentDate;

        private readonly IReadOnlyDictionary<Game, DateTime> _rankingStart = new Dictionary<Game, DateTime>
        {
            { Game.GoldenEye, new DateTime(1998, 07, 26) },
            { Game.PerfectDark, new DateTime(2000, 01, 01) }
        };

        public MainWindow()
            : this(new BackProvider())
        { }

        public MainWindow(IBackProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            InitializeComponent();
            _currentDate = _rankingStart[SelectedGame];
            //RankingDatePicker.SelectedDateChanged += RankingDatePicker_SelectedDateChanged;
            RankingDatePicker.SelectedDate = _currentDate;
            _timer = new Timer(TimerDelay);
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_inProgress)
            {
                System.Diagnostics.Debug.WriteLine("Reentrance");
                return;
            }

            _inProgress = true;

            if (_currentDate >= DateTime.Today)
            {
                Dispatcher.Invoke(() => AnimationButton_Click(null, null));
                _inProgress = false;
                return;
            }

            _currentDate = _currentDate.AddDays(7);
            if (_currentDate > DateTime.Today)
            {
                _currentDate = DateTime.Today;
            }

            var itemsSource = _provider
                .GetRankingAsync(SelectedGame, _currentDate)
                .GetAwaiter()
                .GetResult()
                .Select((r, i) => r.WithRank(i + 1))
                .ToList();

            Dispatcher.Invoke(() =>
            {
                RankingView.ItemsSource = itemsSource;
                RankingDatePicker.SelectedDate = _currentDate;
            });

            _inProgress = false;
        }

        private void AnimationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
                AnimationButton.Content = "Start Animation";
            }
            else
            {
                _timer.Start();
                AnimationButton.Content = "Stop Animation";
            }
        }
    }
}
