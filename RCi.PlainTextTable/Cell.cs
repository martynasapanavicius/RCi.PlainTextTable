namespace RCi.PlainTextTable
{
    public sealed class Cell
    {
        public PlainTextTable Host { get; }
        public Coordinate Coordinate { get; }
        public string Text { get; set; } = string.Empty;
        public int ColumnSpan { get; set; } = 1; // TODO: validate or clamp values on set
        public int RowSpan { get; set; } = 1; // TODO: validate or clamp values on set
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
    }
}
