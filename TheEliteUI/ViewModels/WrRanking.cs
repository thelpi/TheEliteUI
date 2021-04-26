using System.Windows.Controls;
using TheEliteUI.Dtos;

namespace TheEliteUI.ViewModels
{
    public class WrRanking : IRanking
    {
        public StandingWrDto Dto { get; }

        public object Key => string.Concat(Dto.Stage, Dto.Level, Dto.Time);

        public string Label => string.Concat(Dto.Stage, " - ", Dto.Level, " - ", Dto.Time);

        public int Value => Dto.Days;

        public string HexColor => "ffffff";

        public int Rank => Dto.Rank;

        public int Position => Rank + Dto.SubRank;

        public double ItemsCount => StandingWrDto.DefaultPaginationLimit;

        public double ValueMin => StandingWrDto.MinDays;

        public double ValueMax => Untied ? StandingWrDto.MaxDaysUntied : StandingWrDto.MaxDaysTied;

        public bool Untied { get; }

        internal WrRanking(StandingWrDto dto, bool untied)
        {
            Dto = dto;
            Untied = untied;
        }

        public bool IsKey(object otherKey)
        {
            return otherKey?.ToString() == Key.ToString();
        }

        public ContentControl GetToolTip()
        {
            return new WrRankingToolTipControl
            {
                DataContext = this
            };
        }
    }
}
