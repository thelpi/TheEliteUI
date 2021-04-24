using System;
using System.Windows.Controls;
using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    public class RankingControl : UserControl
    {
        public static double ControlHeight { get; } = 30;
        public static double ControlBorderThickness { get; } = 3;
        public static double Realheight => ControlHeight + (2 * ControlBorderThickness);

        private const double LabelPixels = 150;
        private const double PixelsByValue = 800;
        private const double ValueMargin = 100;

        public IRanking Item { get; }

        private readonly SourceToTargetByStep _top;
        private readonly SourceToTargetByStep _width;
        private readonly SourceToTargetByStep _points;
        private readonly SourceToTargetByStep _rank;
 
        protected Panel MainPanel { private get; set; }
        protected Label ValueLabel { private get; set; }
        protected Label RankLabel { private get; set; }

        private readonly Func<double, object> _valueParser;

        public RankingControl()
        {
            // do not remove this controller
        }

        public RankingControl(IRanking item,
            int steps,
            Func<double, object> valueParser)
        {
            Item = item;
            DataContext = item;

            _top = new SourceToTargetByStep(GetTargetTop(item), steps, item.ItemsCount * Realheight);
            _width = new SourceToTargetByStep(GetTargetWidth(item), steps, 0);
            _points = new SourceToTargetByStep(item.Value, steps, 0);
            _rank = new SourceToTargetByStep(item.Rank, steps, item.ItemsCount + 1);
            _valueParser = valueParser;
        }

        internal void UpdateItemtarget(IRanking item)
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

        protected void ArrangeControl(bool step)
        {
            SetValue(Canvas.TopProperty, _top.SetCurrentStep(step));
            MainPanel.Width = _width.SetCurrentStep(step);
            ValueLabel.Content = _valueParser(_points.SetCurrentStep(step));
            RankLabel.Content = Convert.ToInt32(_rank.SetCurrentStep(step)).ToString().PadLeft(2, '0');
        }

        private static double GetTargetTop(IRanking item)
        {
            return (item.Position - 1) * Realheight;
        }

        private static double GetTargetWidth(IRanking item)
        {
            // TODO: "ValueMargin" should be in pixels unit
            var valueToConsider = item.Value > item.ValueMin
                ? item.Value - (item.ValueMin - ValueMargin)
                : ValueMargin;
            return LabelPixels + ((valueToConsider * PixelsByValue) / item.ValueMax);
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
