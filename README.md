# RCi.PlainTextTable
.NET Plain text table builder.

## Simple Example

```csharp
var ptt = new PlainTextTable();
ptt.AppendRow("First Name", "Second Name", "Age");
ptt.AppendRow("Erin", "Zhang", 35);
ptt.AppendRow("Nalani", "David", 22);
ptt.AppendRow("Lexi", "Kim", 47);
```
```
+------------+-------------+-----+
| First Name | Second Name | Age |
+------------+-------------+-----+
| Erin       | Zhang       | 35  |
+------------+-------------+-----+
| Nalani     | David       | 22  |
+------------+-------------+-----+
| Lexi       | Kim         | 47  |
+------------+-------------+-----+
```
## Complex Example
```csharp
var ptt = new PlainTextTable
{
    BorderStyle = BorderStyle.UnicodeSingle,
    DefaultHorizontalAlignment = HorizontalAlignment.Right,
};

// append new row (returns row)
ptt.AppendRow("Accounts")
    // select first cell in this row (returns cell)
    .First()
    // set column span (returns cell)
    .SetColumnSpan(3)
    // set horizontal alignment (returns cell)
    .SetHorizontalAlignment(HorizontalAlignment.Center);

// append new row (returns row)
ptt.AppendRow("Full Name", "Age", "USD", "EUR")
    // select first column in this row (returns cell)
    .First()
    // set column span for this cell (returns cell)
    .SetColumnSpan(2)
    // set horizontal alignment for this cell (returns cell)
    .SetHorizontalAlignment(HorizontalAlignment.Center);

// append new row (returns row)
ptt.AppendRow("Erin", "Zhang", 35, 123.45, 0);

// append new row (returns row)
ptt.AppendRow("Nalani", "David", 22, 0, 420.69);

// append new row (returns row)
ptt.AppendRow("Lexi", "Kim", 47, 523.44, 1999);

// append new row (returns row)
ptt.AppendRow("TOTAL", 646.89, 2419.69)
    // select first column in this row (returns cell)
    .First()
    // set column span for this cell (returns cell)
    .SetColumnSpan(3);

// select first column (returns column)
ptt.FirstColumn()
    // slice column (skip first and take until last) (returns column span)
    .Slice(2, ^1)
    // set horizontal alignment for this column span (returns column span)
    .SetHorizontalAlignment(HorizontalAlignment.Left)
    // move to the right (targeting 2nd column) (returns column span)
    .MoveRight()
    // set horizontal alignment for this column span (returns column span)
    .SetHorizontalAlignment(HorizontalAlignment.Left);

// select last column (returns column)
ptt.LastColumn()
    // skip first two rows (returns column span)
    .Skip(2)
    // set only right border (returns column span)
    .SetBorders(Borders.None with { Right = Border.Normal })
    // move to the left (targeting 2nd column from the end) (returns column span)
    .MoveLeft()
    // set only left border (returns column span)
    .SetBorders(Borders.None with { Left = Border.Normal });

// select last row (returns row)
ptt.FirstRow()
    // set uniform bold border for all cells in this row (returns row)
    .SetBorders(Border.Bold)
    // move down to the second row (returns row)
    .MoveDown()
    // set uniform bold borders for the whole row (returns row)
    .SetBorders(Border.Bold)
    // move down to the last row (4 times) (returns row)
    .MoveDown(4)
    // set uniform bold borders for the whole row (returns row)
    .SetBorders(Border.Bold);
```
```
┏━━━━━━━━━━━━━━━━━━━━━━┓                   
┃       Accounts       ┃                   
┣━━━━━━━━━━━━━━━━┳━━━━━╋━━━━━━━━┳━━━━━━━━━┓
┃   Full Name    ┃ Age ┃    USD ┃     EUR ┃
┡━━━━━━━━┯━━━━━━━╇━━━━━╇━━━━━━━━┻━━━━━━━━━┩
│ Erin   │ Zhang │  35 │ 123.45         0 │
├────────┼───────┼─────┤                  │
│ Nalani │ David │  22 │      0    420.69 │
├────────┼───────┼─────┤                  │
│ Lexi   │ Kim   │  47 │ 523.44      1999 │
┢━━━━━━━━┷━━━━━━━┷━━━━━╈━━━━━━━━┳━━━━━━━━━┪
┃                TOTAL ┃ 646.89 ┃ 2419.69 ┃
┗━━━━━━━━━━━━━━━━━━━━━━┻━━━━━━━━┻━━━━━━━━━┛
```
