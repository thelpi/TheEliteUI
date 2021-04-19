namespace TheEliteUI.Model
{
    public class Ranking
    {
        public const int MaxPoints = 6000;

        public Game Game { get; set; }
        public long PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Points { get; set; }
        public long CumuledTime { get; set; }
        public int UntiedRecordsCount { get; set; }
        public int RecordsCount { get; set; }
        public string PlayerColor { get; set; }
        public int Rank { get; private set; }

        public Ranking WithRank(int rank)
        {
            Rank = rank;
            return this;
        }
    }
}
