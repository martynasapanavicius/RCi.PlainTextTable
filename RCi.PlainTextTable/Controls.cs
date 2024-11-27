using System;
using System.Collections.Generic;
using System.Linq;

namespace RCi.Toolbox
{
    public abstract class ControlBase(PlainTextTable host)
    {
        public PlainTextTable Host() => host;
        internal abstract Cell Next(Cell cell);
        internal abstract IEnumerable<Cell> EnumerateAlive();
        public abstract Cell First();
        public abstract Cell Last();
    }

    public abstract class RowBase(PlainTextTable host, int row) :
        ControlBase(host)
    {
        public int RowIndex { get; } = row;
        internal override Cell Next(Cell cell) => Host().Cell(cell.Coordinate.MoveRight());
        internal override IEnumerable<Cell> EnumerateAlive()
        {
            var host = Host();
            for (var i = 0; i < host.ColumnCount; i++)
            {
                var cell = host[RowIndex, i];
                if (cell.IsAlive)
                {
                    yield return cell;
                }
            }
        }
        public abstract RowSpan Slice(Range range);
    }

    public abstract class ColumnBase(PlainTextTable host, int col) :
        ControlBase(host)
    {
        public int ColumnIndex { get; } = col;
        internal override Cell Next(Cell cell) => Host().Cell(cell.Coordinate.MoveDown());
        internal override IEnumerable<Cell> EnumerateAlive()
        {
            var host = Host();
            for (var i = 0; i < host.RowCount; i++)
            {
                var cell = host[i, ColumnIndex];
                if (cell.IsAlive)
                {
                    yield return cell;
                }
            }
        }
        public abstract ColumnSpan Slice(Range range);
    }

    public sealed class Row(PlainTextTable host, int row) :
        RowBase(host, row)
    {
        public override Cell First() => Host().Cell(RowIndex, 0);
        public override Cell Last() => Host().Cell(RowIndex, Host().ColumnCount - 1);
        public override RowSpan Slice(Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(Host().ColumnCount);
            return new RowSpan(Host(), RowIndex, offset, length);
        }
    }

    public sealed class Column(PlainTextTable host, int col) :
        ColumnBase(host, col)
    {
        public override Cell First() => Host().Cell(0, ColumnIndex);
        public override Cell Last() => Host().Cell(Host().RowCount - 1, ColumnIndex);
        public override ColumnSpan Slice(Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(Host().RowCount);
            return new ColumnSpan(Host(), ColumnIndex, offset, length);
        }
    }

    public sealed class RowSpan(PlainTextTable host, int row, int offset, int length) :
        RowBase(host, row)
    {
        public int Offset { get; } = offset;
        public int Length { get; } = length;
        internal override IEnumerable<Cell> EnumerateAlive()
        {
            var host = Host();
            for (var i = 0; i < Length; i++)
            {
                var cell = host[RowIndex, Offset + i];
                if (cell.IsAlive)
                {
                    yield return cell;
                }
            }
        }
        public override Cell First() => Host().Cell(RowIndex, Offset);
        public override Cell Last() => Host().Cell(RowIndex, Offset + Length - 1);
        public override RowSpan Slice(Range range)
        {
            var start = range.Start.IsFromEnd
                ? Length - range.Start.Value
                : range.Start.Value;
            var end = range.End.IsFromEnd
                ? Length - range.End.Value
                : range.End.Value;
            var absoluteStart = Offset + start;
            var absoluteEnd = absoluteStart + end;
            absoluteStart = Math.Clamp(absoluteStart, Offset, Offset + Length);
            absoluteEnd = Math.Clamp(absoluteEnd, Offset, Offset + Length);
            return new RowSpan(Host(), RowIndex, absoluteStart, absoluteEnd - absoluteStart);
        }
    }

