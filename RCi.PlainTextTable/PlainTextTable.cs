using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using LogicalCellsMap = System.Collections.Generic.IDictionary<RCi.PlainTextTable.Coordinate /* logical coordinate */, RCi.PlainTextTable.LogicalCell>;
using LogicalToPhysicalMap = System.Collections.Generic.IDictionary<RCi.PlainTextTable.Coordinate /* logical coordinate */, System.Collections.Generic.HashSet<RCi.PlainTextTable.Coordinate /* physical coordinate */>>;
using PhysicalToLogicalMap = System.Collections.Generic.IDictionary<RCi.PlainTextTable.Coordinate /* physical coordinate */, RCi.PlainTextTable.Coordinate /* physical coordinate */>;

namespace RCi.PlainTextTable
{
    public sealed class PlainTextTable
    {
        private readonly Dictionary<Coordinate, Cell> _cells = new();
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public Margin DefaultMargin { get; set; } = new(1, 0);
        public Borders DefaultBorders { get; set; } = new(Border.Normal);
        public HorizontalAlignment DefaultHorizontalAlignment { get; set; } = HorizontalAlignment.Left;
        public VerticalAlignment DefaultVerticalAlignment { get; set; } = VerticalAlignment.Top;
        public ICell this[Coordinate c]
        {
            get
            {
                if (!_cells.TryGetValue(c, out var cell))
                {
                    cell = new Cell
                    {
                        Host = this,
                        Coordinate = c,
                    };
                    _cells.Add(c, cell);
                    RowCount = Math.Max(RowCount, c.Row + 1);
                    ColumnCount = Math.Max(ColumnCount, c.Col + 1);
                }
                return cell;
            }
        }
        public ICell this[int row, int col] => this[(row, col)];

        public RowControl AppendRow()
        {
            var row = RowCount;
            RowCount++;
            return new RowControl(this, row);
        }

        public ColumnControl AppendColumn()
        {
            var col = ColumnCount;
            ColumnCount++;
            return new ColumnControl(this, col);
        }

