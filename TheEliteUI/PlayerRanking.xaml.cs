using System.Windows.Controls;
using TheEliteUI.Models;

namespace TheEliteUI
{
    public partial class PlayerRanking : UserControl
    {
        public static double ControlHeight { get; } = 30;
        public static double ControlBorderThickness { get; } = 3;
        public static double Realheight => ControlHeight + (2 * ControlBorderThickness);

        private readonly int StepsCount;

        public long PlayerId { get; }

        private double _targetTopPx;
        private double _sourceTopPx;
        private double _stepTopPx;

        private double _targetWidthPx;
        private double _sourceWidthPx;
        private double _stepWidthPx;

        public PlayerRanking(Ranking item, int steps, int itemsCount)
        {
            InitializeComponent();
            PlayerId = item.PlayerId;
            DataContext = item;
            StepsCount = steps;
            SetTheoricalWidthAndTop(item, true, itemsCount);
            SetInitialActualWidthAndTop();
        }

        internal void Update(Ranking item, int itemsCount)
        {
            DataContext = item;
            SetTheoricalWidthAndTop(item, false, itemsCount);
        }

        internal void SetActualWidthAndTop()
        {
            var currentTop = (double)GetValue(Canvas.TopProperty);
            var newTop = currentTop + _stepTopPx;
            SetValue(Canvas.TopProperty, newTop);
            
            MainCanvas.Width = MainCanvas.Width + _stepWidthPx;
            MainPanel.Width = MainPanel.Width + _stepWidthPx;
        }

        private void SetInitialActualWidthAndTop()
        {
            SetValue(Canvas.TopProperty, _sourceTopPx);

            MainCanvas.Width = _sourceWidthPx;
            MainPanel.Width = _sourceWidthPx;
        }

        private void SetTheoricalWidthAndTop(Ranking item, bool isNew, int itemsCount)
        {
            // start position on y axis:
            // for a new item, it's at the bottom (so after every items of the window)
            // for an existing item, it's the current position
            _sourceTopPx = isNew ? itemsCount * Realheight : _targetTopPx;
            // position target on y axis
            _targetTopPx = (item.Rank - 1) * Realheight;
            // pixels to move, from source to target, at each step
            _stepTopPx = (_targetTopPx - _sourceTopPx) / StepsCount;

            // The same kind of stuff for the width
            // except the initial width is zero
            _sourceWidthPx = isNew ? 0 : _targetWidthPx;
            // TODO: explain formula
            _targetWidthPx = 150 + ((item.Points * 300) / (double)Ranking.MaxPoints);
            _stepWidthPx = (_targetWidthPx - _sourceWidthPx) / StepsCount;
        }
    }
}
