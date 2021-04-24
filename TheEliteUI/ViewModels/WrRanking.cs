using TheEliteUI.Dtos;

namespace TheEliteUI.ViewModels
{
    public class WrRanking : IRanking
    {
        private readonly StandingWrDto _dto;

        public object Key => string.Concat(_dto.Stage, _dto.Level, _dto.Time);

        public string Label => string.Concat(_dto.Stage, " - ", _dto.Level, " - ", _dto.Time);

        public int Value => _dto.Days;

        public string HexColor => "ffffff";

        public int Rank => _dto.Rank;

        public int Position => Rank + _dto.SubRank;

        public double ItemsCount => StandingWrDto.DefaultPaginationLimit;

        public double ValueMin => StandingWrDto.MinDays;

        public double ValueMax => Untied ? StandingWrDto.MaxDaysUntied : StandingWrDto.MaxDaysTied;

        public bool Untied { get; }

        public bool IsKey(object otherKey)
        {
            return otherKey?.ToString() == Key.ToString();
        }

        internal WrRanking(StandingWrDto dto, bool untied)
        {
            _dto = dto;
            Untied = untied;
        }
    }
}
