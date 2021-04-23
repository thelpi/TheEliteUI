using TheEliteUI.Dtos;
using TheEliteUI.ViewModels;

namespace TheEliteUI
{
    /// <summary>
    /// Logique d'interaction pour WrRanking.xaml
    /// </summary>
    public partial class WrRankingControl : RankingControl
    {
        public WrRankingControl(WrRanking item, int steps, bool untiedMode)
            : base(item,
                  steps,
                  StandingWrDto.MinDays,
                  untiedMode ? StandingWrDto.MaxDaysUntied : StandingWrDto.MaxDaysTied,
                  StandingWrDto.DefaultPaginationLimit)
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
