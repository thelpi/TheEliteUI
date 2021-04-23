using System;
using System.Collections.Generic;
using TheEliteUI.Dtos;

namespace TheEliteUI.Providers
{
    public interface IRankingProvider
    {
        IReadOnlyCollection<PlayerRankingDto> GetRanking(Game game, DateTime date, int page, int limit);
    }
}