    public sealed class ColumnSpan(PlainTextTable host, int col, int offset, int length) :
        ColumnBase(host, col)
    {
        public int Offset { get; } = offset;
        public int Length { get; } = length;
        internal override IEnumerable<Cell> EnumerateAlive()
        {
            var host = Host();
            for (var i = 0; i < Length; i++)
            {
                var cell = host[Offset + i, ColumnIndex];
                if (cell.IsAlive)
                {
                    yield return cell;
                }
            }
        }
        public override Cell First() => Host().Cell(Offset, ColumnIndex);
        public override Cell Last() => Host().Cell(Offset + Length - 1, ColumnIndex);
        public override ColumnSpan Slice(Range range)
        {
            var start = range.Start.IsFromEnd
                ? Length - range.Start.Value
                : range.Start.Value;
            var end = range.End.IsFromEnd
                ? Length - range.End.Value
                : range.End.Value;
            var absoluteStart = Offset + start;
            var absoluteEnd = absoluteStart + end;
            absoluteStart = Math.Clamp(absoluteStart, Offset, Offset + Length);
            absoluteEnd = Math.Clamp(absoluteEnd, Offset, Offset + Length);
            return new ColumnSpan(Host(), ColumnIndex, absoluteStart, absoluteEnd - absoluteStart);
        }
    }

    // ================================================================

    public static class PlainTextTableMovementExtensions
    {
        public static Row AppendRow(this PlainTextTable host, params string[] texts) => host.AppendRow().SetText(texts);
        public static Row AppendRow(this PlainTextTable host, params object[] texts) => host.AppendRow().SetText(texts);
        public static Column AppendColumn(this PlainTextTable host, params string[] texts) => host.AppendColumn().SetText(texts);
        public static Column AppendColumn(this PlainTextTable host, params object[] texts) => host.AppendColumn().SetText(texts);
        public static Row Row(this PlainTextTable host, int row) => new(host, row);
        public static Row Row(this PlainTextTable host, Index row) => new(host, row.IsFromEnd ? host.RowCount - row.Value : row.Value);
        public static Column Column(this PlainTextTable host, int col) => new(host, col);
        public static Column Column(this PlainTextTable host, Index col) => new(host, col.IsFromEnd ? host.ColumnCount - col.Value : col.Value);
        public static Row FirstRow(this PlainTextTable host) => new(host, 0);
        public static Row LastRow(this PlainTextTable host) => new(host, host.RowCount - 1);
        public static Column FirstColumn(this PlainTextTable host) => new(host, 0);
        public static Column LastColumn(this PlainTextTable host) => new(host, host.ColumnCount - 1);
    }

    public static class ControlMovementExtensions
    {
        public static Row Row(this RowBase rowBase) => rowBase.Host().Row(rowBase.RowIndex);
        public static Column Column(this ColumnBase columnBase) => columnBase.Host().Column(columnBase.ColumnIndex);

        public static RowSpan Slice(this RowBase rowBase, Index start, Index end) => rowBase.Slice(new Range(start, end));
        public static RowSpan Slice(this RowBase rowBase, int offset, int length) => rowBase.Slice(new Index(offset), new Index(offset + length));
        public static RowSpan Slice(this RowBase rowBase, Index offset) => rowBase.Slice(offset, ^0);
        public static RowSpan Slice(this RowBase rowBase, int offset) => rowBase.Slice(new Index(offset));

        public static ColumnSpan Slice(this ColumnBase columnBase, Index start, Index end) => columnBase.Slice(new Range(start, end));
        public static ColumnSpan Slice(this ColumnBase columnBase, int offset, int length) => columnBase.Slice(new Index(offset), new Index(offset + length));
        public static ColumnSpan Slice(this ColumnBase columnBase, Index offset) => columnBase.Slice(offset, ^0);
        public static ColumnSpan Slice(this ColumnBase columnBase, int offset) => columnBase.Slice(new Index(offset));

        public static RowSpan Slice(this Row row, Index start, Index end) => row.Slice(new Range(start, end));
        public static RowSpan Slice(this Row row, int offset, int length) => row.Slice(new Index(offset), new Index(offset + length));
        public static RowSpan Slice(this Row row, Index offset) => row.Slice(offset, ^0);
        public static RowSpan Slice(this Row row, int offset) => row.Slice(new Index(offset));

        public static ColumnSpan Slice(this Column column, Index start, Index end) => column.Slice(new Range(start, end));
        public static ColumnSpan Slice(this Column column, int offset, int length) => column.Slice(new Index(offset), new Index(offset + length));
        public static ColumnSpan Slice(this Column column, Index offset) => column.Slice(offset, ^0);
        public static ColumnSpan Slice(this Column column, int offset) => column.Slice(new Index(offset));

        public static RowSpan Slice(this RowSpan rowSpan, Index start, Index end) => rowSpan.Slice(new Range(start, end));
        public static RowSpan Slice(this RowSpan rowSpan, int offset, int length) => rowSpan.Slice(new Index(offset), new Index(offset + length));
        public static RowSpan Slice(this RowSpan rowSpan, Index offset) => rowSpan.Slice(offset, ^0);
        public static RowSpan Slice(this RowSpan rowSpan, int offset) => rowSpan.Slice(new Index(offset));

