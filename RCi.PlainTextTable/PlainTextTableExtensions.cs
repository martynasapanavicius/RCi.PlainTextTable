namespace RCi.Toolbox.Ptt
{
    public static class PlainTextTableExtensions
    {
        // modify

        public static PlainTextTable Delete(this Cell cell)
        {
            var host = cell.Host();
            host.DeleteCell(cell);
            return host;
        }

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
        public static Cell SetBorders(this Cell cell, Border left, Border top, Border right, Border bottom) =>
            cell.SetBorders(new Borders(left, top, right, bottom));
        public static Cell SetBorders(this Cell cell, Border horizontal, Border vertical) =>
            cell.SetBorders(new Borders(horizontal, vertical));
        public static Cell SetBorders(this Cell cell, Border uniform) =>
            cell.SetBorders(new Borders(uniform));
        public static Cell SetLeftBorder(this Cell cell, Border border) =>
            cell.SetBorders((cell.Borders ?? cell.Host().DefaultBorders) with { Left = border });
        public static Cell SetTopBorder(this Cell cell, Border border) =>
            cell.SetBorders((cell.Borders ?? cell.Host().DefaultBorders) with { Top = border });
        public static Cell SetRightBorder(this Cell cell, Border border) =>
            cell.SetBorders((cell.Borders ?? cell.Host().DefaultBorders) with { Right = border });
        public static Cell SetBottomBorder(this Cell cell, Border border) =>
            cell.SetBorders((cell.Borders ?? cell.Host().DefaultBorders) with { Bottom = border });

        public static Cell SetHorizontalAlignment(this Cell cell, HorizontalAlignment? horizontalAlignment)
        {
            cell.HorizontalAlignment = horizontalAlignment;
            return cell;
        }
        public static Cell SetLeftHorizontalAlignment(this Cell cell) => cell.SetHorizontalAlignment(HorizontalAlignment.Left);
        public static Cell SetCenterHorizontalAlignment(this Cell cell) => cell.SetHorizontalAlignment(HorizontalAlignment.Center);
        public static Cell SetRightHorizontalAlignment(this Cell cell) => cell.SetHorizontalAlignment(HorizontalAlignment.Right);

        public static Cell SetVerticalAlignment(this Cell cell, VerticalAlignment? verticalAlignment)
        {
            cell.VerticalAlignment = verticalAlignment;
            return cell;
        }
        public static Cell SetTopVerticalAlignment(this Cell cell) => cell.SetVerticalAlignment(VerticalAlignment.Top);
        public static Cell SetCenterVerticalAlignment(this Cell cell) => cell.SetVerticalAlignment(VerticalAlignment.Center);
        public static Cell SetBottomVerticalAlignment(this Cell cell) => cell.SetVerticalAlignment(VerticalAlignment.Bottom);

        // movement

        public static Row Row(this Cell cell) => new(cell.Host(), cell.Coordinate.Row);
        public static Column Column(this Cell cell) => new(cell.Host(), cell.Coordinate.Col);

        public static Cell MoveLeft(this Cell cell) => cell.Host()[cell.Coordinate.MoveLeft()];
        public static Cell MoveUp(this Cell cell) => cell.Host()[cell.Coordinate.MoveUp()];
        public static Cell MoveRight(this Cell cell) => cell.Host()[cell.Coordinate.MoveRight()];
        public static Cell MoveDown(this Cell cell) => cell.Host()[cell.Coordinate.MoveDown()];
    }
}
