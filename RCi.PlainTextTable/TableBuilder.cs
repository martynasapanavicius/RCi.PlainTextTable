using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LogicalCellsMap =
    System.Collections.Generic.Dictionary
    <
        RCi.PlainTextTable.Coordinate /* logical coordinate */,
        RCi.PlainTextTable.LogicalCell
    >;
using LogicalToPhysicalMap =
    System.Collections.Generic.Dictionary
    <
        RCi.PlainTextTable.Coordinate /* logical coordinate */,
        System.Collections.Generic.HashSet<RCi.PlainTextTable.Coordinate /* physical coordinate */>
    >;
using PhysicalToLogicalMap =
    System.Collections.Generic.Dictionary
    <
        RCi.PlainTextTable.Coordinate /* physical coordinate */,
        RCi.PlainTextTable.Coordinate /* physical coordinate */
    >;

namespace RCi.PlainTextTable
{
    internal static class TableBuilder
    {
        private static Coordinate GetLogicalCellTopLeftPhysicalCoordinate(
            LogicalToPhysicalMap logicalToPhysicalMap, Coordinate logicalCoordinate) =>
            logicalToPhysicalMap[logicalCoordinate].OrderBy(x => x).First();

        private static void BuildPhysicalGrid
        (
            LogicalCellsMap logicalCellsMap,
            out LogicalToPhysicalMap logicalToPhysicalMap,
            out PhysicalToLogicalMap physicalToLogicalMap
        )
        {
            // construct physical grid
            logicalToPhysicalMap = new LogicalToPhysicalMap();
            physicalToLogicalMap = new PhysicalToLogicalMap();

            // helper set for finding next available column in a row
            var physicalCoordinatesSet = new HashSet<Coordinate>();

            // since we might have empty logical rows, ignore them in physical mapping
            var logicalToPhysicalRowMap = logicalCellsMap
                .Values
                .Select(p => p.Row)
                .Distinct()
                .OrderBy(x => x)
                .Select((n, i) => (logicalRow: n, physicalRow: i))
                .ToDictionary(t => t.logicalRow, t => t.physicalRow);
            foreach (var logicalCell in logicalCellsMap.Values.OrderBy(x => x.Coordinate))
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

        private static void MergeTransientEmptyPhysicalColsAndRows
        (
            ref LogicalCellsMap logicalCellsMap,
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
                MergeCol(ref logicalCellsMap, ref logicalToPhysicalMap, ref physicalToLogicalMap, physicalCol);
            }

            var physicalRowsToValidate = physicalToLogicalMap
                .Select(p => p.Key.Row)
                .Distinct()
                .OrderByDescending(x => x)
                .ToImmutableArray();
            foreach (var physicalRow in physicalRowsToValidate)
            {
                MergeRow(ref logicalCellsMap, ref logicalToPhysicalMap, ref physicalToLogicalMap, physicalRow);
            }

            return;

            static void MergeCol
            (
                ref LogicalCellsMap logicalCellsMap,
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
                ref LogicalCellsMap logicalCellsMap,
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

        private static void BuildPhysicalBorders
        (
            LogicalCellsMap logicalCellsMap,
            LogicalToPhysicalMap logicalToPhysicalMap,
            out IDictionary<Coordinate, Border> verticalPhysicalCellBordersMap,
            out IDictionary<Coordinate, Border> horizontalPhysicalCellBordersMap
        )
        {
            verticalPhysicalCellBordersMap = new SortedDictionary<Coordinate /* physical coordinate */, Border>(); // TODO: SortedDictionary
            horizontalPhysicalCellBordersMap = new SortedDictionary<Coordinate /* physical coordinate */, Border>(); // TODO: SortedDictionary

            foreach (var logicalCell in logicalCellsMap.Values.OrderBy(x => x.Coordinate))
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

        private static ImmutableArray<int> GrowPhysicalCols
        (
            LogicalCellsMap logicalCellsMap,
            LogicalToPhysicalMap logicalToPhysicalMap,
            ImmutableArray<int> physicalVerticalBorderWidths
        )
        {
            var physicalColWidths = new int[logicalToPhysicalMap.Count == 0 ? 0 : logicalToPhysicalMap.Values.Max(x => x.Max(c => c.Col)) + 1];

            foreach (var logicalCell in logicalCellsMap.Values.OrderBy(x => x.Coordinate))
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

            return physicalColWidths.UnsafeAsImmutableArray();
        }

        private static ImmutableArray<int> GrowPhysicalRows
        (
            LogicalCellsMap logicalCellsMap,
            LogicalToPhysicalMap logicalToPhysicalMap,
            ImmutableArray<int> physicalHorizontalBorderHeights
        )
        {
            var physicalRowHeights = new int[logicalToPhysicalMap.Count == 0 ? 0 : logicalToPhysicalMap.Values.Max(x => x.Max(c => c.Row)) + 1];

            foreach (var logicalCell in logicalCellsMap.Values.OrderBy(x => x.Coordinate))
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

            return physicalRowHeights.UnsafeAsImmutableArray();
        }

        internal static void BuildPhysicalTable
        (
            IEnumerable<LogicalCell> logicalCells,
            out LogicalCellsMap logicalCellsMap,
            out LogicalToPhysicalMap logicalToPhysicalMap,
            out ImmutableArray<int> physicalVerticalBorderWidths,
            out ImmutableArray<int> physicalHorizontalBorderHeights,
            out ImmutableArray<int> physicalColWidths,
            out ImmutableArray<int> physicalRowHeights
        )
        {
            logicalCellsMap = logicalCells.ToDictionary(c => c.Coordinate, c => c);

            // build physical grid
            BuildPhysicalGrid(logicalCellsMap, out logicalToPhysicalMap, out var physicalToLogicalMap);

            // there might be empty rows with no original cell spawns, we can merge those
            MergeTransientEmptyPhysicalColsAndRows(ref logicalCellsMap, ref logicalToPhysicalMap, ref physicalToLogicalMap);

            // build physical borders map
            BuildPhysicalBorders(logicalCellsMap, logicalToPhysicalMap, out var verticalPhysicalCellBordersMap, out var horizontalPhysicalCellBordersMap);

            // get max sizes for global physical borders
            physicalVerticalBorderWidths =
            [
                ..verticalPhysicalCellBordersMap
                    .GroupBy(p => p.Key.Col)
                    .Select(g => (col: g.Key, border: g.Max(p => p.Value)))
                    .OrderBy(t => t.col)
                    .Select(t => PlainTextTable.GetBorderSize(t.border))
            ];
            physicalHorizontalBorderHeights =
            [
                ..horizontalPhysicalCellBordersMap
                    .GroupBy(p => p.Key.Row)
                    .Select(g => (row: g.Key, border: g.Max(p => p.Value)))
                    .OrderBy(t => t.row)
                    .Select(t => PlainTextTable.GetBorderSize(t.border))
            ];

            // get min sizes for physical rows and columns
            physicalColWidths = GrowPhysicalCols(logicalCellsMap, logicalToPhysicalMap, physicalVerticalBorderWidths);
            physicalRowHeights = GrowPhysicalRows(logicalCellsMap, logicalToPhysicalMap, physicalHorizontalBorderHeights);
        }
    }
}
