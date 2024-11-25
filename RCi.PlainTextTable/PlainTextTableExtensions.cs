namespace RCi.PlainTextTable
{
    public static class PlainTextTableExtensions
    {
        // modify

        public static void Delete(this Cell cell) => cell.Host().DeleteCell(cell);

        public static Cell SetText(this Cell cell, string text)
        {
            cell.Text = text;
            return cell;
        }

        public static Cell SetText(this Cell cell, object text) =>
            cell.SetText(text.ToString() ?? string.Empty);

        public static Cell SetColumnSpan(this Cell cell, int span)
        {
            cell.ColumnSpan = span;
            return cell;
        }

        public static Cell SetRowSpan(this Cell cell, int span)
        {
            cell.RowSpan = span;
            return cell;
        }

        public static Cell SetMargin(this Cell cell, Margin? margin)
        {
            cell.Margin = margin;
            return cell;
        }

        public static Cell SetBorders(this Cell cell, Borders? borders)
        {
            cell.Borders = borders;
            return cell;
        }

        public static Cell SetBorders(this Cell cell, Border left, Border top, Border right, Border bottom)
        {
            cell.Borders = new Borders(left, top, right, bottom);
            return cell;
        }

        public static Cell SetBorders(this Cell cell, Border horizontal, Border vertical)
        {
            cell.Borders = new Borders(horizontal, vertical);
            return cell;
        }

        public static Cell SetBorders(this Cell cell, Border uniform)
        {
            cell.Borders = new Borders(uniform);
            return cell;
        }

        public static Cell SetLeftBorder(this Cell cell, Border border)
        {
            cell.Borders = (cell.Borders ?? cell.Host().DefaultBorders) with
            {
                Left = border,
            };
            return cell;
        }

        public static Cell SetTopBorder(this Cell cell, Border border)
        {
            cell.Borders = (cell.Borders ?? cell.Host().DefaultBorders) with
            {
                Top = border,
            };
            return cell;
        }

        public static Cell SetRightBorder(this Cell cell, Border border)
        {
            cell.Borders = (cell.Borders ?? cell.Host().DefaultBorders) with
            {
                Right = border,
            };
            return cell;
        }

        public static Cell SetBottomBorder(this Cell cell, Border border)
        {
            cell.Borders = (cell.Borders ?? cell.Host().DefaultBorders) with
            {
                Bottom = border,
            };
            return cell;
        }

        public static Cell SetHorizontalAlignment(this Cell cell, HorizontalAlignment? horizontalAlignment)
        {
            cell.HorizontalAlignment = horizontalAlignment;
            return cell;
        }

        public static Cell SetVerticalAlignment(this Cell cell, VerticalAlignment? verticalAlignment)
        {
            cell.VerticalAlignment = verticalAlignment;
            return cell;
        }

        // movement

        public static Row Row(this Cell cell) => new(cell.Host(), cell.Coordinate.Row);
        public static Column Column(this Cell cell) => new(cell.Host(), cell.Coordinate.Col);

        public static Cell MoveLeft(this Cell cell) => cell.Host()[cell.Coordinate.MoveLeft()];
        public static Cell MoveUp(this Cell cell) => cell.Host()[cell.Coordinate.MoveUp()];
        public static Cell MoveRight(this Cell cell) => cell.Host()[cell.Coordinate.MoveRight()];
        public static Cell MoveDown(this Cell cell) => cell.Host()[cell.Coordinate.MoveDown()];

        public static RowSpan TakeLeft(this Cell cell, int count) => cell.Row().Slice(cell.Coordinate.Col - count + 1, count);
        public static RowSpan TakeRight(this Cell cell, int count) => cell.Row().Slice(cell.Coordinate.Col, count);
        public static ColumnSpan TakeUp(this Cell cell, int count) => cell.Column().Slice(cell.Coordinate.Row - count + 1, count);
        public static ColumnSpan TakeDown(this Cell cell, int count) => cell.Column().Slice(cell.Coordinate.Row, count);
    }
}