        internal void DeleteCell(ICell cell)
        {
            if (!ReferenceEquals(cell.Host(), this))
            {
                throw new ArgumentException("cell does not belong to this plain text table");
            }
            _cells.Remove(cell.Coordinate);
            RowCount = _cells.Count == 0 ? 0 : _cells.Max(p => p.Key.Row) + 1;
            ColumnCount = _cells.Count == 0 ? 0 : _cells.Max(p => p.Key.Col) + 1;
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
            _cells.Select(p => new LogicalCell
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

        private static string CompileToText(IEnumerable<LogicalCell> logicalCellsStream)
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
                .GroupBy(p => p.Key.Col)
                .Select(g => (col: g.Key, border: g.Max(p => p.Value)))
                .OrderBy(t => t.col)
                .Select(t => GetBorderSize(t.border))
                .ToImmutableArray();
            var physicalHorizontalBorderHeights = horizontalPhysicalCellBordersMap
                .GroupBy(p => p.Key.Row)
                .Select(g => (row: g.Key, border: g.Max(p => p.Value)))
                .OrderBy(t => t.row)
                .Select(t => GetBorderSize(t.border))
                .ToImmutableArray();
            var physicalColWidths = new int[logicalToPhysicalMap.Count == 0 ? 0 : logicalToPhysicalMap.Values.Max(x => x.Max(c => c.Col)) + 1];
            var physicalRowHeights = new int[logicalToPhysicalMap.Count == 0 ? 0 : logicalToPhysicalMap.Values.Max(x => x.Max(c => c.Row)) + 1];

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
                        .Select(x => x.Col)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToImmutableArray();
                    var minPhysicalCol = physicalCols[0];
                    var maxPhysicalCol = physicalCols[^1];
                    var localColDistribution = physicalCols.ToDictionary(x => x, _ => 0);

                    // distribute height across columns and borders
                    var widthBudget = logicalCell.TextSizeWithMargin.Width;
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
                        .Select(x => x.Row)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToImmutableArray();
                    var minPhysicalRow = physicalRows[0];
                    var maxPhysicalRow = physicalRows[^1];
                    var localRowDistribution = physicalRows.ToDictionary(x => x, _ => 0);

                    // distribute height across rows and borders
                    var heightBudget = logicalCell.TextSizeWithMargin.Height;
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
            var canvas /* [row, col] */ = Enumerable.Range(0, canvasSize.Height).Select(_ => new char[canvasSize.Width]).ToArray();
            var borderMask /* [row, col] */ = Enumerable.Range(0, canvasSize.Height).Select(_ => new Border[canvasSize.Width]).ToArray();

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
                for (var i = 0; i <= physicalCoordinate.Col; i++)
                {
                    x += physicalVerticalBorderWidths[i];
                }
                for (var i = 0; i <= physicalCoordinate.Row; i++)
                {
                    y += physicalHorizontalBorderHeights[i];
                }
                // account for previous cells
                for (var i = 0; i < physicalCoordinate.Col; i++)
                {
                    x += physicalColWidths[i];
                }
                for (var i = 0; i < physicalCoordinate.Row; i++)
                {
                    y += physicalRowHeights[i];
                }

                return (y, x); // row = y, col = x
            }

            TableCellMargins GetTableCellMargins(Coordinate logicalCoordinate)
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
                var xyTopLeft = GetTopLeftTableCoordinate(topLeftPhysicalCell);
                var xyBottomRight = GetTopLeftTableCoordinate(bottomRightPhysicalCell);
                xyBottomRight =
                (
                    xyBottomRight.Row + physicalRowHeights[bottomRightPhysicalCell.Row],
                    xyBottomRight.Col + physicalColWidths[bottomRightPhysicalCell.Col]
                );
                var area = new Margin(xyTopLeft.Col, xyTopLeft.Row, xyBottomRight.Col, xyBottomRight.Row);

                // get borders and corners
                var logicalCell = logicalCellsMap[logicalCoordinate];
                var leftBorderSize = GetBorderSize(logicalCell.Borders.Left);
                var topBorderSize = GetBorderSize(logicalCell.Borders.Top);
                var rightBorderSize = GetBorderSize(logicalCell.Borders.Right);
                var bottomBorderSize = GetBorderSize(logicalCell.Borders.Bottom);

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

            static char GetHorizontalBorderCharacter(Border border) => border switch
            {
                Border.None => ' ',
                Border.Normal => '-',
                Border.Bold => '=',
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            };

            static char GetVerticalBorderCharacter(Border border) => border switch
            {
                Border.None => ' ',
                Border.Normal => '|',
                Border.Bold => '@',
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            };

            static char GetBorderCornerCharacter(Border border) => border switch
            {
                Border.None => ' ',
                Border.Normal => '+',
                Border.Bold => '#',
                _ => throw new ArgumentOutOfRangeException(nameof(border)),
            };

            static void PaintRectangle(char[][] canvas, Margin rectangle, char color)
            {
                for (var y = rectangle.Top; y < rectangle.Bottom; y++)
                {
                    for (var x = rectangle.Left; x < rectangle.Right; x++)
                    {
                        canvas[y][x] = color;
                    }
                }
            }

            static void PaintBorder(char[][] canvas, Border[][] borderMask, Margin rectangle, char color, Border weight)
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

            static void PaintText(char[][] canvas, Margin cellArea, LogicalCell logicalCell)
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
                var topLeftBorder = (Border)Math.Max((int)logicalCell.Borders.Top, (int)logicalCell.Borders.Left);
                var topRightBorder = (Border)Math.Max((int)logicalCell.Borders.Top, (int)logicalCell.Borders.Right);
                var bottomRightBorder = (Border)Math.Max((int)logicalCell.Borders.Bottom, (int)logicalCell.Borders.Right);
                var bottomLeftBorder = (Border)Math.Max((int)logicalCell.Borders.Bottom, (int)logicalCell.Borders.Left);
                PaintBorder(canvas, borderMask, m.TopLeftBorderCorner, GetBorderCornerCharacter(topLeftBorder), topLeftBorder);
                PaintBorder(canvas, borderMask, m.TopRightBorderCorner, GetBorderCornerCharacter(topRightBorder), topRightBorder);
                PaintBorder(canvas, borderMask, m.BottomRightBorderCorner, GetBorderCornerCharacter(bottomRightBorder), bottomRightBorder);
                PaintBorder(canvas, borderMask, m.BottomLeftBorderCorner, GetBorderCornerCharacter(bottomLeftBorder), bottomLeftBorder);
            }

