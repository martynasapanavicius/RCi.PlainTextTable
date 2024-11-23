namespace RCi.PlainTextTable
{
    public readonly record struct Size(int Width, int Height)
    {
        public static implicit operator Size((int, int) t) => new(t.Item1, t.Item2);
    }
}
