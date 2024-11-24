namespace RCi.PlainTextTable
{
    public readonly struct RowControl(PlainTextTable host, int row)
    {
        public Cell this[int col] => host[row, col];
        public PlainTextTable Host => host;

        public PlainTextTable Delete()
        {
            var columnCount = host.ColumnCount;
            for (var i = columnCount - 1; i >= 0; i--)
            {
                host[row, i].Delete();
            }
            return host;
        }

        public RowControl Text(params string[] texts)
        {
            for (var i = 0; i < texts.Length; i++)
            {
                host[row, i].Text(texts[i]);
            }
            return this;
        }

        public RowControl Text(string text)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].Text(text);
            }
            return this;
        }

        public RowControl Margin(params Margin[] margins)
        {
            for (var i = 0; i < margins.Length; i++)
            {
                host[row, i].Margin(margins[i]);
            }
            return this;
        }

        public RowControl Margin(Margin margin)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].Margin(margin);
            }
            return this;
        }

        public RowControl Borders(params Borders[] borders)
        {
            for (var i = 0; i < borders.Length; i++)
            {
                host[row, i].Borders(borders[i]);
            }
            return this;
        }

        public RowControl Borders(Borders borders)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].Borders(borders);
            }
            return this;
        }

        public RowControl HorizontalAlignment(params HorizontalAlignment[] horizontalAlignments)
        {
            for (var i = 0; i < horizontalAlignments.Length; i++)
            {
                host[row, i].HorizontalAlignment(horizontalAlignments[i]);
            }
            return this;
        }

        public RowControl HorizontalAlignment(HorizontalAlignment horizontalAlignment)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].HorizontalAlignment(horizontalAlignment);
            }
            return this;
        }

        public RowControl VerticalAlignment(params VerticalAlignment[] verticalAlignment)
        {
            for (var i = 0; i < verticalAlignment.Length; i++)
            {
                host[row, i].VerticalAlignment(verticalAlignment[i]);
            }
            return this;
        }

        public RowControl VerticalAlignment(VerticalAlignment verticalAlignment)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].VerticalAlignment(verticalAlignment);
            }
            return this;
        }
    }
}
