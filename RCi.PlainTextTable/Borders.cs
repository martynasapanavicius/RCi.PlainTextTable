namespace RCi.PlainTextTable
{
    public readonly record struct Borders(Border Left, Border Top, Border Right, Border Bottom)
    {
        public Borders(Border horizontal, Border vertical) :
            this(horizontal, vertical, horizontal, vertical)
        {
        }

        public Borders(Border uniform) :
            this(uniform, uniform, uniform, uniform)
        {
        }

        public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
    }
}
