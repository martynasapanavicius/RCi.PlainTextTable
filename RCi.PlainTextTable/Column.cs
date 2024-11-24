namespace RCi.PlainTextTable
{
    public readonly struct Column(PlainTextTable host, int col)
    {
        public Cell this[int row] => host[row, col];
        public Cell Row(int row) => host[row, col];
        public PlainTextTable Host() => host;

        public PlainTextTable Delete()
        {
            var rowCount = host.RowCount;
            for (var i = rowCount - 1; i >= 0; i--)
            {
                host[i, col].Delete();
            }
            return host;
        }

        public Column Text(params string[] texts)
        {
            for (var i = 0; i < texts.Length; i++)
            {
                host[i, col].Text(texts[i]);
            }
            return this;
        }

        public Column Text(params object[] texts)
        {
            for (var i = 0; i < texts.Length; i++)
            {
                host[i, col].Text(texts[i]);
            }
            return this;
        }

        public Column Text(string text)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].Text(text);
            }
            return this;
        }

        public Column Text(object text)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].Text(text);
            }
            return this;
        }

        public Column RowSpan(params int[] rowSpans)
        {
            for (var i = 0; i < rowSpans.Length; i++)
            {
                host[i, col].RowSpan(rowSpans[i]);
            }
            return this;
        }

        public Column RowSpan(int rowSpan)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].RowSpan(rowSpan);
            }
            return this;
        }

        public Column ColumnSpan(params int[] columnSpans)
        {
            for (var i = 0; i < columnSpans.Length; i++)
            {
                host[i, col].ColumnSpan(columnSpans[i]);
            }
            return this;
        }

        public Column ColumnSpan(int columnSpan)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].ColumnSpan(columnSpan);
            }
            return this;
        }

        public Column RowAndColumnSpan(params (int RowSpan, int ColumnSpan)[] rowAndColumnSpans)
        {
            for (var i = 0; i < rowAndColumnSpans.Length; i++)
            {
                host[i, col].RowSpan(rowAndColumnSpans[i].RowSpan);
                host[i, col].ColumnSpan(rowAndColumnSpans[i].ColumnSpan);
            }
            return this;
        }

        public Column RowAndColumnSpan((int RowSpan, int ColumnSpan) rowAndColumnSpan)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].RowSpan(rowAndColumnSpan.RowSpan);
                host[i, col].ColumnSpan(rowAndColumnSpan.ColumnSpan);
            }
            return this;
        }

        public Column Margin(params Margin[] margins)
        {
            for (var i = 0; i < margins.Length; i++)
            {
                host[i, col].Margin(margins[i]);
            }
            return this;
        }

        public Column Margin(Margin margin)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].Margin(margin);
            }
            return this;
        }

        public Column Borders(params Borders[] borders)
        {
            for (var i = 0; i < borders.Length; i++)
            {
                host[i, col].Borders(borders[i]);
            }
            return this;
        }

        public Column Borders(Borders borders)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].Borders(borders);
            }
            return this;
        }

        public Column HorizontalAlignment(params HorizontalAlignment[] horizontalAlignments)
        {
            for (var i = 0; i < horizontalAlignments.Length; i++)
            {
                host[i, col].HorizontalAlignment(horizontalAlignments[i]);
            }
            return this;
        }

        public Column HorizontalAlignment(HorizontalAlignment horizontalAlignment)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].HorizontalAlignment(horizontalAlignment);
            }
            return this;
        }

        public Column VerticalAlignment(params VerticalAlignment[] verticalAlignment)
        {
            for (var i = 0; i < verticalAlignment.Length; i++)
            {
                host[i, col].VerticalAlignment(verticalAlignment[i]);
            }
            return this;
        }

        public Column VerticalAlignment(VerticalAlignment verticalAlignment)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].VerticalAlignment(verticalAlignment);
            }
            return this;
        }

        public Column MoveLeft() => new(Host(), col - 1);

        public Column MoveRight() => new(Host(), col + 1);
    }
}
