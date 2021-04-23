using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using TheEliteUI.Dtos;
using TheEliteUI.Providers;
using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    public partial class MainWindow : Window
    {
        private const int Steps = 100;
        private const int DelayBeforeRanking = 1000;
        private const int TimerDelay = DelayBeforeRanking / Steps;
        private const Game SelectedGame = Game.GoldenEye;
        private const int DaysBetweenRanking = 50;

        private const string StartAnimationLabel = "Start Animation";
        private const string StopAnimationLabel = "Stop Animation";

        private readonly IEliteProvider _eliteProvider;
        private readonly IClockProvider _clockProvider;
        private readonly Timer _timer;

        private bool _inProgress;
        private DateTime _currentDate;
        private int _step = 0;

        public MainWindow()
            : this(new EliteProvider(), new ClockProvider())
        { }

        public MainWindow(
            IEliteProvider provider,
            IClockProvider clockProvider)
        {
            InitializeComponent();

            _eliteProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _clockProvider = clockProvider ?? throw new ArgumentNullException(nameof(clockProvider));

            _currentDate = PlayerRankingDto.RankingStart[SelectedGame];
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

                    var rankingItems = _eliteProvider.GetRanking(SelectedGame, _currentDate, 0, PlayerRankingDto.DefaultPaginationLimit);
                    var wrStandingUntiedItems = _eliteProvider.GetStandingWr(SelectedGame, _currentDate, true, 0, StandingWrDto.DefaultPaginationLimit);
                    var wrStandingItems = _eliteProvider.GetStandingWr(SelectedGame, _currentDate, false, 0, StandingWrDto.DefaultPaginationLimit);

                    // can fail on window closing
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            SetRankingViewItems(RankingView, rankingItems.Select(r => new PlayerRanking(r)));
                            SetRankingViewItems(WrStandingUntiedView, wrStandingUntiedItems.Select(r => new WrRanking(r)), true);
                            SetRankingViewItems(WrStandingView, wrStandingItems.Select(r => new WrRanking(r)), false);
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
                        RefreshItemsTopPosition<PlayerRankingControl>(RankingView);
                        RefreshItemsTopPosition<WrRankingControl>(WrStandingUntiedView);
                        RefreshItemsTopPosition<WrRankingControl>(WrStandingView);
                    });
                }
                catch { }

                _step++;
                _step = _step == Steps ? 0 : _step;
            }
            
            _inProgress = false;
        }

        private void SetRankingViewItems(Canvas view, IEnumerable<PlayerRanking> rankingItems)
        {
            foreach (var item in rankingItems)
            {
                AddOrUpdatePlayerRanking(view, item);
            }
            ClearObsoleteItemsFromRankinkView<PlayerRankingControl>(view, rankingItems);
        }

        private void SetRankingViewItems(Canvas view, IEnumerable<WrRanking> rankingItems, bool untiedMode)
        {
            foreach (var item in rankingItems)
            {
                AddOrUpdateWrStandingRanking(view, item, untiedMode);
            }
            ClearObsoleteItemsFromRankinkView<WrRankingControl>(view, rankingItems);
        }

        private void AddOrUpdatePlayerRanking(Canvas view, PlayerRanking item)
        {
            var ranking = GetItems<PlayerRankingControl>(view)
                .SingleOrDefault(r => r.Item.IsKey(item.Key));
            if (ranking == null)
            {
                var rk = new PlayerRankingControl(item, Steps);
                view.Children.Add(rk);
            }
            else
            {
                // TODO: set once
                ranking.UpdateItemtarget(item, PlayerRankingDto.MinPoints, PlayerRankingDto.MaxPoints);
            }
        }

        private void AddOrUpdateWrStandingRanking(Canvas view, WrRanking item, bool untiedMode)
        {
            var ranking = GetItems<WrRankingControl>(view)
                .SingleOrDefault(r => r.Item.IsKey(item.Key));
            if (ranking == null)
            {
                var rk = new WrRankingControl(item, Steps, untiedMode);
                view.Children.Add(rk);
            }
            else
            {
                // TODO: set once
                ranking.UpdateItemtarget(item, StandingWrDto.MinDays,
                    untiedMode ? StandingWrDto.MaxDaysUntied : StandingWrDto.MaxDaysTied);
            }
        }

        private void ClearObsoleteItemsFromRankinkView<T>(Canvas view, IEnumerable<IRanking> itemsToKeep) where T : RankingControl
        {
            GetItems<T>(view)
                .Where(r => !itemsToKeep.Any(_ => _.IsKey(r.Item.Key)))
                .ToList()
                .ForEach(r => view.Children.Remove(r));
        }

        private void RefreshItemsTopPosition<T>(Canvas view) where T : RankingControl
        {
            GetItems<T>(view)
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

        private IEnumerable<T> GetItems<T>(Canvas view)
        {
            return view.Children.OfType<T>();
        }
    }
}