            #region // BuildPhysicalGrid

            static void BuildPhysicalGrid
            (
                IReadOnlyDictionary<Coordinate, LogicalCell> logicalCells,
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
                        .Where(x => x.Row == row)
                        .OrderBy(x => x.Col);
                    foreach (var pair in stream)
                    {
                        if (pair.Col != expected)
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
                    .OrderBy(x => x.Row)
                    .ThenBy(x => x.Col)
                    .First();

            static Coordinate GetLogicalCellBottomRightPhysicalCoordinate(LogicalToPhysicalMap logicalToPhysicalMap, Coordinate logicalCoordinate) =>
                logicalToPhysicalMap[logicalCoordinate]
                    .OrderByDescending(x => x.Row)
                    .ThenByDescending(x => x.Col)
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
                    .Select(p => p.Key.Col)
                    .Distinct()
                    .OrderByDescending(x => x)
                    .ToImmutableArray();
                foreach (var physicalCol in physicalColsToValidate)
                {
                    MergeCol(logicalCellsMap, ref logicalToPhysicalMap, ref physicalToLogicalMap, physicalCol);
                }

                var physicalRowsToValidate = physicalToLogicalMap
                    .Select(p => p.Key.Row)
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
                    foreach (var (_, logicalCoordinate) in physicalToLogicalMap.Where(x => x.Key.Col == physicalCol))
                    {
                        var topLeftPhysicalCoordinate = GetLogicalCellTopLeftPhysicalCoordinate(logicalToPhysicalMap, logicalCoordinate);
                        if (topLeftPhysicalCoordinate.Col == physicalCol)
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
                            .Where(x => x.Col == physicalCol)
                            .ToImmutableArray();
                        foreach (var physicalCoordinate in physicalCoordinatesToRemove)
                        {
                            logicalToPhysicalMap[logicalCoordinate].Remove(physicalCoordinate);
                            physicalToLogicalMap.Remove(physicalCoordinate);
                        }
                    }

                    // move all physical coordinates from this column to the right
                    var physicalCoordinatesToMove = physicalToLogicalMap
                        .Where(p => p.Key.Col > physicalCol)
                        .ToImmutableArray();
                    foreach (var (physicalCoordinate, logicalCoordinate) in physicalCoordinatesToMove)
                    {
                        physicalToLogicalMap.Remove(physicalCoordinate);
                        physicalToLogicalMap.Add((physicalCoordinate.Row, physicalCoordinate.Col - 1), logicalCoordinate);
                    }
                    foreach (var (_, physicalCoordinateSet) in logicalToPhysicalMap)
                    {
                        var physicalCoordinates = physicalCoordinateSet
                            .Where(x => x.Col > physicalCol)
                            .ToImmutableArray();
                        foreach (var physicalCoordinate in physicalCoordinates)
                        {
                            physicalCoordinateSet.Remove(physicalCoordinate);
                        }
                        foreach (var physicalCoordinate in physicalCoordinates)
                        {
                            physicalCoordinateSet.Add((physicalCoordinate.Row, physicalCoordinate.Col - 1));
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
                    foreach (var (_, logicalCoordinate) in physicalToLogicalMap.Where(x => x.Key.Row == physicalRow))
                    {
                        var topLeftPhysicalCoordinate = GetLogicalCellTopLeftPhysicalCoordinate(logicalToPhysicalMap, logicalCoordinate);
                        if (topLeftPhysicalCoordinate.Row == physicalRow)
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
                            .Where(x => x.Row == physicalRow)
                            .ToImmutableArray();
                        foreach (var physicalCoordinate in physicalCoordinatesToRemove)
                        {
                            logicalToPhysicalMap[logicalCoordinate].Remove(physicalCoordinate);
                            physicalToLogicalMap.Remove(physicalCoordinate);
                        }
                    }

                    // move all physical coordinates from this row to the upside
                    var physicalCoordinatesToMove = physicalToLogicalMap
                        .Where(p => p.Key.Row > physicalRow)
                        .ToImmutableArray();
                    foreach (var (physicalCoordinate, logicalCoordinate) in physicalCoordinatesToMove)
                    {
                        physicalToLogicalMap.Remove(physicalCoordinate);
                        physicalToLogicalMap.Add((physicalCoordinate.Row - 1, physicalCoordinate.Col), logicalCoordinate);
                    }
                    foreach (var (_, physicalCoordinateSet) in logicalToPhysicalMap)
                    {
                        var physicalCoordinates = physicalCoordinateSet
                            .Where(x => x.Row > physicalRow)
                            .ToImmutableArray();
                        foreach (var physicalCoordinate in physicalCoordinates)
                        {
                            physicalCoordinateSet.Remove(physicalCoordinate);
                        }
                        foreach (var physicalCoordinate in physicalCoordinates)
                        {
                            physicalCoordinateSet.Add((physicalCoordinate.Row - 1, physicalCoordinate.Col));
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
                out IDictionary<Coordinate, Border> verticalPhysicalCellBordersMap,
                out IDictionary<Coordinate, Border> horizontalPhysicalCellBordersMap
            )
            {
                verticalPhysicalCellBordersMap = new SortedDictionary<Coordinate /* physical coordinate */, Border>(); // TODO: SortedDictionary
                horizontalPhysicalCellBordersMap = new SortedDictionary<Coordinate /* physical coordinate */, Border>(); // TODO: SortedDictionary

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
                            var physicalCoordinate = new Coordinate(topLeftPhysicalCoordinate.Row + y, topLeftPhysicalCoordinate.Col + x);

                            // check which physical cell borders are on the edge of logical cell
                            var canHaveLeftBorder = x == 0;
                            var canHaveTopBorder = y == 0;
                            var canHaveRightBorder = x == logicalCell.ColumnSpan - 1;
                            var canHaveBottomBorder = y == logicalCell.RowSpan - 1;

                            // get border types
                            var leftBorder = canHaveLeftBorder ? logicalCell.Borders.Left : Border.None;
                            var topBorder = canHaveTopBorder ? logicalCell.Borders.Top : Border.None;
                            var rightBorder = canHaveRightBorder ? logicalCell.Borders.Right : Border.None;
                            var bottomBorder = canHaveBottomBorder ? logicalCell.Borders.Bottom : Border.None;

                            // update vertical borders
                            MergeBorder(verticalPhysicalCellBordersMap, physicalCoordinate, canHaveLeftBorder, leftBorder);
                            MergeBorder(verticalPhysicalCellBordersMap, (physicalCoordinate.Row, physicalCoordinate.Col + 1), canHaveRightBorder, rightBorder);

                            // update horizontal borders
                            MergeBorder(horizontalPhysicalCellBordersMap, physicalCoordinate, canHaveTopBorder, topBorder);
                            MergeBorder(horizontalPhysicalCellBordersMap, (physicalCoordinate.Row + 1, physicalCoordinate.Col), canHaveBottomBorder, bottomBorder);
                        }
                    }
                }

                return;

                static Border MergeBorders(Border left, Border right) => (Border)Math.Max((int)left, (int)right);

                static void MergeBorder(IDictionary<Coordinate, Border> existingBorders, Coordinate coordinate, bool canHaveBorder, Border newBorder)
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
                        existingBorders[coordinate] = Border.None;
                    }
                }
            }

            #endregion

            #region // GetBorderSize

            static int GetBorderSize(Border border) => border switch
            {
                Border.None => 0,
                Border.Normal => 1,
                Border.Bold => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(border))
            };

            #endregion
        }
    }
}
