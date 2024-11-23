namespace RCi.PlainTextTable
{
    public static class PlainTextTableExtensions
    {
        public static Cell Text(this Cell cell, string text)
        {
            cell.Text = text;
            return cell;
        }

        public static Cell ColumnSpan(this Cell cell, int span)
        {
            cell.ColumnSpan = span;
            return cell;
        }

        public static Cell RowSpan(this Cell cell, int span)
        {
            cell.RowSpan = span;
            return cell;
        }
    }
}
