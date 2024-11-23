namespace RCi.PlainTextTable
{
    public readonly struct ColumnControl(PlainTextTable host, int col)
    {
        public Cell this[int row] => host[row, col];
        public PlainTextTable Host() => host;

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
    }
}
