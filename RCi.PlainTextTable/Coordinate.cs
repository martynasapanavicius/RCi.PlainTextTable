namespace RCi.PlainTextTable
{
    public readonly record struct Coordinate(int Row, int Col)
    {
        public static implicit operator Coordinate((int, int) t) => new(t.Item1, t.Item2);
    }
}
