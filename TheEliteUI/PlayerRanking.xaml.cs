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

        private int _targetPoints;
        private int _sourcePoints;
        private double _stepPoints;
        private int _currentPoints;

        public PlayerRanking(Ranking item, int steps)
        {
            InitializeComponent();
            PlayerId = item.PlayerId;
            DataContext = item;
            StepsCount = steps;
            SetTheoricalWidthAndTop(item, true);
            SetInitialActualWidthAndTop();
        }

        internal void Update(Ranking item)
        {
            DataContext = item;
            SetTheoricalWidthAndTop(item, false);
        }

        internal void SetActualWidthAndTop()
        {
            var currentTop = (double)GetValue(Canvas.TopProperty);
            var newTop = currentTop + _stepTopPx;
            SetValue(Canvas.TopProperty, newTop);
            
            MainCanvas.Width = MainCanvas.Width + _stepWidthPx;
            MainPanel.Width = MainPanel.Width + _stepWidthPx;

            _currentPoints = System.Convert.ToInt32(_currentPoints + _stepPoints);
            PointsLabel.Content = _currentPoints;
        }

        private void SetInitialActualWidthAndTop()
        {
            SetValue(Canvas.TopProperty, _sourceTopPx);

            MainCanvas.Width = _sourceWidthPx;
            MainPanel.Width = _sourceWidthPx;

            PointsLabel.Content = _sourcePoints;
        }

        private void SetTheoricalWidthAndTop(Ranking item, bool isNew)
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
            // but we also stores the current point value, to avoid getting it from the label content
            _sourcePoints = isNew ? 0 : _targetPoints;
            _currentPoints = _sourcePoints;
            _targetPoints = item.Points;
            _stepPoints = (_targetPoints - _sourcePoints) / StepsCount;
        }
    }
}
