namespace TheEliteUI.ViewModels
{
    public interface IRanking
    {
        double ItemsCount { get; }
        double ValueMin { get; }
        double ValueMax { get; }
        object Key { get; }
        string Label { get; }
        int Value { get; }
        string HexColor { get; }
        int Rank { get; }
        int Position { get; }

        bool IsKey(object otherKey);
    }
}
