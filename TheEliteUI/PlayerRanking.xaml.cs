using System.Windows.Controls;
using TheEliteUI.Model;

namespace TheEliteUI
{
    /// <summary>
    /// Logique d'interaction pour PlayerRanking.xaml
    /// </summary>
    public partial class PlayerRanking : UserControl
    {
        public static double ControlHeight { get; } = 30;
        public static double ControlBorderThickness { get; } = 3;

        public PlayerRanking()
        {
            InitializeComponent();
        }

        public PlayerRanking(Ranking item)
        {
            InitializeComponent();
            DataContext = item;
            SetValue(Canvas.TopProperty, (item.Rank - 1) * (ControlHeight + (2 * ControlBorderThickness)));
        }
    }
}
