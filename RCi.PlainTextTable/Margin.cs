namespace RCi.PlainTextTable
{
    public readonly record struct Margin(int Left, int Top, int Right, int Bottom) // TODO: validate or clamp values on set
    {
        public Margin(int horizontal, int vertical) :
            this(horizontal, vertical, horizontal, vertical)
        {
        }

        public Margin(int universal) :
            this(universal, universal, universal, universal)
        {
        }

        public int Width => Right - Left;
        public int Height => Bottom - Top;

        public static implicit operator Margin(int universal) =>
            new(universal);

        public static implicit operator Margin((int horizontal, int vertical) tuple) =>
            new(tuple.horizontal, tuple.vertical);

        public static implicit operator Margin((int left, int top, int right, int bottom) tuple) =>
            new(tuple.left, tuple.top, tuple.right, tuple.bottom);

        public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
    }
}
