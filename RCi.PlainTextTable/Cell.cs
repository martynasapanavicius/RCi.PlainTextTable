namespace RCi.PlainTextTable
{
    public interface ICell
    {
        Coordinate Coordinate { get; }
        string Text { get; set; }
        int ColumnSpan { get; set; }
        int RowSpan { get; set; }
        Margin? Margin { get; set; }
        Borders? Borders { get; set; }
        HorizontalAlignment? HorizontalAlignment { get; set; }
        VerticalAlignment? VerticalAlignment { get; set; }

        PlainTextTable Host();
        void Delete();
    }

    internal sealed class Cell :
        ICell
    {
        public required PlainTextTable Host { get; init; }
        public required Coordinate Coordinate { get; init; }
        public string Text { get; set; } = string.Empty;
        public int ColumnSpan { get; set; } = 1;
        public int RowSpan { get; set; } = 1;
        public Margin? Margin { get; set; }
        public Borders? Borders { get; set; }
        public HorizontalAlignment? HorizontalAlignment { get; set; }
        public VerticalAlignment? VerticalAlignment { get; set; }

        PlainTextTable ICell.Host() => Host;
        public void Delete() => Host.DeleteCell(this);
    }

}
