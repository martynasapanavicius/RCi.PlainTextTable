using System;

namespace RCi.PlainTextTable
{
    public sealed class Cell
    {
        public PlainTextTable Host { get; }
        public Coordinate Coordinate { get; }
        public string Text { get; set; } = string.Empty;

        private int _columnSpan = 1;
        public int ColumnSpan
        {
            get => _columnSpan;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 1, nameof(ColumnSpan));
                _columnSpan = value;
            }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get => _rowSpan;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 1, nameof(RowSpan));
                _rowSpan = value;
            }
        }

        public Margin? Margin { get; set; }
        public Borders? Borders { get; set; }
        public HorizontalAlignment? HorizontalAlignment { get; set; }
        public VerticalAlignment? VerticalAlignment { get; set; }

        internal Cell(PlainTextTable host, Coordinate coordinate)
        {
            Host = host;
            Coordinate = coordinate;
        }

        public void Delete() => Host.DeleteCell(this);
        public RowControl Row() => new(Host, Coordinate.Row);
        public ColumnControl Column() => new(Host, Coordinate.Col);
        public Cell MoveLeft() => Host[Coordinate.MoveLeft()];
        public Cell MoveUp() => Host[Coordinate.MoveUp()];
        public Cell MoveRight() => Host[Coordinate.MoveRight()];
        public Cell MoveDown() => Host[Coordinate.MoveDown()];
    }
}
