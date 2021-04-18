namespace TheEliteUI.Model
{
    public class Ranking
    {
        public Game Game { get; set; }
        public long PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Points { get; set; }
        public long CumuledTime { get; set; }
        public int UntiedRecordsCount { get; set; }
        public int RecordsCount { get; set; }
        public string PlayerColor { get; set; }
    }
}
