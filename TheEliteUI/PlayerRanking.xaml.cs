using System;
using System.Windows.Controls;
using TheEliteUI.Models;

namespace TheEliteUI
{
    public partial class PlayerRanking : UserControl
    {
        public static double ControlHeight { get; } = 30;
        public static double ControlBorderThickness { get; } = 3;
        public static double Realheight => ControlHeight + (2 * ControlBorderThickness);

        private const double PlayerNamePixels = 150;
        private const double PixelsByPoint = 800;
        private const double PointsMargin = 100;

        private readonly int StepsCount;

        public long PlayerId { get; }
        
        private double _targetTopPx;
        private double _sourceTopPx;
        private double _stepTopPx;

        private double _targetWidthPx;
        private double _sourceWidthPx;
        private double _stepWidthPx;

        private double _targetPoints;
        private double _sourcePoints;
        private double _stepPoints;

        private double _targetRank;
        private double _sourceRank;
        private double _stepRank;

        public PlayerRanking(Ranking item, int steps)
        {
            InitializeComponent();
            PlayerId = item.PlayerId;
            DataContext = item;
            StepsCount = steps;
            SetTheoricalValues(item, true);
            SetInitialValues();
        }

        internal void Update(Ranking item)
        {
            DataContext = item;
            SetTheoricalValues(item, false);
        }

        internal void SetActualValues()
        {
            _sourceTopPx += _stepTopPx;
            SetValue(Canvas.TopProperty, _sourceTopPx);

            _sourceWidthPx += _stepWidthPx;
            MainCanvas.Width = _sourceWidthPx;

            _sourcePoints += _stepPoints;
            PointsLabel.Content = Convert.ToInt32(_sourcePoints);
            
            _sourceRank += _stepRank;
            RankLabel.Content = SetRankLabel(Convert.ToInt32(_sourceRank));
        }

        private void SetInitialValues()
        {
            SetValue(Canvas.TopProperty, _sourceTopPx);
            MainCanvas.Width = _sourceWidthPx;
            PointsLabel.Content = _sourcePoints;
            RankLabel.Content = SetRankLabel(Convert.ToInt32(_sourceRank));
        }

        private string SetRankLabel(int rank)
        {
            return rank.ToString().PadLeft(2, '0');
        }

        private void SetTheoricalValues(Ranking item, bool isNew)
        {
            // start position on y axis:
            // for a new item, it's at the bottom (so after every items of the window)
            // for an existing item, it's the current position
            _sourceTopPx = isNew ? Ranking.DefaultPaginationLimit * Realheight : _targetTopPx;
            // position target on y axis
            _targetTopPx = (item.Rank - 1) * Realheight;
            // pixels to move, from source to target, at each step
            _stepTopPx = (_targetTopPx - _sourceTopPx) / StepsCount;

            // The same kind of stuff for the width
            // except the initial width is zero
            _sourceWidthPx = isNew ? 0 : _targetWidthPx;
            // the scale of width starts to "Ranking.MinPoints" and ends at "Ranking.MaxPoints"
            // with a scale of "PixelsByPoint"
            // if real points are below "Ranking.MinPoints", we use this value (minus "PointsMargin")
            // we also add "PlayerNamePixels" to display player's name
            // TODO: not great because "PointsMargin" should not be impacted by "PixelsByPoint"
            // it should be raw pixels (at the very least, the name is confusing)
            var pointsToConsider = item.Points > Ranking.MinPoints
                ? item.Points - (Ranking.MinPoints - PointsMargin)
                : PointsMargin;
            _targetWidthPx = PlayerNamePixels + ((pointsToConsider * PixelsByPoint) / Ranking.MaxPoints);
            _stepWidthPx = (_targetWidthPx - _sourceWidthPx) / StepsCount;

            // same stuff for points display management
            _sourcePoints = isNew ? 0 : _targetPoints;
            _targetPoints = item.Points;
            _stepPoints = (_targetPoints - _sourcePoints) / StepsCount;

            // same stuff for rank
            _sourceRank = isNew ? (Ranking.DefaultPaginationLimit + 1) : _targetRank;
            _targetRank = item.Rank;
            _stepRank = (_targetRank - _sourceRank) / StepsCount;
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

            public double SetCurrentStep()
            {
                _current += _step;
                return _current;
            }
        }
    }
}
