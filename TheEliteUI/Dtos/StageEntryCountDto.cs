using System;

namespace TheEliteUI.Dtos
{
    public class StageEntryCountDto
    {
        public Level? Level { get; set; }
        public Stage Stage { get; set; }
        public int PeriodEntriesCount { get; set; }
        public int TotalEntriesCount { get; set; }
        public int AllStagesEntriesCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
