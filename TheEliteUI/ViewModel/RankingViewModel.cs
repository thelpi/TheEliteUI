namespace TheEliteUI.ViewModel
{
    public class RankingViewModel
    {
        public int Rank { get; }
        public string PlayerName { get; }
        public int Points { get; }
        public long CumuledTime { get; }
        public int UntiedRecordsCount { get; }
        public int RecordsCount { get; }
        public string PlayerColor { get; }

        internal RankingViewModel(Model.Ranking ranking, int rank)
        {
            Rank = rank;
            PlayerName = ranking.PlayerName;
            Points = ranking.Points;
            CumuledTime = ranking.CumuledTime;
            UntiedRecordsCount = ranking.UntiedRecordsCount;
            RecordsCount = ranking.RecordsCount;
            PlayerColor = ranking.PlayerColor;
        }
    }
}
