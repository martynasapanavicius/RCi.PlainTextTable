using System;
using System.Collections.Generic;
using System.Linq;

namespace RCi.PlainTextTable
{
    public abstract class ControlBase(PlainTextTable host)
    {
        public PlainTextTable Host() => host;
        public abstract Cell FirstCell();
        internal abstract Cell NextCell(Cell cell);
        public abstract IEnumerable<Cell> ExistingCells();
    }

    public abstract class RowBase(PlainTextTable host, int row) :
        ControlBase(host)
    {
        public int RowIndex { get; } = row;
        public Row Row() => Host().Row(RowIndex);
        public abstract Cell Skip(int count);
        public override Cell FirstCell() => Skip(0);
        internal override Cell NextCell(Cell cell) => Host().Cell(cell.Coordinate.MoveRight());
        public override IEnumerable<Cell> ExistingCells()
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
    }

    public abstract class ColumnBase(PlainTextTable host, int col) :
        ControlBase(host)
    {
        public int ColumnIndex { get; } = col;
        public Column Column() => Host().Column(ColumnIndex);
        public abstract Cell Skip(int count);
        public override Cell FirstCell() => Skip(0);
        internal override Cell NextCell(Cell cell) => Host().Cell(cell.Coordinate.MoveDown());
        public override IEnumerable<Cell> ExistingCells()
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
    }

    public class Row(PlainTextTable host, int row) :
        RowBase(host, row)
    {
        public override Cell Skip(int count) => Host().Cell(RowIndex, count);
        public Row MoveUp() => new(Host(), RowIndex - 1);
        public Row MoveDown() => new(Host(), RowIndex + 1);
        public RowSpan Slice(int offset, int length) => new(Host(), RowIndex, offset, length);
        public RowSpan Slice(int offset) => Slice(offset, Host().ColumnCount - offset);
        public RowSpan Slice(Range range) => new(Host(), RowIndex, range.GetOffsetAndLength(Host().ColumnCount));
        public RowSpan Take(int count) => Slice(0, count);
        public RowSpan TakeLast(int count) => Slice(Host().ColumnCount - 1 - count, count);
    }

    public class Column(PlainTextTable host, int col) :
        ColumnBase(host, col)
    {
        public override Cell Skip(int count) => Host().Cell(count, ColumnIndex);
        public Column MoveLeft() => new(Host(), ColumnIndex - 1);
        public Column MoveRight() => new(Host(), ColumnIndex + 1);
        public ColumnSpan Slice(int offset, int length) => new(Host(), ColumnIndex, offset, length);
        public ColumnSpan Slice(int offset) => Slice(offset, Host().RowCount - offset);
        public ColumnSpan Slice(Range range) => new(Host(), ColumnIndex, range.GetOffsetAndLength(Host().RowCount));
        public ColumnSpan Take(int count) => Slice(0, count);
        public ColumnSpan TakeLast(int count) => Slice(Host().ColumnCount - 1 - count, count);
    }

    public sealed class RowSpan(PlainTextTable host, int row, int offset, int length) :
        RowBase(host, row)
    {
        public RowSpan(PlainTextTable host, int row, (int Offset, int Length) t)
            : this(host, row, t.Offset, t.Length) { }

        public override Cell Skip(int count) => Host().Cell(RowIndex, offset + count);
        public override IEnumerable<Cell> ExistingCells()
        {
            var host = Host();
            for (var i = 0; i < length; i++)
            {
                yield return host[RowIndex, offset + i];
            }
        }
        public RowSpan MoveUp() => new(Host(), RowIndex - 1, offset, length);
        public RowSpan MoveDown() => new(Host(), RowIndex + 1, offset, length);
    }

    public sealed class ColumnSpan(PlainTextTable host, int col, int offset, int length) :
        ColumnBase(host, col)
    {
        public ColumnSpan(PlainTextTable host, int row, (int Offset, int Length) t)
            : this(host, row, t.Offset, t.Length) { }

        public override Cell Skip(int count) => Host().Cell(offset + count, ColumnIndex);
        public override IEnumerable<Cell> ExistingCells()
        {
            var host = Host();
            for (var i = 0; i < length; i++)
            {
                yield return host[offset + i, ColumnIndex];
            }
        }
        public ColumnSpan MoveLeft() => new(Host(), ColumnIndex - 1, offset, length);
        public ColumnSpan MoveRight() => new(Host(), ColumnIndex + 1, offset, length);
    }

    // ---

    public static class ControlExtensions
    {
        public static TControl Delete<TControl>(this TControl control)
            where TControl : ControlBase
        {
            foreach (var cell in control.ExistingCells().Reverse())
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
            foreach (var cell in control.ExistingCells())
            {
                setter(cell, uniformValue);
            }
            return control;
        }

        public static TControl Text<TControl>(this TControl control, params string[] texts)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.Text(v), control.FirstCell(), control.NextCell, texts);

        public static TControl Text<TControl>(this TControl control, params object[] texts)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.Text(v), control.FirstCell(), control.NextCell, texts);

        public static TControl Text<TControl>(this TControl control, string text)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.Text(v), text);

        public static TControl Text<TControl>(this TControl control, object text)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.Text(v), text);

        public static TControl Margin<TControl>(this TControl control, params Margin[] margins)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.Margin(v), control.FirstCell(), control.NextCell, margins);

        public static TControl Margin<TControl>(this TControl control, Margin margin)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.Margin(v), margin);

        public static TControl RowSpan<TControl>(this TControl control, params int[] rowSpans)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.RowSpan(v), control.FirstCell(), control.NextCell, rowSpans);

        public static TControl RowSpan<TControl>(this TControl control, int rowSpan)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.RowSpan(v), rowSpan);

        public static TControl ColumnSpan<TControl>(this TControl control, params int[] columnSpans)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.ColumnSpan(v), control.FirstCell(), control.NextCell, columnSpans);

        public static TControl ColumnSpan<TControl>(this TControl control, int columnSpan)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.ColumnSpan(v), columnSpan);

        public static TControl Borders<TControl>(this TControl control, params Borders[] borders)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.Borders(v), control.FirstCell(), control.NextCell, borders);

        public static TControl Borders<TControl>(this TControl control, Borders borders)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.Borders(v), borders);

        public static TControl HorizontalAlignment<TControl>(this TControl control, params HorizontalAlignment[] horizontalAlignments)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.HorizontalAlignment(v), control.FirstCell(), control.NextCell, horizontalAlignments);

        public static TControl HorizontalAlignment<TControl>(this TControl control, HorizontalAlignment horizontalAlignment)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.HorizontalAlignment(v), horizontalAlignment);

        public static TControl VerticalAlignment<TControl>(this TControl control, params VerticalAlignment[] verticalAlignments)
            where TControl : ControlBase =>
            control.Apply((c, v) => c.VerticalAlignment(v), control.FirstCell(), control.NextCell, verticalAlignments);

        public static TControl VerticalAlignment<TControl>(this TControl control, VerticalAlignment verticalAlignment)
            where TControl : ControlBase =>
            control.ApplyUniform((c, v) => c.VerticalAlignment(v), verticalAlignment);
    }
}
