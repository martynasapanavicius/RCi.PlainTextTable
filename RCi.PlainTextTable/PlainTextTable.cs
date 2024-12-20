﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RCi.Toolbox.Ptt
{
    public sealed class PlainTextTable
    {
        // storage

        private readonly Dictionary<Coordinate, Cell> _cells = new();
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public BorderStyle BorderStyle { get; set; } = BorderStyle.Ascii;
        public Borders DefaultBorders { get; set; } = new(Border.Normal);
        public Margin DefaultMargin { get; set; } = new(1, 0);
        public HorizontalAlignment DefaultHorizontalAlignment { get; set; } = HorizontalAlignment.Left;
        public VerticalAlignment DefaultVerticalAlignment { get; set; } = VerticalAlignment.Top;

        // derivatives

        public Cell this[int row, int col] => Cell(new Coordinate(row, col));
        public Cell this[Coordinate c] => Cell(c);
        public Cell Cell(int row, int col) => Cell(new Coordinate(row, col));
        public Cell Cell(Coordinate c)
        {
            if (_cells.TryGetValue(c, out var cell))
            {
                return cell;
            }
            cell = new Cell(this, c);
            _cells.Add(c, cell);
            RowCount = Math.Max(RowCount, c.Row + 1);
            ColumnCount = Math.Max(ColumnCount, c.Col + 1);
            return cell;
        }

        internal void DeleteCell(Cell cell)
        {
            if (!ReferenceEquals(cell.Host(), this))
            {
                throw new ArgumentException("cell does not belong to this plain text table");
            }
            _cells.Remove(cell.Coordinate);
            RowCount = _cells.Count == 0 ? 0 : _cells.Max(p => p.Key.Row) + 1;
            ColumnCount = _cells.Count == 0 ? 0 : _cells.Max(p => p.Key.Col) + 1;
        }

        public Row AppendRow()
        {
            var row = RowCount;
            RowCount++;
            return new Row(this, row);
        }

        public Column AppendColumn()
        {
            var col = ColumnCount;
            ColumnCount++;
            return new Column(this, col);
        }

        public override string ToString() => RenderTable
        (
            BorderStyle,
            _cells.Where(c => c.Value.IsAlive)
                .Select(p => new LogicalCell
                {
                    Coordinate = p.Value.Coordinate,
                    Text = p.Value.Text,
                    ColumnSpan = p.Value.ColumnSpan,
                    RowSpan = p.Value.RowSpan,
                    Margin = p.Value.Margin ?? DefaultMargin,
                    Borders = p.Value.Borders ?? DefaultBorders,
                    HorizontalAlignment = p.Value.HorizontalAlignment ?? DefaultHorizontalAlignment,
                    VerticalAlignment = p.Value.VerticalAlignment ?? DefaultVerticalAlignment,
                })
        );

        internal static string RenderTable(BorderStyle style, IEnumerable<LogicalCell> logicalCells)
        {
            TableBuilder.BuildPhysicalTable
            (
                logicalCells,
                out var logicalCellsMap,
                out var logicalToPhysicalMap,
                out var physicalVerticalBorderWidths,
                out var physicalHorizontalBorderHeights,
                out var physicalColWidths,
                out var physicalRowHeights
            );
            return TableRenderer.RenderText
            (
                logicalCellsMap,
                logicalToPhysicalMap,
                physicalVerticalBorderWidths,
                physicalHorizontalBorderHeights,
                physicalColWidths,
                physicalRowHeights,
                style
            );
        }
    }
}
