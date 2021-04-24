using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    /// <summary>
    /// Logique d'interaction pour WrRanking.xaml
    /// </summary>
    public partial class WrRankingControl : RankingControl
    {
        public WrRankingControl(WrRanking item, int steps)
            : base(item, steps)
        {
            InitializeComponent();
            RankLabel = WrRankLabel;
            MainPanel = WrMainPanel;
            ValueLabel = DaysLabel;
            ValueParser = v => System.Convert.ToInt32(v);
            ArrangeControl(false);
        }
    }
}
