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
            DataContext = dto;
            // TODO: sets real binding instead of code-behind
            SetLevelLabels(dto, Level.Easy);
            SetLevelLabels(dto, Level.Medium);
            SetLevelLabels(dto, Level.Hard);
        }

        private void SetLevelLabels(PlayerRankingDto dto, Level level)
        {
            (FindName($"{level}Label") as Label).Content = level;
            (FindName($"{level}PointsLabel") as Label).Content =
                dto.LevelPoints.ContainsKey(level)
                    ? dto.LevelPoints[level].ToString()
                    : NA;
            (FindName($"{level}TimeLabel") as Label).Content =
                dto.LevelCumuledTime.ContainsKey(level)
                    ? _timeToTextConverter.InnerConvert(dto.LevelCumuledTime[level]).ToString()
                    : NA;
            (FindName($"{level}WrLabel") as Label).Content =
                dto.LevelRecordsCount.ContainsKey(level)
                    ? string.Concat(
                        dto.LevelRecordsCount[level],
                        " (",
                        (dto.LevelUntiedRecordsCount.ContainsKey(level)
                            ? dto.LevelUntiedRecordsCount[level].ToString()
                            : NA),
                        ")")
                    : NA;
        }
    }
}
