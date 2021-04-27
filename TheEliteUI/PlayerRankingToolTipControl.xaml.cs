using System.Windows.Controls;
using TheEliteUI.Converters;
using TheEliteUI.Dtos;

namespace TheEliteUI
{
    /// <summary>
    /// Logique d'interaction pour WrRankingToolTipControl.xaml
    /// </summary>
    public partial class PlayerRankingToolTipControl : UserControl
    {
        // A big ugly but it will work fine
        private static readonly TimeToTextConverter _timeToTextConverter = new TimeToTextConverter();

        private const string NA = "N/A";

        public PlayerRankingToolTipControl(PlayerRankingDto dto)
        {
            InitializeComponent();
            LevelsView.ItemsSource = dto.Levels;
        }
    }
}
