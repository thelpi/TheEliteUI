using System.Windows;
using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    public partial class WrRankingControl : RankingControl
    {
        public WrRankingControl(WrRanking item, int steps, FrameworkElement container)
            : base(item, steps, v => System.Convert.ToInt32(v), container)
        {
            InitializeComponent();
            RankLabel = WrRankLabel;
            MainPanel = WrMainPanel;
            ValueLabel = DaysLabel;
            ArrangeControl(false);
        }
    }
}
