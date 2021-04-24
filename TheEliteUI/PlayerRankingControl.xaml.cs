using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    public partial class PlayerRankingControl : RankingControl
    {
        public PlayerRankingControl(PlayerRanking item, int steps)
            : base(item, steps, v => System.Convert.ToInt32(v))
        {
            InitializeComponent();
            RankLabel = PlayerRankLabel;
            MainPanel = PlayerMainCanvas;
            ValueLabel = PointsLabel;
            ArrangeControl(false);
        }
    }
}
