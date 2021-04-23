using System;
using System.Collections.Generic;

namespace TheEliteUI.Dtos
{
    public class RankingDto
    {
        public const int MaxPoints = 6000;
        public const int DefaultPaginationLimit = 25;
        // note: this value is connected to pagination
        public const int MinPoints = 4000;

        public static readonly IReadOnlyDictionary<Game, DateTime> RankingStart = new Dictionary<Game, DateTime>
        {
            { Game.GoldenEye, new DateTime(1998, 07, 26) },
            { Game.PerfectDark, new DateTime(2000, 01, 01) }
        };

        public Game Game { get; set; }
        public long PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Points { get; set; }
        public long CumuledTime { get; set; }
        public int UntiedRecordsCount { get; set; }
        public int RecordsCount { get; set; }
        public string PlayerColor { get; set; }
        public int Rank { get; set; }
    }
}
