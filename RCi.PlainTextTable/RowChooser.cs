namespace RCi.PlainTextTable
{
    public readonly struct RowChooser(PlainTextTable host)
    {
        public RowControl this[int row] => new(host, row);
    }
}
