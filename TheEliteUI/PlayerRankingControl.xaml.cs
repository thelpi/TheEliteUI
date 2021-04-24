using System.Windows;
using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    public partial class PlayerRankingControl : RankingControl
    {
        public PlayerRankingControl(PlayerRanking item, int steps, FrameworkElement container)
            : base(item, steps, v => System.Convert.ToInt32(v), container)
        {
            InitializeComponent();
            RankLabel = PlayerRankLabel;
            MainPanel = PlayerMainCanvas;
            ValueLabel = PointsLabel;
            ArrangeControl(false);
        }
    }
}
