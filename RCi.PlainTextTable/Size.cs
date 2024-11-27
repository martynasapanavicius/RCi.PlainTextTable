namespace RCi.Toolbox
{
    internal readonly record struct Size(int Width, int Height)
    {
        public static implicit operator Size((int Width, int Height) t) => new(t.Width, t.Height);
        public static implicit operator (int Width, int Height)(Size s) => new(s.Width, s.Height);
    }
}
