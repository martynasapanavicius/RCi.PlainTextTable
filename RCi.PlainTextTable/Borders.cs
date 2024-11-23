namespace RCi.PlainTextTable
{
    public readonly record struct Borders(Border Left, Border Top, Border Right, Border Bottom)
    {
        public Borders(Border universalHorizontal, Border universalVertical) :
            this(universalHorizontal, universalVertical, universalHorizontal, universalVertical)
        {
        }

        public Borders(Border universal) :
            this(universal, universal, universal, universal)
        {
        }

        public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
    }
}
