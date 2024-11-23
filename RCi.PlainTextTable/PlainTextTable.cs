using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Coordinate = (int row, int col);
using Size = (int width, int height);
using LogicalCellsMap = System.Collections.Generic.IDictionary<(int row, int col) /* logical coordinate */, RCi.PlainTextTable.PttLogicalCell>;
using LogicalToPhysicalMap = System.Collections.Generic.IDictionary<(int row, int col) /* logical coordinate */, System.Collections.Generic.HashSet<(int row, int col) /* physical coordinate */>>;
using PhysicalToLogicalMap = System.Collections.Generic.IDictionary<(int row, int col) /* physical coordinate */, (int row, int col) /* physical coordinate */>;

namespace RCi.PlainTextTable
{
    public static class PttExtensions
    {
        public static IPttCell Text(this IPttCell cell, string text)
        {
            cell.Text = text;
            return cell;
        }

        public static IPttCell ColumnSpan(this IPttCell cell, int span)
        {
            cell.ColumnSpan = span;
            return cell;
        }

        public static IPttCell RowSpan(this IPttCell cell, int span)
        {
            cell.RowSpan = span;
            return cell;
        }

        public static PttRowControl Row(this Ptt ptt, int row) => new(ptt, row);

        public static PttColumnControl Column(this Ptt ptt, int col) => new(ptt, col);
    }

    public enum PttHorizontalAlignment { Left, Center, Right, }

    public enum PttVerticalAlignment { Top, Center, Bottom, }

    public enum PttBorder { None, Normal, Bold, }

    public readonly record struct PttMargin(int Left, int Top, int Right, int Bottom)
    {
        public PttMargin(int horizontal, int vertical) :
            this(horizontal, vertical, horizontal, vertical)
        {
        }

        public PttMargin(int universal) :
            this(universal, universal, universal, universal)
        {
        }

        public int Width => Right - Left;
        public int Height => Bottom - Top;

        public static implicit operator PttMargin(int universal) =>
            new(universal);

        public static implicit operator PttMargin((int horizontal, int vertical) tuple) =>
            new(tuple.horizontal, tuple.vertical);

        public static implicit operator PttMargin((int left, int top, int right, int bottom) tuple) =>
            new(tuple.left, tuple.top, tuple.right, tuple.bottom);

        public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
    }

    public readonly record struct PttBorders(PttBorder Left, PttBorder Top, PttBorder Right, PttBorder Bottom)
    {
        public PttBorders(PttBorder universalHorizontal, PttBorder universalVertical) :
            this(universalHorizontal, universalVertical, universalHorizontal, universalVertical)
        {
        }

        public PttBorders(PttBorder universal) :
            this(universal, universal, universal, universal)
        {
        }

        public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
    }

    public interface IPttCell
    {
        Coordinate Coordinate { get; }
        string Text { get; set; }
        int ColumnSpan { get; set; }
        int RowSpan { get; set; }
        PttMargin? Margin { get; set; }
        PttBorders? Borders { get; set; }
        PttHorizontalAlignment? HorizontalAlignment { get; set; }
        PttVerticalAlignment? VerticalAlignment { get; set; }

        Ptt Host();
        void Delete();
    }

    internal sealed class PttCell :
        IPttCell
    {
        public required Ptt Host { get; init; }
        public required Coordinate Coordinate { get; init; }
        public string Text { get; set; } = string.Empty;
        public int ColumnSpan { get; set; } = 1;
        public int RowSpan { get; set; } = 1;
        public PttMargin? Margin { get; set; }
        public PttBorders? Borders { get; set; }
        public PttHorizontalAlignment? HorizontalAlignment { get; set; }
        public PttVerticalAlignment? VerticalAlignment { get; set; }

        Ptt IPttCell.Host() => Host;
        public void Delete() => Host.DeleteCell(this);
    }

    public readonly struct PttRowControl(Ptt host, int row)
    {
        public IPttCell this[int col] => host[row, col];
        public Ptt Host() => host;

        public PttRowControl Text(params string[] texts)
        {
            for (var i = 0; i < texts.Length; i++)
            {
                host[row, i].Text(texts[i]);
            }
            return this;
        }

        public PttRowControl Text(string text)
        {
            for (var i = 0; i < host.ColumnCount; i++)
            {
                host[row, i].Text(text);
            }
            return this;
        }
    }

    public readonly struct PttColumnControl(Ptt host, int col)
    {
        public IPttCell this[int row] => host[row, col];
        public Ptt Host() => host;

        public PttColumnControl Text(params string[] texts)
        {
            for (var i = 0; i < texts.Length; i++)
            {
                host[i, col].Text(texts[i]);
            }
            return this;
        }

        public PttColumnControl Text(string text)
        {
            for (var i = 0; i < host.RowCount; i++)
            {
                host[i, col].Text(text);
            }
            return this;
        }
    }

