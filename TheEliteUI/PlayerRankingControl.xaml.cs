using System;
using System.Windows.Controls;
using TheEliteUI.Dtos;
using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    public partial class PlayerRankingControl : UserControl
    {
        public static double ControlHeight { get; } = 30;
        public static double ControlBorderThickness { get; } = 3;
        public static double Realheight => ControlHeight + (2 * ControlBorderThickness);

        private const double PlayerNamePixels = 150;
        private const double PixelsByPoint = 800;
        private const double PointsMargin = 100;

        public PlayerRanking Player { get; }

        private readonly SourceToTargetByStep _top;
        private readonly SourceToTargetByStep _width;
        private readonly SourceToTargetByStep _points;
        private readonly SourceToTargetByStep _rank;

        public PlayerRankingControl(PlayerRanking item, int steps)
        {
            InitializeComponent();
            Player = item;
            DataContext = item;
            
            _top = new SourceToTargetByStep(GetTargetTop(item), steps, PlayerRankingDto.DefaultPaginationLimit * Realheight);
            _width = new SourceToTargetByStep(GetTargetWidth(item), steps, 0);
            _points = new SourceToTargetByStep(item.Value, steps, 0);
            _rank = new SourceToTargetByStep(item.Rank, steps, PlayerRankingDto.DefaultPaginationLimit + 1);

            ArrangeControl(false);
        }

        internal void UpdateItemtarget(PlayerRanking item)
        {
            DataContext = item;
            _top.SetNewTarget(GetTargetTop(item));
            _width.SetNewTarget(GetTargetWidth(item));
            _points.SetNewTarget(item.Value);
            _rank.SetNewTarget(item.Rank);
        }

        internal void ArrangeControl()
        {
            ArrangeControl(true);
        }

        private void ArrangeControl(bool step)
        {
            SetValue(Canvas.TopProperty, _top.SetCurrentStep(step));
            MainCanvas.Width = _width.SetCurrentStep(step);
            PointsLabel.Content = Convert.ToInt32(_points.SetCurrentStep(step));
            RankLabel.Content = Convert.ToInt32(_rank.SetCurrentStep(step)).ToString().PadLeft(2, '0');
        }

        private static double GetTargetTop(PlayerRanking item)
        {
            return (item.Rank - 1) * Realheight;
        }

        private static double GetTargetWidth(PlayerRanking item)
        {
            // TODO: "PointsMargin" should be in pixels unit
            var pointsToConsider = item.Value > PlayerRankingDto.MinPoints
                ? item.Value - (PlayerRankingDto.MinPoints - PointsMargin)
                : PointsMargin;
            return PlayerNamePixels + ((pointsToConsider * PixelsByPoint) / PlayerRankingDto.MaxPoints);
        }

        private class SourceToTargetByStep
        {
            private double _current;
            private double _target;
            private double _step;
            private readonly int _stepsCount;

            public SourceToTargetByStep(double targetValue, int stepsCount, double currentValue)
            {
                _stepsCount = stepsCount;
                _current = currentValue;
                _target = targetValue;
                SetStep();
            }

            public void SetNewTarget(double targetValue)
            {
                _current = _target;
                _target = targetValue;
                SetStep();
            }

            private void SetStep()
            {
                _step = (_target - _current) / _stepsCount;
            }

            public double SetCurrentStep(bool step)
            {
                if (step)
                {
                    _current += _step;
                }
                return _current;
            }
        }
    }
}
