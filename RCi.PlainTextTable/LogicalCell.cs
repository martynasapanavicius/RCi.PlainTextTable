using System;
using System.Linq;

namespace RCi.Toolbox
{
    internal sealed record LogicalCell
    {
        public required Coordinate Coordinate { get; init; }
        public int Col => Coordinate.Col;
        public int Row => Coordinate.Row;
        public required string Text { get; init; }
        public required int ColumnSpan { get; init; }
        public required int RowSpan { get; init; }
        public required Margin Margin { get; init; }
        public required Borders Borders { get; init; }
        public required HorizontalAlignment HorizontalAlignment { get; init; }
        public required VerticalAlignment VerticalAlignment { get; init; }

        private Size? _textSize;
        public Size TextSize => _textSize ??= GetTextSize();

        private Size? _textSizeWithMargin;
        public Size TextSizeWithMargin => _textSizeWithMargin ??= GetTextSizeWithMargin();

        private Size GetTextSize()
        {
            var lines = Text.Split(Environment.NewLine);
            var width = lines.Length == 0 ? 0 : lines.Max(l => l.Length);
            var height = lines.Length;
            return (width, height);
        }

        private Size GetTextSizeWithMargin()
        {
            return
            (
                Margin.Left + TextSize.Width + Margin.Right,
                Margin.Top + TextSize.Height + Margin.Bottom
            );
        }

        public override string ToString() =>
            $"{Coordinate}, " +
            $"CS={ColumnSpan}, " +
            $"RS={RowSpan}, " +
            $"M=({Margin}), " +
            $"TS={TextSize}, " +
            $"TSWM={TextSizeWithMargin}, " +
            $"HA={HorizontalAlignment.ToString()[..1]}, " +
            $"VA={VerticalAlignment.ToString()[..1]}, " +
            $"B=({Borders})";
    }
}
