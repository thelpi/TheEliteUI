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
        private const int Steps = 50;
        private const int DelayBeforeRanking = 500;
        private const int TimerDelay = DelayBeforeRanking / Steps;
        private const Game SelectedGame = Game.GoldenEye;

        private const string StartAnimationLabel = "Start animation";
        private const string StopAnimationLabel = "Stop animation";

        private readonly IEliteProvider _eliteProvider;
        private readonly IClockProvider _clockProvider;
        private readonly Timer _timer;

        private bool _inProgress;
        private DateTime _currentDate;
        private int _step = 0;
        private int _daysBetweenRanking = 100;

        public MainWindow() : this(new EliteProvider(), new ClockProvider()) { }

        public MainWindow(IEliteProvider provider, IClockProvider clockProvider)
        {
            InitializeComponent();

            _eliteProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _clockProvider = clockProvider ?? throw new ArgumentNullException(nameof(clockProvider));

            _currentDate = PlayerRankingDto.RankingStart[SelectedGame];
            _timer = new Timer(TimerDelay);
            _timer.Elapsed += _timer_Elapsed;

            ChangeButtonStyle(false);
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
            
            if (_step == 0 && _currentDate >= _clockProvider.Today)
            {
                // stop animation after current date
                Dispatcher.Invoke(StopAnimation);
            }
            else
            {
                if (_step == 0)
                {
                    // if the next date is past today, we force today
                    _currentDate = _currentDate.AddDays(_daysBetweenRanking);
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
                            SetRankingViewItems(
                                RankingView,
                                rankingItems.Select(r => new PlayerRanking(r)),
                                CreatePlayerRankingControl);
                            SetRankingViewItems(
                                WrStandingUntiedView,
                                wrStandingUntiedItems.Select(r => new WrRanking(r, true)),
                                CreateWrRankingControl);
                            SetRankingViewItems(
                                WrStandingView,
                                wrStandingItems.Select(r => new WrRanking(r, false)),
                                CreateWrRankingControl);
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

        private WrRankingControl CreateWrRankingControl(WrRanking r)
        {
            var control = new WrRankingControl(r, Steps, RankingView);
            // the tooltip is rebuild each time the mouse is hover the control
            // but the content while hovering is not dynamic
            control.MouseEnter += (s, e) => control.ToolTip = (control.DataContext as WrRanking).GetToolTip();
            return control;
        }

        private PlayerRankingControl CreatePlayerRankingControl(PlayerRanking r)
        {
            return new PlayerRankingControl(r, Steps, RankingView);
        }

        private void SetRankingViewItems<TItem, TControl>(
            Canvas view,
            IEnumerable<TItem> rankingItems,
            Func<TItem, TControl> ctorFunc)
            where TItem : IRanking
            where TControl : RankingControl
        {
            foreach (var item in rankingItems)
            {
                AddOrUpdateRanking(view, item, r => ctorFunc(r));
            }
            ClearObsoleteItemsFromRankinkView<TControl>(view, rankingItems.Select(r => (IRanking)r));
        }

        private void AddOrUpdateRanking<T>(Canvas view, T item, Func<T, RankingControl> ctorFunc)
            where T : IRanking
        {
            var ranking = GetItems<RankingControl>(view)
                .SingleOrDefault(r => r.Item.IsKey(item.Key));
            if (ranking == null)
            {
                view.Children.Add(ctorFunc(item));
            }
            else
            {
                ranking.UpdateItemtarget(item);
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
            ChangeButtonStyle(true);
        }

        private void StopAnimation()
        {
            _timer.Stop();
            ChangeButtonStyle(false);
        }

        private void ChangeButtonStyle(bool toStop)
        {
            var imgSource = UiExtensions.CreateImgSource(toStop ? "noatunpause.png" : "noatunplay.png");
            var img = AnimationButton.Content as Image;
            img.ToolTip = toStop ? StopAnimationLabel : StartAnimationLabel;
            img.Source = imgSource;
        }

        private IEnumerable<T> GetItems<T>(Canvas view)
        {
            return view.Children.OfType<T>();
        }

        private void DaysBetweenRankingText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(DaysBetweenRankingText.Text, out int daysBetweenRankingText))
            {
                _daysBetweenRanking = daysBetweenRankingText;
            }
        }
    }
}
