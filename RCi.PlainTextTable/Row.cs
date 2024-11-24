namespace RCi.PlainTextTable
{
    public readonly struct Row(PlainTextTable host, int row)
    {
        public Cell this[int col] => host[row, col];
        public Cell Col(int col) => host[row, col];
        public PlainTextTable Host() => host;

        public PlainTextTable Delete()
        {
            var columnCount = host.ColumnCount;
            for (var i = columnCount - 1; i >= 0; i--)
            {
                host[row, i].Delete();
            }
            return host;
        }

        public Row Text(params string[] texts)
        {
            for (var i = 0; i < texts.Length; i++)
            {
                host[row, i].Text(texts[i]);
            }
            return this;
        }

        public Row Text(params object[] texts)
        {
            for (var i = 0; i < texts.Length; i++)
            {
                host[row, i].Text(texts[i]);
            }
            return this;
        }

        public Row Text(string text)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].Text(text);
            }
            return this;
        }

        public Row Text(object text)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].Text(text);
            }
            return this;
        }

        public Row RowSpan(params int[] rowSpans)
        {
            for (var i = 0; i < rowSpans.Length; i++)
            {
                host[row, i].RowSpan(rowSpans[i]);
            }
            return this;
        }

        public Row RowSpan(int rowSpan)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].RowSpan(rowSpan);
            }
            return this;
        }

        public Row ColumnSpan(params int[] columnSpans)
        {
            for (var i = 0; i < columnSpans.Length; i++)
            {
                host[row, i].ColumnSpan(columnSpans[i]);
            }
            return this;
        }

        public Row ColumnSpan(int columnSpan)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].ColumnSpan(columnSpan);
            }
            return this;
        }

        public Row RowAndColumnSpan(params (int RowSpan, int ColumnSpan)[] rowAndColumnSpans)
        {
            for (var i = 0; i < rowAndColumnSpans.Length; i++)
            {
                host[row, i].RowSpan(rowAndColumnSpans[i].RowSpan);
                host[row, i].ColumnSpan(rowAndColumnSpans[i].ColumnSpan);
            }
            return this;
        }

        public Row RowAndColumnSpan((int RowSpan, int ColumnSpan) rowAndColumnSpan)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].RowSpan(rowAndColumnSpan.RowSpan);
                host[row, i].ColumnSpan(rowAndColumnSpan.ColumnSpan);
            }
            return this;
        }

        public Row Margin(params Margin[] margins)
        {
            for (var i = 0; i < margins.Length; i++)
            {
                host[row, i].Margin(margins[i]);
            }
            return this;
        }

        public Row Margin(Margin margin)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].Margin(margin);
            }
            return this;
        }

        public Row Borders(params Borders[] borders)
        {
            for (var i = 0; i < borders.Length; i++)
            {
                host[row, i].Borders(borders[i]);
            }
            return this;
        }

        public Row Borders(Borders borders)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].Borders(borders);
            }
            return this;
        }

        public Row HorizontalAlignment(params HorizontalAlignment[] horizontalAlignments)
        {
            for (var i = 0; i < horizontalAlignments.Length; i++)
            {
                host[row, i].HorizontalAlignment(horizontalAlignments[i]);
            }
            return this;
        }

        public Row HorizontalAlignment(HorizontalAlignment horizontalAlignment)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].HorizontalAlignment(horizontalAlignment);
            }
            return this;
        }

        public Row VerticalAlignment(params VerticalAlignment[] verticalAlignment)
        {
            for (var i = 0; i < verticalAlignment.Length; i++)
            {
                host[row, i].VerticalAlignment(verticalAlignment[i]);
            }
            return this;
        }

        public Row VerticalAlignment(VerticalAlignment verticalAlignment)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].VerticalAlignment(verticalAlignment);
            }
            return this;
        }

        public Row MoveUp() => new(Host(), row - 1);

        public Row MoveDown() => new(Host(), row + 1);
    }
}
