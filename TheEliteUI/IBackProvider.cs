using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheEliteUI.Model;

namespace TheEliteUI
{
    public interface IBackProvider
    {
        Task<IReadOnlyCollection<Ranking>> GetRankingAsync(Game game, DateTime date);
    }
}
