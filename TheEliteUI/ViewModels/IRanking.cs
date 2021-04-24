namespace TheEliteUI.ViewModels
{
    public interface IRanking
    {
        object Key { get; }
        string Label { get; }
        int Value { get; }
        string HexColor { get; }
        int Rank { get; }
        int Position { get; }

        bool IsKey(object otherKey);
    }
}
