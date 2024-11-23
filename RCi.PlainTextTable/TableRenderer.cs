﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using ReadOnlyLogicalCellsMap =
    System.Collections.Generic.IReadOnlyDictionary
    <
        RCi.PlainTextTable.Coordinate /* logical coordinate */,
        RCi.PlainTextTable.LogicalCell
    >;
using ReadOnlyLogicalToPhysicalMap =
    System.Collections.Generic.IReadOnlyDictionary
    <
        RCi.PlainTextTable.Coordinate /* logical coordinate */,
        System.Collections.Generic.HashSet<RCi.PlainTextTable.Coordinate /* physical coordinate */>
    >;

namespace RCi.PlainTextTable
{
    internal static class TableRenderer
    {
        private sealed record TableCellMargins
        {
            public required Margin Area { get; init; }

            // borders
            public required Margin LeftBorder { get; init; }
            public required Margin TopBorder { get; init; }
            public required Margin RightBorder { get; init; }
            public required Margin BottomBorder { get; init; }

            // corners
            public required Margin TopLeftBorderCorner { get; init; }
            public required Margin TopRightBorderCorner { get; init; }
            public required Margin BottomRightBorderCorner { get; init; }
            public required Margin BottomLeftBorderCorner { get; init; }
        }

        internal static string CanvasToString(char[][] canvas)
        {
            var sb = new StringBuilder();
            for (var r = 0; r < canvas.Length; r++)
            {
                var row = canvas[r];

                // patch nil characters
                for (var i = 0; i < row.Length; i++)
                {
                    if (row[i] == '\0')
                    {
                        row[i] = ' ';
                    }
                }

                sb.Append(row);

                if (r != canvas.Length - 1)
                {
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        internal static string RenderText
        (
            ReadOnlyLogicalCellsMap logicalCellsMap,
            ReadOnlyLogicalToPhysicalMap logicalToPhysicalMap,
            ImmutableArray<int> physicalVerticalBorderWidths,
            ImmutableArray<int> physicalHorizontalBorderHeights,
            ImmutableArray<int> physicalColWidths,
            ImmutableArray<int> physicalRowHeights
        )
        {
            var sumGlobalBordersWidth = physicalVerticalBorderWidths.Length == 0 ? 0 : physicalVerticalBorderWidths.Sum();
            var sumGlobalBordersHeight = physicalHorizontalBorderHeights.Length == 0 ? 0 : physicalHorizontalBorderHeights.Sum();
            var sumGlobalCellsWidth = physicalColWidths.Length == 0 ? 0 : physicalColWidths.Sum();
            var sumGlobalCellsHeight = physicalRowHeights.Length == 0 ? 0 : physicalRowHeights.Sum();
            var canvasSize = new Size(sumGlobalBordersWidth + sumGlobalCellsWidth, sumGlobalBordersHeight + sumGlobalCellsHeight);
            var canvas /* [row, col] */ = Enumerable.Range(0, canvasSize.Height).Select(_ => new char[canvasSize.Width]).ToArray();
            var borderMask /* [row, col] */ = Enumerable.Range(0, canvasSize.Height).Select(_ => new Border[canvasSize.Width]).ToArray();

            foreach (var (logicalCoordinate, _) in logicalCellsMap)
            {
                RenderCell
                (
                    logicalCellsMap,
                    logicalToPhysicalMap,
                    physicalVerticalBorderWidths,
                    physicalHorizontalBorderHeights,
                    physicalColWidths,
                    physicalRowHeights,
                    ref canvas,
                    ref borderMask,
                    logicalCoordinate
                );
            }

            return CanvasToString(canvas);
        }

        private static void RenderCell
        (
            ReadOnlyLogicalCellsMap logicalCellsMap,
            ReadOnlyLogicalToPhysicalMap logicalToPhysicalMap,
            ImmutableArray<int> physicalVerticalBorderWidths,
            ImmutableArray<int> physicalHorizontalBorderHeights,
            ImmutableArray<int> physicalColWidths,
            ImmutableArray<int> physicalRowHeights,
            ref char[][] canvas,
            ref Border[][] borderMask,
            Coordinate logicalCoordinate
        )
        {
            var logicalCell = logicalCellsMap[logicalCoordinate];
            var margins = GetTableCellMargins
            (
                logicalCellsMap,
                logicalToPhysicalMap,
                physicalVerticalBorderWidths,
                physicalHorizontalBorderHeights,
                physicalColWidths,
                physicalRowHeights,
                logicalCoordinate
            );

            // background
            PaintRectangle(canvas, margins.Area, ' ');

            // text
            PaintText(canvas, margins.Area, logicalCell);

            // border lines
            PaintBorder(canvas, borderMask, margins.LeftBorder, GetVerticalBorderCharacter(logicalCell.Borders.Left), logicalCell.Borders.Left);
            PaintBorder(canvas, borderMask, margins.TopBorder, GetHorizontalBorderCharacter(logicalCell.Borders.Top), logicalCell.Borders.Top);
            PaintBorder(canvas, borderMask, margins.RightBorder, GetVerticalBorderCharacter(logicalCell.Borders.Right), logicalCell.Borders.Right);
            PaintBorder(canvas, borderMask, margins.BottomBorder, GetHorizontalBorderCharacter(logicalCell.Borders.Bottom), logicalCell.Borders.Bottom);

            // border corners
            var topLeftBorder = (Border)Math.Max((int)logicalCell.Borders.Top, (int)logicalCell.Borders.Left);
            var topRightBorder = (Border)Math.Max((int)logicalCell.Borders.Top, (int)logicalCell.Borders.Right);
            var bottomRightBorder = (Border)Math.Max((int)logicalCell.Borders.Bottom, (int)logicalCell.Borders.Right);
            var bottomLeftBorder = (Border)Math.Max((int)logicalCell.Borders.Bottom, (int)logicalCell.Borders.Left);
            PaintBorder(canvas, borderMask, margins.TopLeftBorderCorner, GetBorderCornerCharacter(topLeftBorder), topLeftBorder);
            PaintBorder(canvas, borderMask, margins.TopRightBorderCorner, GetBorderCornerCharacter(topRightBorder), topRightBorder);
            PaintBorder(canvas, borderMask, margins.BottomRightBorderCorner, GetBorderCornerCharacter(bottomRightBorder), bottomRightBorder);
            PaintBorder(canvas, borderMask, margins.BottomLeftBorderCorner, GetBorderCornerCharacter(bottomLeftBorder), bottomLeftBorder);
        }

        private static Coordinate GetTopLeftTableCoordinate
        (
            ImmutableArray<int> physicalVerticalBorderWidths,
            ImmutableArray<int> physicalHorizontalBorderHeights,
            ImmutableArray<int> physicalColWidths,
            ImmutableArray<int> physicalRowHeights,
            Coordinate physicalCoordinate
        )
        {
            int col = 0, row = 0;

            // account for previous borders
            for (var i = 0; i <= physicalCoordinate.Col; i++)
            {
                col += physicalVerticalBorderWidths[i];
            }
            for (var i = 0; i <= physicalCoordinate.Row; i++)
            {
                row += physicalHorizontalBorderHeights[i];
            }

            // account for previous cells
            for (var i = 0; i < physicalCoordinate.Col; i++)
            {
                col += physicalColWidths[i];
            }
            for (var i = 0; i < physicalCoordinate.Row; i++)
            {
                row += physicalRowHeights[i];
            }

            return new Coordinate(row, col);
        }

        private static TableCellMargins GetTableCellMargins
        (
            ReadOnlyLogicalCellsMap logicalCellsMap,
            ReadOnlyLogicalToPhysicalMap logicalToPhysicalMap,
            ImmutableArray<int> physicalVerticalBorderWidths,
            ImmutableArray<int> physicalHorizontalBorderHeights,
            ImmutableArray<int> physicalColWidths,
            ImmutableArray<int> physicalRowHeights,
            Coordinate logicalCoordinate
        )
        {
            // get cell area
            var pCells = logicalToPhysicalMap[logicalCoordinate];
            var topLeftPhysicalCell = pCells
                .OrderBy(x => x.Row)
                .ThenBy(x => x.Col)
                .First();
            var bottomRightPhysicalCell = pCells
                .OrderByDescending(x => x.Row)
                .ThenByDescending(x => x.Col)
                .First();

            var xyTopLeft = GetTopLeftTableCoordinate
            (
                physicalVerticalBorderWidths,
                physicalHorizontalBorderHeights,
                physicalColWidths,
                physicalRowHeights,
                topLeftPhysicalCell
            );
            var xyBottomRight = GetTopLeftTableCoordinate
            (
                physicalVerticalBorderWidths,
                physicalHorizontalBorderHeights,
                physicalColWidths,
                physicalRowHeights,
                bottomRightPhysicalCell
            );
            xyBottomRight =
            (
                xyBottomRight.Row + physicalRowHeights[bottomRightPhysicalCell.Row],
                xyBottomRight.Col + physicalColWidths[bottomRightPhysicalCell.Col]
            );
            var area = new Margin(xyTopLeft.Col, xyTopLeft.Row, xyBottomRight.Col, xyBottomRight.Row);

            // get borders and corners
            var logicalCell = logicalCellsMap[logicalCoordinate];
            var leftBorderSize = PlainTextTable.GetBorderSize(logicalCell.Borders.Left);
            var topBorderSize = PlainTextTable.GetBorderSize(logicalCell.Borders.Top);
            var rightBorderSize = PlainTextTable.GetBorderSize(logicalCell.Borders.Right);
            var bottomBorderSize = PlainTextTable.GetBorderSize(logicalCell.Borders.Bottom);

            var borderLeft = new Margin(area.Left - leftBorderSize, area.Top, area.Left, area.Bottom);
            var borderTopLeft = new Margin(area.Left - leftBorderSize, area.Top - topBorderSize, area.Left, area.Top);
            var borderTop = new Margin(area.Left, area.Top - topBorderSize, area.Right, area.Top);
            var borderTopRight = new Margin(area.Right, area.Top - topBorderSize, area.Right + rightBorderSize, area.Top);
            var borderRight = new Margin(area.Right, area.Top, area.Right + rightBorderSize, area.Bottom);
            var borderBottomRight = new Margin(area.Right, area.Bottom, area.Right + rightBorderSize, area.Bottom + bottomBorderSize);
            var borderBottom = new Margin(area.Left, area.Bottom, area.Right, area.Bottom + bottomBorderSize);
            var borderBottomLeft = new Margin(area.Left - leftBorderSize, area.Bottom, area.Left, area.Bottom + bottomBorderSize);

            return new TableCellMargins
            {
                Area = area,

                LeftBorder = borderLeft,
                TopBorder = borderTop,
                RightBorder = borderRight,
                BottomBorder = borderBottom,

                TopLeftBorderCorner = borderTopLeft,
                TopRightBorderCorner = borderTopRight,
                BottomRightBorderCorner = borderBottomRight,
                BottomLeftBorderCorner = borderBottomLeft,
            };
        }

        private static char GetHorizontalBorderCharacter(Border border) => border switch
        {
            Border.None => ' ',
            Border.Normal => '-',
            Border.Bold => '=',
            _ => throw new ArgumentOutOfRangeException(nameof(border)),
        };

        private static char GetVerticalBorderCharacter(Border border) => border switch
        {
            Border.None => ' ',
            Border.Normal => '|',
            Border.Bold => '@',
            _ => throw new ArgumentOutOfRangeException(nameof(border)),
        };

        private static char GetBorderCornerCharacter(Border border) => border switch
        {
            Border.None => ' ',
            Border.Normal => '+',
            Border.Bold => '#',
            _ => throw new ArgumentOutOfRangeException(nameof(border)),
        };

        private static void PaintRectangle(char[][] canvas, Margin rectangle, char color)
        {
            for (var y = rectangle.Top; y < rectangle.Bottom; y++)
            {
                for (var x = rectangle.Left; x < rectangle.Right; x++)
                {
                    canvas[y][x] = color;
                }
            }
        }

        private static void PaintBorder(char[][] canvas, Border[][] borderMask, Margin rectangle, char color, Border weight)
        {
            for (var y = rectangle.Top; y < rectangle.Bottom; y++)
            {
                for (var x = rectangle.Left; x < rectangle.Right; x++)
                {
                    if (borderMask[y][x] <= weight)
                    {
                        canvas[y][x] = color;
                        borderMask[y][x] = weight;
                    }
                }
            }
        }

        private static void PaintText(char[][] canvas, Margin cellArea, LogicalCell logicalCell)
        {
            var lines = logicalCell.Text.Split(Environment.NewLine);
            if (lines.Length == 0)
            {
                // no text
                return;
            }

            // area in which are allowed to write text
            var writableArea = new Margin
            (
                cellArea.Left + logicalCell.Margin.Left,
                cellArea.Top + logicalCell.Margin.Top,
                cellArea.Right - logicalCell.Margin.Right,
                cellArea.Bottom - logicalCell.Margin.Bottom
            );

            var verticalOffset = logicalCell.VerticalAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Center => (writableArea.Height - lines.Length) / 2,
                VerticalAlignment.Bottom => writableArea.Height - lines.Length,
                _ => throw new ArgumentOutOfRangeException(nameof(logicalCell.VerticalAlignment)),
            };

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var horizontalOffset = logicalCell.HorizontalAlignment switch
                {
                    HorizontalAlignment.Left => 0,
                    HorizontalAlignment.Center => (writableArea.Width - line.Length) / 2,
                    HorizontalAlignment.Right => writableArea.Width - line.Length,
                    _ => throw new ArgumentOutOfRangeException(nameof(logicalCell.HorizontalAlignment)),
                };
                var y = writableArea.Top + i + verticalOffset; // row
                var x = writableArea.Left + horizontalOffset; // col
                for (var c = 0; c < line.Length; c++)
                {
                    canvas[y][x + c] = line[c];
                }
            }
        }
    }
}