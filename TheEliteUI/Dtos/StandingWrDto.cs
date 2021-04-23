using System;

namespace TheEliteUI.Dtos
{
    public class StandingWrDto
    {
        public const int DefaultPaginationLimit = 10;
        public const int MinDays = 0;
        public const int MaxDaysUntied = 3000;
        public const int MaxDaysTied = 6000;

        public Level Level { get; set; }
        public Stage Stage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? StandingStartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Time { get; set; }
        public string StartPlayerName { get; set; }
        public string EndPlayerName { get; set; }
        public int Days { get; set; }
        public int StandingDays { get; set; }
        public bool StillWr { get; set; }
        public int Rank { get; set; }
    }
}
