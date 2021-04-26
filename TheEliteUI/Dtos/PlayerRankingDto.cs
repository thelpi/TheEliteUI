using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TheEliteUI.Dtos
{
    public class PlayerRankingDto
    {
        public const int MaxPoints = 6000;
        public const int DefaultPaginationLimit = 20;
        // TODO: find a method to link the value with the pagination
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
        public int SubRank { get; set; }
        public IReadOnlyDictionary<Level, int> LevelPoints { get; set; }
        public IReadOnlyDictionary<Level, int> LevelUntiedRecordsCount { get; set; }
        public IReadOnlyDictionary<Level, int> LevelRecordsCount { get; set; }
        public IReadOnlyDictionary<Level, long> LevelCumuledTime { get; set; }
        public IReadOnlyDictionary<Stage, IReadOnlyDictionary<Level, StageLevelDetails>> Details { get; set; }

        public class StageLevelDetails
        {
            [JsonProperty("item1")]
            public int Rank { get; set; }
            [JsonProperty("item2")]
            public int Points { get; set; }
            [JsonProperty("item3")]
            public int Time { get; set; }
        }
    }
}