        public static ColumnSpan Slice(this ColumnSpan columnSpan, Index start, Index end) => columnSpan.Slice(new Range(start, end));
        public static ColumnSpan Slice(this ColumnSpan columnSpan, int offset, int length) => columnSpan.Slice(new Index(offset), new Index(offset + length));
        public static ColumnSpan Slice(this ColumnSpan columnSpan, Index offset) => columnSpan.Slice(offset, ^0);
        public static ColumnSpan Slice(this ColumnSpan columnSpan, int offset) => columnSpan.Slice(new Index(offset));

        public static Row MoveUp(this Row row, int count = 1) => new(row.Host(), row.RowIndex - count);
        public static Row MoveDown(this Row row, int count = 1) => new(row.Host(), row.RowIndex + count);
        public static Column MoveLeft(this Column column, int count = 1) => new(column.Host(), column.ColumnIndex - count);
        public static Column MoveRight(this Column column, int count = 1) => new(column.Host(), column.ColumnIndex + count);

        public static Row MoveUpToFirst(this Row row) => new(row.Host(), 0);
        public static Row MoveDownToLast(this Row row) => new(row.Host(), row.Host().RowCount - 1);
        public static Column MoveLeftToFirst(this Column column) => new(column.Host(), 0);
        public static Column MoveRightToLast(this Column column) => new(column.Host(), column.Host().ColumnCount - 1);

        public static RowSpan MoveUp(this RowSpan rowSpan, int count = 1) => new(rowSpan.Host(), rowSpan.RowIndex - count, rowSpan.Offset, rowSpan.Length);
        public static RowSpan MoveDown(this RowSpan rowSpan, int count = 1) => new(rowSpan.Host(), rowSpan.RowIndex + count, rowSpan.Offset, rowSpan.Length);
        public static ColumnSpan MoveLeft(this ColumnSpan columnSpan, int count = 1) => new(columnSpan.Host(), columnSpan.ColumnIndex - count, columnSpan.Offset, columnSpan.Length);
        public static ColumnSpan MoveRight(this ColumnSpan columnSpan, int count = 1) => new(columnSpan.Host(), columnSpan.ColumnIndex + count, columnSpan.Offset, columnSpan.Length);

        public static RowSpan MoveUpToFirst(this RowSpan rowSpan) => new(rowSpan.Host(), 0, rowSpan.Offset, rowSpan.Length);
        public static RowSpan MoveDownToLast(this RowSpan rowSpan) => new(rowSpan.Host(), rowSpan.Host().RowCount - 1, rowSpan.Offset, rowSpan.Length);
        public static ColumnSpan MoveLeftToFirst(this ColumnSpan columnSpan) => new(columnSpan.Host(), 0, columnSpan.Offset, columnSpan.Length);
        public static ColumnSpan MoveRightToLast(this ColumnSpan columnSpan) => new(columnSpan.Host(), columnSpan.Host().ColumnCount - 1, columnSpan.Offset, columnSpan.Length);

        public static RowSpan Skip(this RowBase rowBase, int count) => rowBase.Slice(count);
        public static RowSpan SkipLast(this RowBase rowBase, int count) => rowBase.Slice(0, ^count);
        public static ColumnSpan Skip(this ColumnBase columnBase, int count) => columnBase.Slice(count);
        public static ColumnSpan SkipLast(this ColumnBase columnBase, int count) => columnBase.Slice(0, ^count);

        public static RowSpan Take(this RowBase rowBase, int count) => rowBase.Slice(0, count);
        public static RowSpan TakeLast(this RowBase rowBase, int count) => rowBase.Slice(^count, ^0);
        public static ColumnSpan Take(this ColumnBase columnBase, int count) => columnBase.Slice(0, count);
        public static ColumnSpan TakeLast(this ColumnBase columnBase, int count) => columnBase.Slice(^count, ^0);
    }

    // ================================================================

    public static class ControlModifyExtensions
    {
        public static TControl Delete<TControl>(this TControl control)
            where TControl : ControlBase
        {
            foreach (var cell in control.EnumerateAlive().Reverse())
            {
                cell.Delete();
            }
            return control;
        }

