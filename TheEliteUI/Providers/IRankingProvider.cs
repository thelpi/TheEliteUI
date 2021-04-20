using System;
using System.Collections.Generic;
using TheEliteUI.Models;

namespace TheEliteUI.Providers
{
    public interface IRankingProvider
    {
        IReadOnlyCollection<Ranking> GetRanking(Game game, DateTime date, int page, int limit);
    }
}
