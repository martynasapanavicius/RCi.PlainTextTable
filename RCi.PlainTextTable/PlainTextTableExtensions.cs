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

        public static Cell Borders(this Cell cell, Borders borders)
        {
            cell.Borders = borders;
            return cell;
        }

        public static Cell Borders(this Cell cell, Border left, Border top, Border right, Border bottom)
        {
            cell.Borders = new Borders(left, top, right, bottom);
            return cell;
        }

        public static Cell Borders(this Cell cell, Border horizontal, Border vertical)
        {
            cell.Borders = new Borders(horizontal, vertical);
            return cell;
        }

        public static Cell Borders(this Cell cell, Border uniform)
        {
            cell.Borders = new Borders(uniform);
            return cell;
        }

        public static Cell LeftBorder(this Cell cell, Border border)
        {
            cell.Borders = (cell.Borders ?? cell.Host.DefaultBorders) with
            {
                Left = border,
            };
            return cell;
        }

        public static Cell TopBorder(this Cell cell, Border border)
        {
            cell.Borders = (cell.Borders ?? cell.Host.DefaultBorders) with
            {
                Top = border,
            };
            return cell;
        }

        public static Cell RightBorder(this Cell cell, Border border)
        {
            cell.Borders = (cell.Borders ?? cell.Host.DefaultBorders) with
            {
                Right = border,
            };
            return cell;
        }

        public static Cell BottomBorder(this Cell cell, Border border)
        {
            cell.Borders = (cell.Borders ?? cell.Host.DefaultBorders) with
            {
                Bottom = border,
            };
            return cell;
        }
    }
}
