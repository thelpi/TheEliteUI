using System;
using System.Windows;
using System.Windows.Controls;
using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    public class RankingControl : UserControl
    {
        public static double ControlHeight { get; } = 30;
        public static double ControlBorderThickness { get; } = 3;
        public static double Realheight => ControlHeight + (2 * ControlBorderThickness) + SpaceBetweenRanking;

        // minimal width of the ranking control (mostly for label)
        private const double MinimalWidth = 150;
        // don't use the full panel width
        // because the right-padding sets on parent container is ignored
        private const double ContainerAvailableWidthRate = 0.95;
        private const double SpaceBetweenRanking = 2;

        public IRanking Item { get; }

        private readonly SourceToTargetByStep _top;
        private readonly SourceToTargetByStep _width;
        private readonly SourceToTargetByStep _points;
        private readonly SourceToTargetByStep _rank;
        private readonly Func<double, object> _valueParser;
        private readonly FrameworkElement _container;

        protected Panel MainPanel { private get; set; }
        protected Label ValueLabel { private get; set; }
        protected Label RankLabel { private get; set; }

        public RankingControl()
        {
            // do not remove this controller
        }

        public RankingControl(IRanking item,
            int steps,
            Func<double, object> valueParser,
            FrameworkElement container)
        {
            Item = item;
            DataContext = item;

            _container = container;

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

        private double GetTargetWidth(IRanking item)
        {
            var availableWidth = (_container.ActualWidth * ContainerAvailableWidthRate) - MinimalWidth;

            var relativeValue = item.Value - item.ValueMin;
            relativeValue = relativeValue < 0 ? 0 : relativeValue;

            var widthRateOfvalue = relativeValue / (item.ValueMax - item.ValueMin);

            return MinimalWidth + (availableWidth * widthRateOfvalue);
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
