using System;
using System.Linq;
using Coordinate = (int row, int col);
using Size = (int width, int height);

namespace RCi.PlainTextTable
{
    internal sealed record LogicalCell
    {
        public required Coordinate Coordinate { get; init; }
        public int Col => Coordinate.col;
        public int Row => Coordinate.row;
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
                Margin.Left + TextSize.width + Margin.Right,
                Margin.Top + TextSize.height + Margin.Bottom
            );
        }

        public override string ToString() => $"{Coordinate}, CS={ColumnSpan}, RS={RowSpan}, M=({Margin}), TS={TextSize}, TSWM={TextSizeWithMargin}, HA={HorizontalAlignment.ToString()[..1]}, VA={VerticalAlignment.ToString()[..1]}, B=({Borders})";
    }
}
