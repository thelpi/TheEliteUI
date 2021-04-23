﻿using TheEliteUI.Dtos;

namespace TheEliteUI.ViewModels
{
    public class PlayerRanking : IRanking
    {
        private PlayerRankingDto _dto;

        public object Key => _dto.PlayerId;

        public string Label => _dto.PlayerName;

        public int Value => _dto.Points;

        public string HexColor => _dto.PlayerColor;

        public int Rank => _dto.Rank;

        public bool IsKey(object otherKey)
        {
            return otherKey != null
                && double.TryParse(otherKey.ToString(), out double parsedKey)
                && parsedKey == _dto.PlayerId;
        }

        internal PlayerRanking(PlayerRankingDto dto)
        {
            _dto = dto;
        }
    }
}