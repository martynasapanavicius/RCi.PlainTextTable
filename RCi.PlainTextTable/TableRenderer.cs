using System;
using System.Collections.Generic;
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
            ImmutableArray<int> physicalRowHeights,
            BorderStyle style
        )
        {
            var sumGlobalBordersWidth = physicalVerticalBorderWidths.Length == 0 ? 0 : physicalVerticalBorderWidths.Sum();
            var sumGlobalBordersHeight = physicalHorizontalBorderHeights.Length == 0 ? 0 : physicalHorizontalBorderHeights.Sum();
            var sumGlobalCellsWidth = physicalColWidths.Length == 0 ? 0 : physicalColWidths.Sum();
            var sumGlobalCellsHeight = physicalRowHeights.Length == 0 ? 0 : physicalRowHeights.Sum();
            var canvasSize = new Size(sumGlobalBordersWidth + sumGlobalCellsWidth, sumGlobalBordersHeight + sumGlobalCellsHeight);
            var canvas /* [row, col] */ = Enumerable.Range(0, canvasSize.Height).Select(_ => new char[canvasSize.Width]).ToArray();
            var borderMask /* [row, col] */ = Enumerable.Range(0, canvasSize.Height).Select(_ => new Border[canvasSize.Width]).ToArray();
            var borderCornerCoordinates = new HashSet<Coordinate>();

            // render all cells
            foreach (var logicalCoordinate in logicalCellsMap.Keys.OrderBy(x => x))
            {
                RenderCell
                (
                    logicalCellsMap,
                    logicalToPhysicalMap,
                    physicalVerticalBorderWidths,
                    physicalHorizontalBorderHeights,
                    physicalColWidths,
                    physicalRowHeights,
                    style,
                    ref canvas,
                    ref borderMask,
                    ref borderCornerCoordinates,
                    logicalCoordinate
                );
            }

            // reconcile precise corners
            ReconcileBorderCorners(canvas, borderMask, borderCornerCoordinates, style);

            // build string
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
            BorderStyle style,
            ref char[][] canvas,
            ref Border[][] borderMask,
            ref HashSet<Coordinate> borderCornerCoordinates,
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
            PaintBorder(canvas, borderMask, margins.LeftBorder, GetVerticalBorderCharacter(style, logicalCell.Borders.Left), logicalCell.Borders.Left);
            PaintBorder(canvas, borderMask, margins.TopBorder, GetHorizontalBorderCharacter(style, logicalCell.Borders.Top), logicalCell.Borders.Top);
            PaintBorder(canvas, borderMask, margins.RightBorder, GetVerticalBorderCharacter(style, logicalCell.Borders.Right), logicalCell.Borders.Right);
            PaintBorder(canvas, borderMask, margins.BottomBorder, GetHorizontalBorderCharacter(style, logicalCell.Borders.Bottom), logicalCell.Borders.Bottom);

            // border corners
            var topLeftBorder = (Border)Math.Max((int)logicalCell.Borders.Top, (int)logicalCell.Borders.Left);
            var topRightBorder = (Border)Math.Max((int)logicalCell.Borders.Top, (int)logicalCell.Borders.Right);
            var bottomRightBorder = (Border)Math.Max((int)logicalCell.Borders.Bottom, (int)logicalCell.Borders.Right);
            var bottomLeftBorder = (Border)Math.Max((int)logicalCell.Borders.Bottom, (int)logicalCell.Borders.Left);
            PaintBorderCorner(canvas, borderMask, margins.TopLeftBorderCorner, GetBorderCornerCharacter(style, topLeftBorder), topLeftBorder, ref borderCornerCoordinates);
            PaintBorderCorner(canvas, borderMask, margins.TopRightBorderCorner, GetBorderCornerCharacter(style, topRightBorder), topRightBorder, ref borderCornerCoordinates);
            PaintBorderCorner(canvas, borderMask, margins.BottomRightBorderCorner, GetBorderCornerCharacter(style, bottomRightBorder), bottomRightBorder, ref borderCornerCoordinates);
            PaintBorderCorner(canvas, borderMask, margins.BottomLeftBorderCorner, GetBorderCornerCharacter(style, bottomLeftBorder), bottomLeftBorder, ref borderCornerCoordinates);
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
            var physicalCells = logicalToPhysicalMap[logicalCoordinate];
            var topLeftPhysicalCell = physicalCells
                .OrderBy(x => x)
                .First();
            var bottomRightPhysicalCell = physicalCells
                .OrderByDescending(x => x)
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
            xyBottomRight = new Coordinate
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

            var borderLeft = area with { Left = area.Left - leftBorderSize, Right = area.Left };
            var borderTopLeft = new Margin(area.Left - leftBorderSize, area.Top - topBorderSize, area.Left, area.Top);
            var borderTop = area with { Top = area.Top - topBorderSize, Bottom = area.Top };
            var borderTopRight = new Margin(area.Right, area.Top - topBorderSize, area.Right + rightBorderSize, area.Top);
            var borderRight = area with { Left = area.Right, Right = area.Right + rightBorderSize };
            var borderBottomRight = new Margin(area.Right, area.Bottom, area.Right + rightBorderSize, area.Bottom + bottomBorderSize);
            var borderBottom = area with { Top = area.Bottom, Bottom = area.Bottom + bottomBorderSize };
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
                    if (borderMask[y][x] > weight)
                    {
                        continue;
                    }
                    canvas[y][x] = color;
                    borderMask[y][x] = weight;
                }
            }
        }

        private static void PaintBorderCorner(char[][] canvas, Border[][] borderMask, Margin rectangle, char color, Border weight, ref HashSet<Coordinate> borderCornerCoordinates)
        {
            for (var y = rectangle.Top; y < rectangle.Bottom; y++)
            {
                for (var x = rectangle.Left; x < rectangle.Right; x++)
                {
                    if (borderMask[y][x] > weight)
                    {
                        continue;
                    }
                    canvas[y][x] = color;
                    borderMask[y][x] = weight;
                    borderCornerCoordinates?.Add(new Coordinate(y, x));
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

        private static char GetHorizontalBorderCharacter(BorderStyle style, Border border) => style switch
        {
            BorderStyle.Ascii => border switch
            {
                Border.None => ' ',
                Border.Normal => '-',
                Border.Bold => '=',
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            },
            BorderStyle.UnicodeSingle => border switch
            {
                Border.None => ' ',
                Border.Normal => '\u2500',  // ─
                Border.Bold => '\u2501',    // ━
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            },
            BorderStyle.UnicodeDouble => border switch
            {
                Border.None => ' ',
                Border.Normal => '\u2500',  // ─
                Border.Bold => '\u2550',    // ═
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(style)),
        };

        private static char GetVerticalBorderCharacter(BorderStyle style, Border border) => style switch
        {
            BorderStyle.Ascii => border switch
            {
                Border.None => ' ',
                Border.Normal => '|',
                Border.Bold => '#',
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            },
            BorderStyle.UnicodeSingle => border switch
            {
                Border.None => ' ',
                Border.Normal => '\u2502',  // │
                Border.Bold => '\u2503',    // ┃
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            },
            BorderStyle.UnicodeDouble => border switch
            {
                Border.None => ' ',
                Border.Normal => '\u2502',  // │
                Border.Bold => '\u2551',    // ║
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(style)),
        };

        private static char GetBorderCornerCharacter(BorderStyle style, Border border) => style switch
        {
            BorderStyle.Ascii => border switch
            {
                Border.None => ' ',
                Border.Normal => '+',
                Border.Bold => '#',
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            },
            BorderStyle.UnicodeSingle => border switch
            {
                Border.None => ' ',
                Border.Normal => '\u253c',  // ┼
                Border.Bold => '\u254b',    // ╋
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            },
            BorderStyle.UnicodeDouble => border switch
            {
                Border.None => ' ',
                Border.Normal => '\u253c',  // ┼
                Border.Bold => '\u256c',    // ╬
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(style)),
        };

        private static Border GetBorder(Border[][] borderMask, Coordinate coordinate)
        {
            if (coordinate.Row < 0 || coordinate.Row >= borderMask.Length)
            {
                return Border.None;
            }
            if (coordinate.Col < 0 || coordinate.Col >= borderMask[coordinate.Row].Length)
            {
                return Border.None;
            }
            return borderMask[coordinate.Row][coordinate.Col];
        }

        private static void ReconcileBorderCorners(char[][] canvas, Border[][] borderMask, IReadOnlySet<Coordinate> cornerCoordinates, BorderStyle style)
        {
            if (style == BorderStyle.Ascii)
            {
                // ascii does not need it
                return;
            }

            foreach (var coordinate in cornerCoordinates.OrderBy(x => x))
            {
                char newCharacter;
                var center = GetBorder(borderMask, coordinate);
                if (center == Border.None)
                {
                    newCharacter = ' ';
                }
                else
                {
                    var left = GetBorder(borderMask, coordinate.MoveLeft());
                    var top = GetBorder(borderMask, coordinate.MoveUp());
                    var right = GetBorder(borderMask, coordinate.MoveRight());
                    var bottom = GetBorder(borderMask, coordinate.MoveDown());
                    newCharacter = style switch
                    {
                        BorderStyle.UnicodeSingle => ReconcileSingle(left, top, right, bottom),
                        BorderStyle.UnicodeDouble => throw new NotImplementedException(), // TODO:
                        _ => throw new ArgumentOutOfRangeException(nameof(style)),
                    };
                }
                canvas[coordinate.Row][coordinate.Col] = newCharacter;
            }
        }

        private static char ReconcileSingle(Border left, Border top, Border right, Border bottom) => left switch
        {
            Border.None => top switch
            {
                Border.None => right switch
                {
                    Border.None => bottom switch
                    {
                        Border.None => ' ',         // O O O O
                        Border.Normal => '\u2577',  // O O O N ╷
                        Border.Bold => '\u257b',    // O O O B ╻
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Normal => bottom switch
                    {
                        Border.None => '\u2576',    // O O N O ╶
                        Border.Normal => '\u250c',  // O O N N ┌
                        Border.Bold => '\u250e',    // O O N B ┎
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Bold => bottom switch
                    {
                        Border.None => '\u257a',    // O O B O ╺
                        Border.Normal => '\u250d',  // O O B N ┍
                        Border.Bold => '\u250f',    // O O B B ┏
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(right)),
                },
                Border.Normal => right switch
                {
                    Border.None => bottom switch
                    {
                        Border.None => '\u2575',    // O N O O ╵
                        Border.Normal => '\u2502',  // O N O N │
                        Border.Bold => '\u257d',    // O N O B ╽
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Normal => bottom switch
                    {
                        Border.None => '\u2514',    // O N N O └
                        Border.Normal => '\u251c',  // O N N N ├
                        Border.Bold => '\u251f',    // O N N B ┟
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Bold => bottom switch
                    {
                        Border.None => '\u2515',    // O N B O ┕
                        Border.Normal => '\u251d',  // O N B N ┝
                        Border.Bold => '\u2522',    // O N B B ┢
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(right)),
                },
                Border.Bold => right switch
                {
                    Border.None => bottom switch
                    {
                        Border.None => '\u2579',    // O B O O ╹
                        Border.Normal => '\u257f',  // O B O N ╿
                        Border.Bold => '\u2503',    // O B O B ┃
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Normal => bottom switch
                    {
                        Border.None => '\u2516',    // O B N O ┖
                        Border.Normal => '\u251e',  // O B N N ┞
                        Border.Bold => '\u2520',    // O B N B ┠
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Bold => bottom switch
                    {
                        Border.None => '\u2517',    // O B B O ┗
                        Border.Normal => '\u2521',  // O B B N ┡
                        Border.Bold => '\u2523',    // O B B B ┣
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(right)),
                },
                _ => throw new ArgumentOutOfRangeException(nameof(top)),
            },
            Border.Normal => top switch
            {
                Border.None => right switch
                {
                    Border.None => bottom switch
                    {
                        Border.None => '\u2574',    // N O O O ╴
                        Border.Normal => '\u2510',  // N O O N ┐
                        Border.Bold => '\u2512',    // N O O B ┒
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Normal => bottom switch
                    {
                        Border.None => '\u2500',    // N O N O ─
                        Border.Normal => '\u252c',  // N O N N ┬
                        Border.Bold => '\u2530',    // N O N B ┰
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Bold => bottom switch
                    {
                        Border.None => '\u257c',    // N O B O ╼
                        Border.Normal => '\u252e',  // N O B N ┮
                        Border.Bold => '\u2532',    // N O B B ┲
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(right)),
                },
                Border.Normal => right switch
                {
                    Border.None => bottom switch
                    {
                        Border.None => '\u2518',    // N N O O ┘
                        Border.Normal => '\u2524',  // N N O N ┤
                        Border.Bold => '\u2527',    // N N O B ┧
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Normal => bottom switch
                    {
                        Border.None => '\u2534',    // N N N O ┴
                        Border.Normal => '\u253c',  // N N N N ┼
                        Border.Bold => '\u2541',    // N N N B ╁
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Bold => bottom switch
                    {
                        Border.None => '\u2536',    // N N B O ┶
                        Border.Normal => '\u253e',  // N N B N ┾
                        Border.Bold => '\u2546',    // N N B B ╆
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(right)),
                },
                Border.Bold => right switch
                {
                    Border.None => bottom switch
                    {
                        Border.None => '\u251a',    // N B O O ┚
                        Border.Normal => '\u2526',  // N B O N ┦
                        Border.Bold => '\u2528',    // N B O B ┨
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Normal => bottom switch
                    {
                        Border.None => '\u2538',    // N B N O ┸
                        Border.Normal => '\u2540',  // N B N N ╀
                        Border.Bold => '\u2542',    // N B N B ╂
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Bold => bottom switch
                    {
                        Border.None => '\u253a',    // N B B O ┺
                        Border.Normal => '\u2544',  // N B B N ╄
                        Border.Bold => '\u254a',    // N B B B ╊
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(right)),
                },
                _ => throw new ArgumentOutOfRangeException(nameof(top)),
            },
            Border.Bold => top switch
            {
                Border.None => right switch
                {
                    Border.None => bottom switch
                    {
                        Border.None => '\u2578',    // B O O O ╸
                        Border.Normal => '\u2511',  // B O O N ┑
                        Border.Bold => '\u2513',    // B O O B ┓
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Normal => bottom switch
                    {
                        Border.None => '\u257e',    // B O N O ╾
                        Border.Normal => '\u252d',  // B O N N ┭
                        Border.Bold => '\u2531',    // B O N B ┱
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Bold => bottom switch
                    {
                        Border.None => '\u2501',    // B O B O ━
                        Border.Normal => '\u252f',  // B O B N ┯
                        Border.Bold => '\u2533',    // B O B B ┳
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(right)),
                },
                Border.Normal => right switch
                {
                    Border.None => bottom switch
                    {
                        Border.None => '\u2501',    // B N O O ━
                        Border.Normal => '\u2525',  // B N O N ┥
                        Border.Bold => '\u252a',    // B N O B ┪
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Normal => bottom switch
                    {
                        Border.None => '\u2535',    // B N N O ┵
                        Border.Normal => '\u253d',  // B N N N ┽
                        Border.Bold => '\u2545',    // B N N B ╅
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Bold => bottom switch
                    {
                        Border.None => '\u2537',    // B N B O ┷
                        Border.Normal => '\u253f',  // B N B N ┿
                        Border.Bold => '\u2548',    // B N B B ╈
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(right)),
                },
                Border.Bold => right switch
                {
                    Border.None => bottom switch
                    {
                        Border.None => '\u251b',    // B B O O ┛
                        Border.Normal => '\u2529',  // B B O N ┩
                        Border.Bold => '\u252b',    // B B O B ┫
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Normal => bottom switch
                    {
                        Border.None => '\u2539',    // B B N O ┹
                        Border.Normal => '\u2543',  // B B N N ╃
                        Border.Bold => '\u2549',    // B B N B ╉
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    Border.Bold => bottom switch
                    {
                        Border.None => '\u253b',    // B B B O ┻
                        Border.Normal => '\u2547',  // B B B N ╇
                        Border.Bold => '\u254b',    // B B B B ╋
                        _ => throw new ArgumentOutOfRangeException(nameof(bottom)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(right)),
                },
                _ => throw new ArgumentOutOfRangeException(nameof(top)),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(left)),
        };
    }
}
