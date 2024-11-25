namespace RCi.PlainTextTable.Tests
{
    [Parallelizable(ParallelScope.All)]
    public static class ReadmeTests
    {
        [Test]
        public static void Simple()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow("First Name", "Second Name", "Age");
            ptt.AppendRow("Erin", "Zhang", 35);
            ptt.AppendRow("Nalani", "David", 22);
            ptt.AppendRow("Lexi", "Kim", 47);

            var actual = ptt.ToString();
            const string expected =
                """
                +------------+-------------+-----+
                | First Name | Second Name | Age |
                +------------+-------------+-----+
                | Erin       | Zhang       | 35  |
                +------------+-------------+-----+
                | Nalani     | David       | 22  |
                +------------+-------------+-----+
                | Lexi       | Kim         | 47  |
                +------------+-------------+-----+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Complex()
        {
            var ptt = new PlainTextTable
            {
                BorderStyle = BorderStyle.UnicodeSingle
            };

            ptt.AppendRow("Personal Data", "Balance")
                .ColumnSpan(3, 2)
                .HorizontalAlignment(HorizontalAlignment.Center);
            ptt.AppendRow("Full Name", "Age", "USD", "EUR")
                .FirstCell()
                    .ColumnSpan(2)
                    .HorizontalAlignment(HorizontalAlignment.Center);
            ptt.AppendRow("Erin", "Zhang", 35, 123.45, 0);
            ptt.AppendRow("Nalani", "David", 22, 0, 420.69);
            ptt.AppendRow("Lexi", "Kim", 47, 523.44, 1999);
            ptt.AppendRow("TOTAL", 646.89, 2419.69)
                .FirstCell()
                    .ColumnSpan(3)
                    .HorizontalAlignment(HorizontalAlignment.Right);

            ptt.Row(1)
                .TakeLast(2)
                .HorizontalAlignment(HorizontalAlignment.Right);
            ptt.LastColumn()
                .HorizontalAlignment(HorizontalAlignment.Right)
                .MoveLeft()
                .HorizontalAlignment(HorizontalAlignment.Right);
            ptt.LastRow()
                .HorizontalAlignment(HorizontalAlignment.Right)
                .Borders(Borders.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌──────────────────────┬──────────────────┐
                │    Personal Data     │     Balance      │
                ├────────────────┬─────┼────────┬─────────┤
                │   Full Name    │ Age │    USD │     EUR │
                ├────────┬───────┼─────┼────────┼─────────┤
                │ Erin   │ Zhang │ 35  │ 123.45 │       0 │
                ├────────┼───────┼─────┼────────┼─────────┤
                │ Nalani │ David │ 22  │      0 │  420.69 │
                ├────────┼───────┼─────┼────────┼─────────┤
                │ Lexi   │ Kim   │ 47  │ 523.44 │    1999 │
                ┢━━━━━━━━┷━━━━━━━┷━━━━━╈━━━━━━━━╈━━━━━━━━━┪
                ┃                TOTAL ┃ 646.89 ┃ 2419.69 ┃
                ┗━━━━━━━━━━━━━━━━━━━━━━┻━━━━━━━━┻━━━━━━━━━┛
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
