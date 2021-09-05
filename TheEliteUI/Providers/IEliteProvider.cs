using System;
using System.Collections.Generic;
using TheEliteUI.Dtos;

namespace TheEliteUI.Providers
{
    public interface IEliteProvider
    {
        IReadOnlyCollection<PlayerRankingDto> GetRanking(Game game, DateTime date, int page, int limit);

        IReadOnlyCollection<StandingWrDto> GetStandingWr(Game game, DateTime atDate, bool untied, int page, int limit);

        IReadOnlyCollection<StageEntryCountDto> GetStagesEntriesCount(Game game, DateTime startDate, DateTime endDate, DateTime globalStartDate, DateTime globalEndDate, bool levelDetails);
    }
}
