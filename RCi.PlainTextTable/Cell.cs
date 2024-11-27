using System;

namespace RCi.Toolbox.Ptt
{
    public sealed class Cell
    {
        private readonly PlainTextTable _host;
        public PlainTextTable Host() => _host;

        public Coordinate Coordinate { get; }

        private string? _text; // is not alive if text is null
        public string Text
        {
            get => _text ??= string.Empty; // create on touch if needed
            set => _text = value;
        }

        internal bool IsAlive => _text is not null;

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
            _host = host;
            Coordinate = coordinate;
        }
    }
}
