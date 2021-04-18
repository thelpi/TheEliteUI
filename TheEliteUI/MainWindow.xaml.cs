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
        private const int TimerDelay = 1000;
        private const Game SelectedGame = Game.GoldenEye;

        private readonly IBackProvider _provider;
        private readonly Timer _timer;

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
            RankingDatePicker.SelectedDateChanged += RankingDatePicker_SelectedDateChanged;
            RankingDatePicker.SelectedDate = _rankingStart[SelectedGame];
            _timer = new Timer(TimerDelay);
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var currentDate = RankingDatePicker.SelectedDate.Value;

                if (currentDate >= DateTime.Today)
                {
                    AnimationButton_Click(null, null);
                    return;
                }

                currentDate = currentDate.AddDays(7);
                if (currentDate > DateTime.Today)
                {
                    currentDate = DateTime.Today;
                }

                RankingDatePicker.SelectedDate = currentDate;
            });
        }

        private void RankingDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var itemsSource = _provider
                .GetRankingAsync(SelectedGame, RankingDatePicker.SelectedDate.Value)
                .GetAwaiter()
                .GetResult();

            RankingView.ItemsSource = itemsSource
                .Select((_ , r) => new ViewModel.RankingViewModel(_, r + 1))
                .ToList();
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
