namespace RCi.PlainTextTable
{
    public readonly struct RowControl(PlainTextTable host, int row)
    {
        public ICell this[int col] => host[row, col];
        public PlainTextTable Host() => host;

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
    }
}
