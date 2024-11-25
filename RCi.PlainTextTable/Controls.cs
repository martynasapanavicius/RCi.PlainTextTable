using System;
using System.Collections.Generic;
using System.Linq;

namespace RCi.PlainTextTable
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
        public abstract RowSpan Slice(int offset, int length);
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
        public abstract ColumnSpan Slice(int offset, int length);
    }

    public sealed class Row(PlainTextTable host, int row) :
        RowBase(host, row)
    {
        public override Cell First() => Host().Cell(RowIndex, 0);
        public override Cell Last() => Host().Cell(RowIndex, Host().ColumnCount - 1);
        public override RowSpan Slice(int offset, int length) => new(Host(), RowIndex, offset, length);
    }

    public sealed class Column(PlainTextTable host, int col) :
        ColumnBase(host, col)
    {
        public override Cell First() => Host().Cell(0, ColumnIndex);
        public override Cell Last() => Host().Cell(Host().RowCount - 1, ColumnIndex);
        public override ColumnSpan Slice(int offset, int length) => new(Host(), ColumnIndex, offset, length);
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
        public override RowSpan Slice(int offset, int length)
        {
            var absoluteStart = Offset + offset;
            var absoluteEnd = absoluteStart + length;
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
        public override ColumnSpan Slice(int offset, int length)
        {
            var absoluteStart = Offset + offset;
            var absoluteEnd = absoluteStart + length;
            absoluteStart = Math.Clamp(absoluteStart, Offset, Offset + Length);
            absoluteEnd = Math.Clamp(absoluteEnd, Offset, Offset + Length);
            return new ColumnSpan(Host(), ColumnIndex, absoluteStart, absoluteEnd - absoluteStart);
        }
    }

    // ================================================================

    public static class ControlMovementExtensions
    {
        public static Row Row(this RowBase row) => row.Host().Row(row.RowIndex);
        public static Column Column(this ColumnBase column) => column.Host().Column(column.ColumnIndex);

        public static RowSpan Slice(this Row row, (int Offset, int Length) offsetAndLength) => row.Slice(offsetAndLength.Offset, offsetAndLength.Length);
        public static RowSpan Slice(this Row row, Range range) => row.Slice(range.GetOffsetAndLength(row.Host().ColumnCount));
        public static RowSpan Slice(this Row row, Index start, Index end) => row.Slice(new Range(start, end));
        public static RowSpan Slice(this Row row, int offset) => row.Slice(new Index(offset), ^0);
        public static RowSpan Slice(this Row row, Index offset) => row.Slice(offset, ^0);

        public static ColumnSpan Slice(this Column column, (int Offset, int Length) offsetAndLength) => column.Slice(offsetAndLength.Offset, offsetAndLength.Length);
        public static ColumnSpan Slice(this Column column, Range range) => column.Slice(range.GetOffsetAndLength(column.Host().RowCount));
        public static ColumnSpan Slice(this Column column, Index start, Index end) => column.Slice(new Range(start, end));
        public static ColumnSpan Slice(this Column column, int offset) => column.Slice(new Index(offset), ^0);
        public static ColumnSpan Slice(this Column column, Index offset) => column.Slice(offset, ^0);

        public static RowSpan Slice(this RowSpan rowSpan, (int Offset, int Length) offsetAndLength) => rowSpan.Slice(offsetAndLength.Offset, offsetAndLength.Length);
        public static RowSpan Slice(this RowSpan rowSpan, Range range) => rowSpan.Slice(range.GetOffsetAndLength(rowSpan.Length));
        public static RowSpan Slice(this RowSpan rowSpan, Index start, Index end) => rowSpan.Slice(new Range(start, end));
        public static RowSpan Slice(this RowSpan rowSpan, int offset) => rowSpan.Slice(new Index(offset), ^0);
        public static RowSpan Slice(this RowSpan rowSpan, Index offset) => rowSpan.Slice(offset, ^0);

        public static ColumnSpan Slice(this ColumnSpan columnSpan, (int Offset, int Length) offsetAndLength) => columnSpan.Slice(offsetAndLength.Offset, offsetAndLength.Length);
        public static ColumnSpan Slice(this ColumnSpan columnSpan, Range range) => columnSpan.Slice(range.GetOffsetAndLength(columnSpan.Length));
        public static ColumnSpan Slice(this ColumnSpan columnSpan, Index start, Index end) => columnSpan.Slice(new Range(start, end));
        public static ColumnSpan Slice(this ColumnSpan columnSpan, int offset) => columnSpan.Slice(new Index(offset), ^0);
        public static ColumnSpan Slice(this ColumnSpan columnSpan, Index offset) => columnSpan.Slice(offset, ^0);

        public static Row MoveUp(this Row row) => new(row.Host(), row.RowIndex - 1);
        public static Row MoveDown(this Row row) => new(row.Host(), row.RowIndex + 1);
        public static Column MoveLeft(this Column column) => new(column.Host(), column.ColumnIndex - 1);
        public static Column MoveRight(this Column column) => new(column.Host(), column.ColumnIndex + 1);

        public static RowSpan MoveUp(this RowSpan rowSpan) => new(rowSpan.Host(), rowSpan.RowIndex - 1, rowSpan.Offset, rowSpan.Length);
        public static RowSpan MoveDown(this RowSpan rowSpan) => new(rowSpan.Host(), rowSpan.RowIndex + 1, rowSpan.Offset, rowSpan.Length);
        public static ColumnSpan MoveLeft(this ColumnSpan columnSpan) => new(columnSpan.Host(), columnSpan.ColumnIndex - 1, columnSpan.Offset, columnSpan.Length);
        public static ColumnSpan MoveRight(this ColumnSpan columnSpan) => new(columnSpan.Host(), columnSpan.ColumnIndex + 1, columnSpan.Offset, columnSpan.Length);

        public static RowSpan Take(this RowBase row, int count) => row.Slice(0, count);
        public static RowSpan TakeLast(this RowBase row, int count) => row.Slice(row.Host().ColumnCount - count, count);
        public static ColumnSpan Take(this ColumnBase column, int count) => column.Slice(0, count);
        public static ColumnSpan TakeLast(this ColumnBase column, int count) => column.Slice(column.Host().RowCount - count, count);
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

        public static TControl SetVerticalAlignment<TControl>(this TControl control, params VerticalAlignment[] verticalAlignments)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.SetVerticalAlignment(v), control.First(), control.Next, verticalAlignments);

        public static TControl SetVerticalAlignment<TControl>(this TControl control, VerticalAlignment verticalAlignment)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.SetVerticalAlignment(v), verticalAlignment);
    }
}