    internal sealed record PttLogicalCell
    {
        public required Coordinate Coordinate { get; init; }
        public int Col => Coordinate.col;
        public int Row => Coordinate.row;
        public required string Text { get; init; }
        public required int ColumnSpan { get; init; }
        public required int RowSpan { get; init; }
        public required PttMargin Margin { get; init; }
        public required PttBorders Borders { get; init; }
        public required PttHorizontalAlignment HorizontalAlignment { get; init; }
        public required PttVerticalAlignment VerticalAlignment { get; init; }

        private Size? _textSize;
        public Size TextSize => _textSize ??= GetTextSize();

        private Size? _textSizeWithMargin;
        public Size TextSizeWithMargin => _textSizeWithMargin ??= GetTextSizeWithMargin();

        private Size GetTextSize()
        {
            var lines = Text.Split(Environment.NewLine);
            var width = lines.Length == 0 ? 0 : lines.Max(l => l.Length);
            var height = lines.Length;
            return (width, height);
        }

        private Size GetTextSizeWithMargin()
        {
            return
            (
                Margin.Left + TextSize.width + Margin.Right,
                Margin.Top + TextSize.height + Margin.Bottom
            );
        }

        public override string ToString() => $"{Coordinate}, CS={ColumnSpan}, RS={RowSpan}, M=({Margin}), TS={TextSize}, TSWM={TextSizeWithMargin}, HA={HorizontalAlignment.ToString()[..1]}, VA={VerticalAlignment.ToString()[..1]}, B=({Borders})";
    }

    internal sealed record PttTableCellMargins
    {
        public required PttMargin Area { get; init; }

        // borders
        public required PttMargin LeftBorder { get; init; }
        public required PttMargin TopBorder { get; init; }
        public required PttMargin RightBorder { get; init; }
        public required PttMargin BottomBorder { get; init; }

        // corners
        public required PttMargin TopLeftBorderCorner { get; init; }
        public required PttMargin TopRightBorderCorner { get; init; }
        public required PttMargin BottomRightBorderCorner { get; init; }
        public required PttMargin BottomLeftBorderCorner { get; init; }
    }

    public sealed class Ptt
    {
        private readonly Dictionary<Coordinate, PttCell> _cells = new();
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public PttMargin DefaultMargin { get; set; } = new(1, 0);
        public PttBorders DefaultBorders { get; set; } = new(PttBorder.Normal);
        public PttHorizontalAlignment DefaultHorizontalAlignment { get; set; } = PttHorizontalAlignment.Left;
        public PttVerticalAlignment DefaultVerticalAlignment { get; set; } = PttVerticalAlignment.Top;
        public IPttCell this[Coordinate c]
        {
            get
            {
                if (!_cells.TryGetValue(c, out var cell))
                {
                    cell = new PttCell
                    {
                        Host = this,
                        Coordinate = c,
                    };
                    _cells.Add(c, cell);
                    RowCount = Math.Max(RowCount, c.row + 1);
                    ColumnCount = Math.Max(ColumnCount, c.col + 1);
                }
                return cell;
            }
        }
        public IPttCell this[int row, int col] => this[(row, col)];

        public PttRowControl AppendRow()
        {
            var row = RowCount;
            RowCount++;
            return new PttRowControl(this, row);
        }

        public PttColumnControl AppendColumn()
        {
            var col = ColumnCount;
            ColumnCount++;
            return new PttColumnControl(this, col);
        }

        internal void DeleteCell(PttCell cell)
        {
            _cells.Remove(cell.Coordinate);
            RowCount = _cells.Count == 0 ? 0 : _cells.Max(p => p.Key.row) + 1;
            ColumnCount = _cells.Count == 0 ? 0 : _cells.Max(p => p.Key.col) + 1;
        }

