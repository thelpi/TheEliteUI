using System.Windows.Controls;
using TheEliteUI.Dtos;

namespace TheEliteUI.ViewModels
{
    public class PlayerRanking : IRanking
    {
        private readonly PlayerRankingDto _dto;

        public object Key => _dto.PlayerId;

        public string Label => _dto.PlayerName;

        public int Value => _dto.Points;

        public string HexColor => _dto.PlayerColor;

        public int Rank => _dto.Rank;

        public int Position => Rank + _dto.SubRank;

        public double ItemsCount => PlayerRankingDto.DefaultPaginationLimit;

        public double ValueMin => PlayerRankingDto.MinPoints;

        public double ValueMax => PlayerRankingDto.MaxPoints;

        internal PlayerRanking(PlayerRankingDto dto)
        {
            _dto = dto;
        }

        public bool IsKey(object otherKey)
        {
            return otherKey != null
                && double.TryParse(otherKey.ToString(), out double parsedKey)
                && parsedKey == _dto.PlayerId;
        }

        public ContentControl GetToolTip()
        {
            return new PlayerRankingToolTipControl(_dto);
        }
    }
}