        private static TControl Apply<TControl, TValue>
        (
            this TControl control,
            Action<Cell, TValue> setter,
            Cell firstCell,
            Func<Cell, Cell> getNextCell,
            IEnumerable<TValue> values
        )
            where TControl : ControlBase
        {
            var cell = default(Cell?);
            foreach (var value in values)
            {
                cell = cell is null
                    ? firstCell
                    : getNextCell(cell);
                setter(cell, value);
            }
            return control;
        }

        private static TControl ApplyUniform<TControl, TValue>
        (
            this TControl control,
            Action<Cell, TValue> setter,
            TValue uniformValue
        )
            where TControl : ControlBase
        {
            foreach (var cell in control.EnumerateAlive())
            {
                setter(cell, uniformValue);
            }
            return control;
        }

        public static TControl SetText<TControl>(this TControl control, params string[] texts)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.SetText(v), control.First(), control.Next, texts);

        public static TControl SetText<TControl>(this TControl control, params object[] texts)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.SetText(v), control.First(), control.Next, texts);

        public static TControl SetText<TControl>(this TControl control, string text)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetText(v), text);

        public static TControl SetText<TControl>(this TControl control, object text)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetText(v), text);

        public static TControl SetMargin<TControl>(this TControl control, params Margin[] margins)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.SetMargin(v), control.First(), control.Next, margins);

        public static TControl SetMargin<TControl>(this TControl control, Margin margin)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetMargin(v), margin);

        public static TControl SetRowSpan<TControl>(this TControl control, params int[] rowSpans)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.SetRowSpan(v), control.First(), control.Next, rowSpans);

        public static TControl SetRowSpan<TControl>(this TControl control, int rowSpan)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetRowSpan(v), rowSpan);

        public static TControl SetColumnSpan<TControl>(this TControl control, params int[] columnSpans)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.SetColumnSpan(v), control.First(), control.Next, columnSpans);

        public static TControl SetColumnSpan<TControl>(this TControl control, int columnSpan)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetColumnSpan(v), columnSpan);

        public static TControl SetBorders<TControl>(this TControl control, params Borders[] borders)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.SetBorders(v), control.First(), control.Next, borders);

        public static TControl SetBorders<TControl>(this TControl control, Borders borders)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetBorders(v), borders);

        public static TControl SetBorders<TControl>(this TControl control, Border horizontal, Border vertical)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetBorders(v), new Borders(horizontal, vertical));

        public static TControl SetBorders<TControl>(this TControl control, Border border)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetBorders(v), border);

        public static TControl SetHorizontalAlignment<TControl>(this TControl control, params HorizontalAlignment[] horizontalAlignments)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.SetHorizontalAlignment(v), control.First(), control.Next, horizontalAlignments);

        public static TControl SetHorizontalAlignment<TControl>(this TControl control, HorizontalAlignment horizontalAlignment)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetHorizontalAlignment(v), horizontalAlignment);

        public static TControl SetLeftHorizontalAlignment<TControl>(this TControl control)
            where TControl : ControlBase =>
            control.SetHorizontalAlignment(HorizontalAlignment.Left);

        public static TControl SetCenterHorizontalAlignment<TControl>(this TControl control)
            where TControl : ControlBase =>
            control.SetHorizontalAlignment(HorizontalAlignment.Center);

        public static TControl SetRightHorizontalAlignment<TControl>(this TControl control)
            where TControl : ControlBase =>
            control.SetHorizontalAlignment(HorizontalAlignment.Right);

        public static TControl SetVerticalAlignment<TControl>(this TControl control, params VerticalAlignment[] verticalAlignments)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.SetVerticalAlignment(v), control.First(), control.Next, verticalAlignments);

        public static TControl SetVerticalAlignment<TControl>(this TControl control, VerticalAlignment verticalAlignment)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetVerticalAlignment(v), verticalAlignment);

        public static TControl SetTopVerticalAlignment<TControl>(this TControl control)
            where TControl : ControlBase =>
            control.SetVerticalAlignment(VerticalAlignment.Top);

        public static TControl SetCenterVerticalAlignment<TControl>(this TControl control)
            where TControl : ControlBase =>
            control.SetVerticalAlignment(VerticalAlignment.Center);

        public static TControl SetBottomVerticalAlignment<TControl>(this TControl control)
            where TControl : ControlBase =>
            control.SetVerticalAlignment(VerticalAlignment.Bottom);
    }
}
