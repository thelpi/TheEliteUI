using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    public partial class WrRankingControl : RankingControl
    {
        public WrRankingControl(WrRanking item, int steps)
            : base(item, steps, v => System.Convert.ToInt32(v))
        {
            InitializeComponent();
            RankLabel = WrRankLabel;
            MainPanel = WrMainPanel;
            ValueLabel = DaysLabel;
            ArrangeControl(false);
        }
    }
}
