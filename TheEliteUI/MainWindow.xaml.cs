using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using TheEliteUI.Models;
using TheEliteUI.Providers;

namespace TheEliteUI
{
    public partial class MainWindow : Window
    {
        private const int Steps = 100;
        private const int DelayBeforeRanking = 1000;
        private const int TimerDelay = DelayBeforeRanking / Steps;
        private const Game SelectedGame = Game.GoldenEye;
        private const int DaysBetweenRanking = 200;

        private const string StartAnimationLabel = "Start Animation";
        private const string StopAnimationLabel = "Stop Animation";

        private readonly IRankingProvider _rankingProvider;
        private readonly IClockProvider _clockProvider;
        private readonly Timer _timer;

        private bool _inProgress;
        private DateTime _currentDate;
        private int _step = 0;

        public MainWindow()
            : this(new RankingProvider(), new ClockProvider())
        { }

        public MainWindow(
            IRankingProvider provider,
            IClockProvider clockProvider)
        {
            InitializeComponent();

            _rankingProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _clockProvider = clockProvider ?? throw new ArgumentNullException(nameof(clockProvider));

            _currentDate = Ranking.RankingStart[SelectedGame];
            _timer = new Timer(TimerDelay);
            _timer.Elapsed += _timer_Elapsed;

            AnimationButton.Content = StartAnimationLabel;
            RankingDatePicker.SelectedDate = _currentDate;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_inProgress)
            {
                // avoid multiple time executions
                return;
            }

            _inProgress = true;
            
            if (_currentDate >= _clockProvider.Today)
            {
                // stop animation after current date
                Dispatcher.Invoke(StopAnimation);
            }
            else
            {
                if (_step == 0)
                {
                    // if the next date is past today, we force today
                    _currentDate = _currentDate.AddDays(DaysBetweenRanking);
                    if (_currentDate > _clockProvider.Today)
                    {
                        _currentDate = _clockProvider.Today;
                    }

                    var rankingItems = _rankingProvider.GetRanking(SelectedGame, _currentDate, 0, Ranking.DefaultPaginationLimit);
                    
                    // can fail on window closing
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            SetRankingViewItems(rankingItems);
                            RankingDatePicker.SelectedDate = _currentDate;
                        });
                    }
                    catch { }
                }

                // can fail on window closing
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        RefreshPlayersTopPosition();
                    });
                }
                catch { }

                _step++;
                _step = _step == Steps ? 0 : _step;
            }
            
            _inProgress = false;
        }

        private void SetRankingViewItems(IReadOnlyCollection<Ranking> rankingItems)
        {
            foreach (var item in rankingItems)
            {
                AddOrUpdatePlayerRanking(item);
            }
            ClearObsoletePlayersFromRankinkView(rankingItems.Select(i => i.PlayerId));
        }

        private void AddOrUpdatePlayerRanking(Ranking item)
        {
            var ranking = GetPlayerRankings()
                .SingleOrDefault(r => r.PlayerId == item.PlayerId);
            if (ranking == null)
            {
                var rk = new PlayerRanking(item, Steps);
                RankingView.Children.Add(rk);
            }
            else
            {
                ranking.UpdateItemtarget(item);
            }
        }

        private void ClearObsoletePlayersFromRankinkView(IEnumerable<long> playerIdsToKeep)
        {
            GetPlayerRankings()
                .Where(r => !playerIdsToKeep.Contains(r.PlayerId))
                .ToList()
                .ForEach(r => RankingView.Children.Remove(r));
        }

        private void RefreshPlayersTopPosition()
        {
            GetPlayerRankings()
                .ToList()
                .ForEach(r => r.ArrangeControl());
        }

        private void AnimationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.Enabled)
            {
                StopAnimation();
            }
            else
            {
                StartAnimation();
            }
        }

        private void StartAnimation()
        {
            _currentDate = RankingDatePicker.SelectedDate.Value;
            _timer.Start();
            AnimationButton.Content = StopAnimationLabel;
        }

        private void StopAnimation()
        {
            _timer.Stop();
            AnimationButton.Content = StartAnimationLabel;
        }

        private IEnumerable<PlayerRanking> GetPlayerRankings()
        {
            return RankingView.Children.OfType<PlayerRanking>();
        }
    }
}
