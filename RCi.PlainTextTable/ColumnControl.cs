namespace RCi.PlainTextTable
{
    public readonly struct ColumnControl(PlainTextTable host, int col)
    {
        public Cell this[int row] => host[row, col];
        public PlainTextTable Host => host;

        public PlainTextTable Delete()
        {
            var rowCount = host.RowCount;
            for (var i = rowCount - 1; i >= 0; i--)
            {
                host[i, col].Delete();
            }
            return host;
        }

        public ColumnControl Text(params string[] texts)
        {
            for (var i = 0; i < texts.Length; i++)
            {
                host[i, col].Text(texts[i]);
            }
            return this;
        }

        public ColumnControl Text(string text)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].Text(text);
            }
            return this;
        }

        public ColumnControl Margin(params Margin[] margins)
        {
            for (var i = 0; i < margins.Length; i++)
            {
                host[i, col].Margin(margins[i]);
            }
            return this;
        }

        public ColumnControl Margin(Margin margin)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].Margin(margin);
            }
            return this;
        }

        public ColumnControl Borders(params Borders[] borders)
        {
            for (var i = 0; i < borders.Length; i++)
            {
                host[i, col].Borders(borders[i]);
            }
            return this;
        }

        public ColumnControl Borders(Borders borders)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].Borders(borders);
            }
            return this;
        }

        public ColumnControl HorizontalAlignment(params HorizontalAlignment[] horizontalAlignments)
        {
            for (var i = 0; i < horizontalAlignments.Length; i++)
            {
                host[i, col].HorizontalAlignment(horizontalAlignments[i]);
            }
            return this;
        }

        public ColumnControl HorizontalAlignment(HorizontalAlignment horizontalAlignment)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].HorizontalAlignment(horizontalAlignment);
            }
            return this;
        }

        public ColumnControl VerticalAlignment(params VerticalAlignment[] verticalAlignment)
        {
            for (var i = 0; i < verticalAlignment.Length; i++)
            {
                host[i, col].VerticalAlignment(verticalAlignment[i]);
            }
            return this;
        }

        public ColumnControl VerticalAlignment(VerticalAlignment verticalAlignment)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].VerticalAlignment(verticalAlignment);
            }
            return this;
        }
    }
}
