namespace RCi.PlainTextTable
{
    public readonly struct ColumnChooser(PlainTextTable host)
    {
        public ColumnControl this[int col] => new(host, col);
    }
}
