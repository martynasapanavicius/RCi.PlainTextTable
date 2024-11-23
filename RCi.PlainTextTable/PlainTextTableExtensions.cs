namespace RCi.PlainTextTable
{
    public static class PlainTextTableExtensions
    {
        public static ICell Text(this ICell cell, string text)
        {
            cell.Text = text;
            return cell;
        }

        public static ICell ColumnSpan(this ICell cell, int span)
        {
            cell.ColumnSpan = span;
            return cell;
        }

        public static ICell RowSpan(this ICell cell, int span)
        {
            cell.RowSpan = span;
            return cell;
        }

        public static RowControl Row(this PlainTextTable ptt, int row) => new(ptt, row);

        public static ColumnControl Column(this PlainTextTable ptt, int col) => new(ptt, col);
    }
}
