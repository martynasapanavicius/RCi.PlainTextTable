using System;
using System.Collections.Generic;
using System.Linq;

namespace RCi.PlainTextTable
{
    public abstract class ControlBase(PlainTextTable host)
    {
        public PlainTextTable Host() => host;
        public abstract Cell FirstCell();
        public abstract Cell NextCell(Cell cell);
        public abstract IEnumerable<Cell> ExistingCells();
    }

    public sealed class Row(PlainTextTable host, int row) :
        ControlBase(host)
    {
        public int Index { get; } = row;
        public Cell Col(int col) => Host().Cell(Index, col);
        public override Cell FirstCell() => Col(0);
        public override Cell NextCell(Cell cell) => Host().Cell(cell.Coordinate.MoveRight());
        public override IEnumerable<Cell> ExistingCells()
        {
            var host = Host();
            for (var i = 0; i < host.ColumnCount; i++)
            {
                yield return host[Index, i];
            }
        }
    }

    public sealed class Column(PlainTextTable host, int col) :
        ControlBase(host)
    {
        public int Index { get; } = col;
        public Cell Row(int row) => Host().Cell(row, Index);
        public override Cell FirstCell() => Row(0);
        public override Cell NextCell(Cell cell) => Host().Cell(cell.Coordinate.MoveDown());
        public override IEnumerable<Cell> ExistingCells()
        {
            var host = Host();
            for (var i = 0; i < host.RowCount; i++)
            {
                yield return host[i, Index];
            }
        }
    }

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
