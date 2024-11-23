namespace RCi.PlainTextTable
{
    public readonly struct ColChooser(PlainTextTable host)
    {
        public ColumnControl this[int col] => new(host, col);
    }
}
