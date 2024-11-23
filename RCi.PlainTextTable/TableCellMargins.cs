namespace RCi.PlainTextTable
{
    internal sealed record TableCellMargins
    {
        public required Margin Area { get; init; }

        // borders
        public required Margin LeftBorder { get; init; }
        public required Margin TopBorder { get; init; }
        public required Margin RightBorder { get; init; }
        public required Margin BottomBorder { get; init; }

        // corners
        public required Margin TopLeftBorderCorner { get; init; }
        public required Margin TopRightBorderCorner { get; init; }
        public required Margin BottomRightBorderCorner { get; init; }
        public required Margin BottomLeftBorderCorner { get; init; }
    }
}
