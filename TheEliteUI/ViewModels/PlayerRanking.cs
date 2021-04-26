using System.Windows.Controls;
using TheEliteUI.Dtos;

namespace TheEliteUI.ViewModels
{
    public class PlayerRanking : IRanking
    {
        public PlayerRankingDto Dto { get; }

        public object Key => Dto.PlayerId;

        public string Label => Dto.PlayerName;

        public int Value => Dto.Points;

        public string HexColor => Dto.PlayerColor;

        public int Rank => Dto.Rank;

        public int Position => Rank + Dto.SubRank;

        public double ItemsCount => PlayerRankingDto.DefaultPaginationLimit;

        public double ValueMin => PlayerRankingDto.MinPoints;

        public double ValueMax => PlayerRankingDto.MaxPoints;

        internal PlayerRanking(PlayerRankingDto dto)
        {
            Dto = dto;
        }

        public bool IsKey(object otherKey)
        {
            return otherKey != null
                && double.TryParse(otherKey.ToString(), out double parsedKey)
                && parsedKey == Dto.PlayerId;
        }

        public ContentControl GetToolTip()
        {
            return null;
            //throw new System.NotImplementedException();
        }
    }
}