        public override string ToString() => CompileToText
        (
            //_cells.ToDictionary(p => p.Key, p => new PttLogicalCell
            //{
            //    Coordinate = p.Value.Coordinate,
            //    Text = p.Value.Text,
            //    ColumnSpan = p.Value.ColumnSpan,
            //    RowSpan = p.Value.RowSpan,
            //    Margin = p.Value.Margin ?? DefaultMargin,
            //    Borders = p.Value.Borders ?? DefaultBorders,
            //    HorizontalAlignment = p.Value.HorizontalAlignment ?? DefaultHorizontalAlignment,
            //    VerticalAlignment = p.Value.VerticalAlignment ?? DefaultVerticalAlignment,
            //})
            _cells.Select(p => new PttLogicalCell
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

        private static string CompileToText(IEnumerable<PttLogicalCell> logicalCellsStream)
        {
            var logicalCellsMap = logicalCellsStream.ToDictionary(c => c.Coordinate, c => c);

            // build physical grid
            BuildPhysicalGrid(logicalCellsMap, out var logicalToPhysicalMap, out var physicalToLogicalMap);

            // there might be empty rows with no original cell spawns, we can merge those
            MergeTransientEmptyPhysicalColsAndRows(logicalCellsMap, ref logicalToPhysicalMap, ref physicalToLogicalMap);

            // build physical borders map
            BuildPhysicalBorders(logicalCellsMap, logicalToPhysicalMap, out var verticalPhysicalCellBordersMap, out var horizontalPhysicalCellBordersMap);

            // ------

            // get max sizes for global physical borders
            var physicalVerticalBorderWidths = verticalPhysicalCellBordersMap
                .GroupBy(p => p.Key.col)
                .Select(g => (col: g.Key, border: g.Max(p => p.Value)))
                .OrderBy(t => t.col)
                .Select(t => GetBorderSize(t.border))
                .ToImmutableArray();
            var physicalHorizontalBorderHeights = horizontalPhysicalCellBordersMap
                .GroupBy(p => p.Key.row)
                .Select(g => (row: g.Key, border: g.Max(p => p.Value)))
                .OrderBy(t => t.row)
                .Select(t => GetBorderSize(t.border))
                .ToImmutableArray();
            var physicalColWidths = new int[logicalToPhysicalMap.Count == 0 ? 0 : logicalToPhysicalMap.Values.Max(x => x.Max(c => c.col)) + 1];
            var physicalRowHeights = new int[logicalToPhysicalMap.Count == 0 ? 0 : logicalToPhysicalMap.Values.Max(x => x.Max(c => c.row)) + 1];

            GrowPhysicalCols();
            GrowPhysicalRows();

            void GrowPhysicalCols()
            {
                foreach (var logicalCell in logicalCellsMap.Values
                             .OrderBy(x => x.Row)
                             .ThenBy(x => x.Col))
                {
                    // get physical columns which this logical cell interacts with
                    var physicalCols = logicalToPhysicalMap[logicalCell.Coordinate]
                        .Select(x => x.col)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToImmutableArray();
                    var minPhysicalCol = physicalCols[0];
                    var maxPhysicalCol = physicalCols[^1];
                    var localColDistribution = physicalCols.ToDictionary(x => x, _ => 0);

                    // distribute height across columns and borders
                    var widthBudget = logicalCell.TextSizeWithMargin.width;
                    var firstRound = true;

                    // start with column, then flip to border, then flip to column again and repeat flipping
                    (int? col, int? border) current = (maxPhysicalCol, default);
                    while (widthBudget > 0)
                    {
                        if (current.col is not null)
                        {
                            // distribute to col
                            var physicalCol = current.col.Value;
                            localColDistribution[physicalCol]++;
                            physicalColWidths[physicalCol] = Math.Max(physicalColWidths[physicalCol], localColDistribution[physicalCol]);
                            widthBudget--;
                            if (physicalCol == minPhysicalCol)
                            {
                                // we are at the last column, flip back to first column
                                current = (maxPhysicalCol, default);
                                firstRound = false;
                            }
                            else
                            {
                                // flip to border
                                current = (default, physicalCol);
                            }
                        }
                        else
                        {
                            var physicalBorder = current.border!.Value;
                            if (firstRound)
                            {
                                // distribute to border
                                widthBudget -= physicalVerticalBorderWidths[physicalBorder];
                            }
                            // flip to row
                            current = (physicalBorder - 1, default);
                        }
                    }
                }
            }

            void GrowPhysicalRows()
            {
                foreach (var logicalCell in logicalCellsMap.Values
                             .OrderBy(x => x.Row)
                             .ThenBy(x => x.Col))
                {
                    // get physical rows which this logical cell interacts with
                    var physicalRows = logicalToPhysicalMap[logicalCell.Coordinate]
                        .Select(x => x.row)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToImmutableArray();
                    var minPhysicalRow = physicalRows[0];
                    var maxPhysicalRow = physicalRows[^1];
                    var localRowDistribution = physicalRows.ToDictionary(x => x, _ => 0);

                    // distribute height across rows and borders
                    var heightBudget = logicalCell.TextSizeWithMargin.height;
                    var firstRound = true;

                    // start with row, then flip to border, then flip to row again and repeat flipping
                    (int? row, int? border) current = (maxPhysicalRow, default);
                    while (heightBudget > 0)
                    {
                        if (current.row is not null)
                        {
                            // distribute to row
                            var physicalRow = current.row.Value;
                            localRowDistribution[physicalRow]++;
                            physicalRowHeights[physicalRow] = Math.Max(physicalRowHeights[physicalRow], localRowDistribution[physicalRow]);
                            heightBudget--;
                            if (physicalRow == minPhysicalRow)
                            {
                                // we are at the last row, flip back to first row
                                current = (maxPhysicalRow, default);
                                firstRound = false;
                            }
                            else
                            {
                                // flip to border
                                current = (default, physicalRow);
                            }
                        }
                        else
                        {
                            // distribute to border
                            var physicalBorder = current.border!.Value;
                            if (firstRound)
                            {
                                heightBudget -= physicalHorizontalBorderHeights[physicalBorder];
                            }
                            // flip to row
                            current = (physicalBorder - 1, default);
                        }
                    }
                }
            }

            // ------

            // render
            var sumGlobalBordersWidth = physicalVerticalBorderWidths.Length == 0 ? 0 : physicalVerticalBorderWidths.Sum();
            var sumGlobalBordersHeight = physicalHorizontalBorderHeights.Length == 0 ? 0 : physicalHorizontalBorderHeights.Sum();
            var sumGlobalCellsWidth = physicalColWidths.Length == 0 ? 0 : physicalColWidths.Sum();
            var sumGlobalCellsHeight = physicalRowHeights.Length == 0 ? 0 : physicalRowHeights.Sum();
            var canvasSize = new Size(sumGlobalBordersWidth + sumGlobalCellsWidth, sumGlobalBordersHeight + sumGlobalCellsHeight);
            var canvas /* [row, col] */ = Enumerable.Range(0, canvasSize.height).Select(_ => new char[canvasSize.width]).ToArray();
            var borderMask /* [row, col] */ = Enumerable.Range(0, canvasSize.height).Select(_ => new PttBorder[canvasSize.width]).ToArray();

            foreach (var (logicalCoordinate, _) in logicalCellsMap)
            {
                RenderCell(logicalCoordinate);
            }

            // print table
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

            // ---------

            Coordinate GetTopLeftTableCoordinate(Coordinate physicalCoordinate)
            {
                int x = 0, y = 0;

                // account for previous borders
                for (var i = 0; i <= physicalCoordinate.col; i++)
                {
                    x += physicalVerticalBorderWidths[i];
                }
                for (var i = 0; i <= physicalCoordinate.row; i++)
                {
                    y += physicalHorizontalBorderHeights[i];
                }
                // account for previous cells
                for (var i = 0; i < physicalCoordinate.col; i++)
                {
                    x += physicalColWidths[i];
                }
                for (var i = 0; i < physicalCoordinate.row; i++)
                {
                    y += physicalRowHeights[i];
                }

                return (y, x); // row = y, col = x
            }

            PttTableCellMargins GetTableCellMargins(Coordinate logicalCoordinate)
            {
                // get cell area
                var pCells = logicalToPhysicalMap[logicalCoordinate];
                var topLeftPhysicalCell = pCells
                    .OrderBy(x => x.row)
                    .ThenBy(x => x.col)
                    .First();
                var bottomRightPhysicalCell = pCells
                    .OrderByDescending(x => x.row)
                    .ThenByDescending(x => x.col)
                    .First();
                var xyTopLeft = GetTopLeftTableCoordinate(topLeftPhysicalCell);
                var xyBottomRight = GetTopLeftTableCoordinate(bottomRightPhysicalCell);
                xyBottomRight =
                (
                    xyBottomRight.row + physicalRowHeights[bottomRightPhysicalCell.row],
                    xyBottomRight.col + physicalColWidths[bottomRightPhysicalCell.col]
                );
                var area = new PttMargin(xyTopLeft.col, xyTopLeft.row, xyBottomRight.col, xyBottomRight.row);

                // get borders and corners
                var logicalCell = logicalCellsMap[logicalCoordinate];
                var leftBorderSize = GetBorderSize(logicalCell.Borders.Left);
                var topBorderSize = GetBorderSize(logicalCell.Borders.Top);
                var rightBorderSize = GetBorderSize(logicalCell.Borders.Right);
                var bottomBorderSize = GetBorderSize(logicalCell.Borders.Bottom);

                var borderLeft = new PttMargin(area.Left - leftBorderSize, area.Top, area.Left, area.Bottom);
                var borderTopLeft = new PttMargin(area.Left - leftBorderSize, area.Top - topBorderSize, area.Left, area.Top);
                var borderTop = new PttMargin(area.Left, area.Top - topBorderSize, area.Right, area.Top);
                var borderTopRight = new PttMargin(area.Right, area.Top - topBorderSize, area.Right + rightBorderSize, area.Top);
                var borderRight = new PttMargin(area.Right, area.Top, area.Right + rightBorderSize, area.Bottom);
                var borderBottomRight = new PttMargin(area.Right, area.Bottom, area.Right + rightBorderSize, area.Bottom + bottomBorderSize);
                var borderBottom = new PttMargin(area.Left, area.Bottom, area.Right, area.Bottom + bottomBorderSize);
                var borderBottomLeft = new PttMargin(area.Left - leftBorderSize, area.Bottom, area.Left, area.Bottom + bottomBorderSize);

                return new PttTableCellMargins
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

            static char GetHorizontalBorderCharacter(PttBorder border) => border switch
            {
                PttBorder.None => ' ',
                PttBorder.Normal => '-',
                PttBorder.Bold => '=',
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            };

            static char GetVerticalBorderCharacter(PttBorder border) => border switch
            {
                PttBorder.None => ' ',
                PttBorder.Normal => '|',
                PttBorder.Bold => '@',
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            };

            static char GetBorderCornerCharacter(PttBorder border) => border switch
            {
                PttBorder.None => ' ',
                PttBorder.Normal => '+',
                PttBorder.Bold => '#',
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            };

            static void PaintRectangle(char[][] canvas, PttMargin rectangle, char color)
            {
                for (var y = rectangle.Top; y < rectangle.Bottom; y++)
                {
                    for (var x = rectangle.Left; x < rectangle.Right; x++)
                    {
                        canvas[y][x] = color;
                    }
                }
            }

            static void PaintBorder(char[][] canvas, PttBorder[][] borderMask, PttMargin rectangle, char color, PttBorder weight)
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

            static void PaintText(char[][] canvas, PttMargin cellArea, PttLogicalCell logicalCell)
            {
                var lines = logicalCell.Text.Split(Environment.NewLine);
                if (lines.Length == 0)
                {
                    // no text
                    return;
                }

                // area in which are allowed to write text
                var writableArea = new PttMargin
                (
                    cellArea.Left + logicalCell.Margin.Left,
                    cellArea.Top + logicalCell.Margin.Top,
                    cellArea.Right - logicalCell.Margin.Right,
                    cellArea.Bottom - logicalCell.Margin.Bottom
                );

                var verticalOffset = logicalCell.VerticalAlignment switch
                {
                    PttVerticalAlignment.Top => 0,
                    PttVerticalAlignment.Center => (writableArea.Height - lines.Length) / 2,
                    PttVerticalAlignment.Bottom => writableArea.Height - lines.Length,
                    _ => throw new ArgumentOutOfRangeException(nameof(logicalCell.VerticalAlignment)),
                };

                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var horizontalOffset = logicalCell.HorizontalAlignment switch
                    {
                        PttHorizontalAlignment.Left => 0,
                        PttHorizontalAlignment.Center => (writableArea.Width - line.Length) / 2,
                        PttHorizontalAlignment.Right => writableArea.Width - line.Length,
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

            void RenderCell(Coordinate logicalCoordinate)
            {
                var logicalCell = logicalCellsMap[logicalCoordinate];
                var m = GetTableCellMargins(logicalCoordinate);

                // main area and text
                PaintRectangle(canvas, m.Area, ' ');
                PaintText(canvas, m.Area, logicalCell);

                // border lines
                PaintBorder(canvas, borderMask, m.LeftBorder, GetVerticalBorderCharacter(logicalCell.Borders.Left), logicalCell.Borders.Left);
                PaintBorder(canvas, borderMask, m.TopBorder, GetHorizontalBorderCharacter(logicalCell.Borders.Top), logicalCell.Borders.Top);
                PaintBorder(canvas, borderMask, m.RightBorder, GetVerticalBorderCharacter(logicalCell.Borders.Right), logicalCell.Borders.Right);
                PaintBorder(canvas, borderMask, m.BottomBorder, GetHorizontalBorderCharacter(logicalCell.Borders.Bottom), logicalCell.Borders.Bottom);

                // border corners
                var topLeftBorder = (PttBorder)Math.Max((int)logicalCell.Borders.Top, (int)logicalCell.Borders.Left);
                var topRightBorder = (PttBorder)Math.Max((int)logicalCell.Borders.Top, (int)logicalCell.Borders.Right);
                var bottomRightBorder = (PttBorder)Math.Max((int)logicalCell.Borders.Bottom, (int)logicalCell.Borders.Right);
                var bottomLeftBorder = (PttBorder)Math.Max((int)logicalCell.Borders.Bottom, (int)logicalCell.Borders.Left);
                PaintBorder(canvas, borderMask, m.TopLeftBorderCorner, GetBorderCornerCharacter(topLeftBorder), topLeftBorder);
                PaintBorder(canvas, borderMask, m.TopRightBorderCorner, GetBorderCornerCharacter(topRightBorder), topRightBorder);
                PaintBorder(canvas, borderMask, m.BottomRightBorderCorner, GetBorderCornerCharacter(bottomRightBorder), bottomRightBorder);
                PaintBorder(canvas, borderMask, m.BottomLeftBorderCorner, GetBorderCornerCharacter(bottomLeftBorder), bottomLeftBorder);
            }

            #region // BuildPhysicalGrid

            static void BuildPhysicalGrid
            (
                IReadOnlyDictionary<Coordinate, PttLogicalCell> logicalCells,
                out LogicalToPhysicalMap logicalToPhysicalMap,
                out PhysicalToLogicalMap physicalToLogicalMap
            )
            {
                // construct physical grid
                logicalToPhysicalMap = new SortedDictionary<Coordinate, HashSet<Coordinate>>(); // TODO: SortedDictionary
                physicalToLogicalMap = new SortedDictionary<Coordinate, Coordinate>();          // TODO: SortedDictionary // TODO: are we okay to only have 1:1 mapping?

                // helper set for finding next available column in a row
                var physicalCoordinatesSet = new HashSet<Coordinate>();

                // since we might have empty logical rows, ignore them in physical mapping
                var logicalToPhysicalRowMap = logicalCells
                    .Values
                    .Select(p => p.Row)
                    .Distinct()
                    .OrderBy(x => x)
                    .Select((n, i) => (logicalRow: n, physicalRow: i))
                    .ToDictionary(t => t.logicalRow, t => t.physicalRow);
                foreach (var logicalCell in logicalCells.Values
                             .OrderBy(x => x.Row)
                             .ThenBy(x => x.Col))
                {
                    var physicalCoordinates = new HashSet<Coordinate>();
                    logicalToPhysicalMap.Add(logicalCell.Coordinate, physicalCoordinates);

                    // find next available row
                    var topLeftPhysicalRow = logicalToPhysicalRowMap[logicalCell.Row];

                    // find next available column in this row
                    var topLeftPhysicalCol = GetNextAvailableColumn(physicalCoordinatesSet, topLeftPhysicalRow);

                    // spread vertically (down)
                    for (var y = 0; y < logicalCell.RowSpan; y++)
                    {
                        // spread horizontally (right)
                        for (var x = 0; x < logicalCell.ColumnSpan; x++)
                        {
                            var physicalCoordinate = new Coordinate(topLeftPhysicalRow + y, topLeftPhysicalCol + x);
                            physicalCoordinatesSet.Add(physicalCoordinate); // mark that this cell is created
                            physicalCoordinates.Add(physicalCoordinate);    // add to logical -> physical mapping

                            // TODO: are we okay to only have 1:1 mapping?
                            //physicalToLogicalMap.Add(physicalCoordinate, logicalCell.Coordinate);
                            physicalToLogicalMap[physicalCoordinate] = logicalCell.Coordinate;
                        }
                    }
                }

                return;

                static int GetNextAvailableColumn(HashSet<Coordinate> physicalCoordinatesSet, int row)
                {
                    var expected = 0;
                    var stream = physicalCoordinatesSet
                        .Where(x => x.row == row)
                        .OrderBy(x => x.col);
                    foreach (var pair in stream)
                    {
                        if (pair.col != expected)
                        {
                            return expected;
                        }
                        expected++;
                    }
                    return expected;
                }
            }

            static Coordinate GetLogicalCellTopLeftPhysicalCoordinate(LogicalToPhysicalMap logicalToPhysicalMap, Coordinate logicalCoordinate) =>
                logicalToPhysicalMap[logicalCoordinate]
                    .OrderBy(x => x.row)
                    .ThenBy(x => x.col)
                    .First();

            static Coordinate GetLogicalCellBottomRightPhysicalCoordinate(LogicalToPhysicalMap logicalToPhysicalMap, Coordinate logicalCoordinate) =>
                logicalToPhysicalMap[logicalCoordinate]
                    .OrderByDescending(x => x.row)
                    .ThenByDescending(x => x.col)
                    .First();

            #endregion

            #region // MergeTransientEmptyPhysicalColsAndRows

            static void MergeTransientEmptyPhysicalColsAndRows
            (
                LogicalCellsMap logicalCellsMap,
                ref LogicalToPhysicalMap logicalToPhysicalMap,
                ref PhysicalToLogicalMap physicalToLogicalMap
            )
            {
                var physicalColsToValidate = physicalToLogicalMap
                    .Select(p => p.Key.col)
                    .Distinct()
                    .OrderByDescending(x => x)
                    .ToImmutableArray();
                foreach (var physicalCol in physicalColsToValidate)
                {
                    MergeCol(logicalCellsMap, ref logicalToPhysicalMap, ref physicalToLogicalMap, physicalCol);
                }

                var physicalRowsToValidate = physicalToLogicalMap
                    .Select(p => p.Key.row)
                    .Distinct()
                    .OrderByDescending(x => x)
                    .ToImmutableArray();
                foreach (var physicalRow in physicalRowsToValidate)
                {
                    MergeRow(logicalCellsMap, ref logicalToPhysicalMap, ref physicalToLogicalMap, physicalRow);
                }

                return;

                static void MergeCol
                (
                    LogicalCellsMap logicalCellsMap,
                    ref LogicalToPhysicalMap logicalToPhysicalMap,
                    ref PhysicalToLogicalMap physicalToLogicalMap,
                    int physicalCol
                )
                {
                    var logicalCoordinatesToAdjust = new HashSet<Coordinate>();

                    // find cols which start in this area
                    foreach (var (_, logicalCoordinate) in physicalToLogicalMap.Where(x => x.Key.col == physicalCol))
                    {
                        var topLeftPhysicalCoordinate = GetLogicalCellTopLeftPhysicalCoordinate(logicalToPhysicalMap, logicalCoordinate);
                        if (topLeftPhysicalCoordinate.col == physicalCol)
                        {
                            // this physical cell belongs to logical cell which is spawned in this column,
                            // this column is un-mergeable, thus terminate
                            return;
                        }

                        // collect logical cells to adjust
                        logicalCoordinatesToAdjust.Add(logicalCoordinate);
                    }

                    // this column is empty and transient, meaning it has no native cells,
                    // only the ones which col-span into/through it,
                    // thus delete this column and update mappings
                    foreach (var logicalCoordinate in logicalCoordinatesToAdjust)
                    {
                        var oldCell = logicalCellsMap[logicalCoordinate];
                        var newCell = oldCell with { ColumnSpan = oldCell.ColumnSpan - 1 };
                        logicalCellsMap[logicalCoordinate] = newCell;

                        var physicalCoordinatesToRemove = logicalToPhysicalMap[logicalCoordinate]
                            .Where(x => x.col == physicalCol)
                            .ToImmutableArray();
                        foreach (var physicalCoordinate in physicalCoordinatesToRemove)
                        {
                            logicalToPhysicalMap[logicalCoordinate].Remove(physicalCoordinate);
                            physicalToLogicalMap.Remove(physicalCoordinate);
                        }
                    }

                    // move all physical coordinates from this column to the right
                    var physicalCoordinatesToMove = physicalToLogicalMap
                        .Where(p => p.Key.col > physicalCol)
                        .ToImmutableArray();
                    foreach (var (physicalCoordinate, logicalCoordinate) in physicalCoordinatesToMove)
                    {
                        physicalToLogicalMap.Remove(physicalCoordinate);
                        physicalToLogicalMap.Add((physicalCoordinate.row, physicalCoordinate.col - 1), logicalCoordinate);
                    }
                    foreach (var (_, physicalCoordinateSet) in logicalToPhysicalMap)
                    {
                        var physicalCoordinates = physicalCoordinateSet
                            .Where(x => x.col > physicalCol)
                            .ToImmutableArray();
                        foreach (var physicalCoordinate in physicalCoordinates)
                        {
                            physicalCoordinateSet.Remove(physicalCoordinate);
                        }
                        foreach (var physicalCoordinate in physicalCoordinates)
                        {
                            physicalCoordinateSet.Add((physicalCoordinate.row, physicalCoordinate.col - 1));
                        }
                    }
                }

                static void MergeRow
                (
                    LogicalCellsMap logicalCellsMap,
                    ref LogicalToPhysicalMap logicalToPhysicalMap,
                    ref PhysicalToLogicalMap physicalToLogicalMap,
                    int physicalRow
                )
                {
                    var logicalCoordinatesToAdjust = new HashSet<Coordinate>();

                    // find rows which start in this area
                    foreach (var (_, logicalCoordinate) in physicalToLogicalMap.Where(x => x.Key.row == physicalRow))
                    {
                        var topLeftPhysicalCoordinate = GetLogicalCellTopLeftPhysicalCoordinate(logicalToPhysicalMap, logicalCoordinate);
                        if (topLeftPhysicalCoordinate.row == physicalRow)
                        {
                            // this physical cell belongs to logical cell which is spawned in this row,
                            // this row is un-mergeable, thus terminate
                            return;
                        }

                        // collect logical cells to adjust
                        logicalCoordinatesToAdjust.Add(logicalCoordinate);
                    }

                    // this row is empty and transient, meaning it has no native cells,
                    // only the ones which row-span into/through it,
                    // thus delete this row and update mappings
                    foreach (var logicalCoordinate in logicalCoordinatesToAdjust)
                    {
                        var oldCell = logicalCellsMap[logicalCoordinate];
                        var newCell = oldCell with { RowSpan = oldCell.RowSpan - 1 };
                        logicalCellsMap[logicalCoordinate] = newCell;

                        var physicalCoordinatesToRemove = logicalToPhysicalMap[logicalCoordinate]
                            .Where(x => x.row == physicalRow)
                            .ToImmutableArray();
                        foreach (var physicalCoordinate in physicalCoordinatesToRemove)
                        {
                            logicalToPhysicalMap[logicalCoordinate].Remove(physicalCoordinate);
                            physicalToLogicalMap.Remove(physicalCoordinate);
                        }
                    }

                    // move all physical coordinates from this row to the upside
                    var physicalCoordinatesToMove = physicalToLogicalMap
                        .Where(p => p.Key.row > physicalRow)
                        .ToImmutableArray();
                    foreach (var (physicalCoordinate, logicalCoordinate) in physicalCoordinatesToMove)
                    {
                        physicalToLogicalMap.Remove(physicalCoordinate);
                        physicalToLogicalMap.Add((physicalCoordinate.row - 1, physicalCoordinate.col), logicalCoordinate);
                    }
                    foreach (var (_, physicalCoordinateSet) in logicalToPhysicalMap)
                    {
                        var physicalCoordinates = physicalCoordinateSet
                            .Where(x => x.row > physicalRow)
                            .ToImmutableArray();
                        foreach (var physicalCoordinate in physicalCoordinates)
                        {
                            physicalCoordinateSet.Remove(physicalCoordinate);
                        }
                        foreach (var physicalCoordinate in physicalCoordinates)
                        {
                            physicalCoordinateSet.Add((physicalCoordinate.row - 1, physicalCoordinate.col));
                        }
                    }
                }
            }

            #endregion

            #region // BuildPhysicalBorders

            static void BuildPhysicalBorders
            (
                LogicalCellsMap logicalCellsMap,
                LogicalToPhysicalMap logicalToPhysicalMap,
                out IDictionary<Coordinate, PttBorder> verticalPhysicalCellBordersMap,
                out IDictionary<Coordinate, PttBorder> horizontalPhysicalCellBordersMap
            )
            {
                verticalPhysicalCellBordersMap = new SortedDictionary<Coordinate /* physical coordinate */, PttBorder>(); // TODO: SortedDictionary
                horizontalPhysicalCellBordersMap = new SortedDictionary<Coordinate /* physical coordinate */, PttBorder>(); // TODO: SortedDictionary

                foreach (var logicalCell in logicalCellsMap.Values
                             .OrderBy(x => x.Row)
                             .ThenBy(x => x.Col))
                {
                    var topLeftPhysicalCoordinate = GetLogicalCellTopLeftPhysicalCoordinate(logicalToPhysicalMap, logicalCell.Coordinate);

                    // spread vertically (down)
                    for (var y = 0; y < logicalCell.RowSpan; y++)
                    {
                        // spread horizontally (right)
                        for (var x = 0; x < logicalCell.ColumnSpan; x++)
                        {
                            var physicalCoordinate = new Coordinate(topLeftPhysicalCoordinate.row + y, topLeftPhysicalCoordinate.col + x);

                            // check which physical cell borders are on the edge of logical cell
                            var canHaveLeftBorder = x == 0;
                            var canHaveTopBorder = y == 0;
                            var canHaveRightBorder = x == logicalCell.ColumnSpan - 1;
                            var canHaveBottomBorder = y == logicalCell.RowSpan - 1;

                            // get border types
                            var leftBorder = canHaveLeftBorder ? logicalCell.Borders.Left : PttBorder.None;
                            var topBorder = canHaveTopBorder ? logicalCell.Borders.Top : PttBorder.None;
                            var rightBorder = canHaveRightBorder ? logicalCell.Borders.Right : PttBorder.None;
                            var bottomBorder = canHaveBottomBorder ? logicalCell.Borders.Bottom : PttBorder.None;

                            // update vertical borders
                            MergeBorder(verticalPhysicalCellBordersMap, physicalCoordinate, canHaveLeftBorder, leftBorder);
                            MergeBorder(verticalPhysicalCellBordersMap, (physicalCoordinate.row, physicalCoordinate.col + 1), canHaveRightBorder, rightBorder);

                            // update horizontal borders
                            MergeBorder(horizontalPhysicalCellBordersMap, physicalCoordinate, canHaveTopBorder, topBorder);
                            MergeBorder(horizontalPhysicalCellBordersMap, (physicalCoordinate.row + 1, physicalCoordinate.col), canHaveBottomBorder, bottomBorder);
                        }
                    }
                }

                return;

                static PttBorder MergeBorders(PttBorder left, PttBorder right) => (PttBorder)Math.Max((int)left, (int)right);

                static void MergeBorder(IDictionary<Coordinate, PttBorder> existingBorders, Coordinate coordinate, bool canHaveBorder, PttBorder newBorder)
                {
                    if (canHaveBorder)
                    {
                        // merge
                        var existingBorder = existingBorders.TryGetValue(coordinate, out var border) ? border : default;
                        existingBorders[coordinate] = MergeBorders(existingBorder, newBorder);
                    }
                    else
                    {
                        // override
                        existingBorders[coordinate] = PttBorder.None;
                    }
                }
            }

            #endregion

            #region // GetBorderSize

            static int GetBorderSize(PttBorder border) => border switch
            {
                PttBorder.None => 0,
                PttBorder.Normal => 1,
                PttBorder.Bold => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(border))
            };

            #endregion
        }
    }
}
