using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    public partial class PlayerRankingControl : RankingControl
    {
        public PlayerRankingControl(PlayerRanking item, int steps)
            : base(item, steps)
        {
            InitializeComponent();
            RankLabel = PlayerRankLabel;
            MainPanel = PlayerMainCanvas;
            ValueLabel = PointsLabel;
            ValueParser = v => System.Convert.ToInt32(v);
            ArrangeControl(false);
        }
    }
}
