namespace RCi.PlainTextTable
{
    public readonly record struct Borders(Border Left, Border Top, Border Right, Border Bottom)
    {
        public Borders(Border Horizontal, Border vertical) :
            this(Horizontal, vertical, Horizontal, vertical)
        {
        }

        public Borders(Border uniform) :
            this(uniform, uniform, uniform, uniform)
        {
        }

        public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
    }
}
