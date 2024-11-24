namespace RCi.PlainTextTable.Tests
{
    [Parallelizable(ParallelScope.All)]
    public static class ColumnControlTests
    {
        [Test]
        public static void AppendColumnEmpty()
        {
            var ptt = new PlainTextTable();
            ptt.AppendColumn();

            var actual = ptt.ToString();
            Assert.That(actual, Is.EqualTo(string.Empty));
        }

        [Test]
        public static void AppendColumn()
        {
            var ptt = new PlainTextTable();
            ptt.AppendColumn().Text("a", "A");
            ptt.AppendColumn().Text("b", "B");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+---+
                | a | b |
                +---+---+
                | A | B |
                +---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Text()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().Text("a", "b");
            ptt.AppendRow().Text("A", "B");
            ptt.Cols[0].Text("x");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+---+
                | x | b |
                +---+---+
                | x | B |
                +---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Margin()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().Text("a", "b");
            ptt.AppendRow().Text("A", "B");
            ptt.Col(0).Margin(new Margin(0), new Margin(3));
            ptt.Col(1).Margin(new Margin(2));

            var actual = ptt.ToString();
            const string expected =
                """
                +-------+-----+
                |a      |     |
                |       |     |
                |       |  b  |
                |       |     |
                |       |     |
                +-------+-----+
                |       |     |
                |       |     |
                |       |  B  |
                |   A   |     |
                |       |     |
                |       |     |
                |       |     |
                +-------+-----+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public static void Borders()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().Text("a", "b");
            ptt.AppendRow().Text("A", "B");
            ptt.Col(0).Borders(new Borders(Border.Normal), new Borders(Border.Bold));
            ptt.Col(1).Borders(new Borders(Border.None));

            var actual = ptt.ToString();
            const string expected =
                """
                +---+   
                | a | b 
                #===#   
                # A # B 
                #===#   
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void HorizontalAlignment_()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().Text("a", "b");
            ptt.AppendRow().Text("A", "B");
            ptt.AppendRow().Text("0", "1");
            ptt.AppendRow().Text("xxxxx", "xxxxx");
            ptt.Col(0).HorizontalAlignment(HorizontalAlignment.Left, HorizontalAlignment.Center, HorizontalAlignment.Right);
            ptt.Col(1).HorizontalAlignment(HorizontalAlignment.Center);

            var actual = ptt.ToString();
            const string expected =
                """
                +-------+-------+
                | a     |   b   |
                +-------+-------+
                |   A   |   B   |
                +-------+-------+
                |     0 |   1   |
                +-------+-------+
                | xxxxx | xxxxx |
                +-------+-------+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void VerticalAlignment_()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().Text("a", "b", $"x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x");
            ptt.AppendRow().Text("A", "B", $"x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x");
            ptt.AppendRow().Text("0", "1", $"x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x");
            ptt.Col(0).VerticalAlignment(VerticalAlignment.Top, VerticalAlignment.Center, VerticalAlignment.Bottom);
            ptt.Col(1).VerticalAlignment(VerticalAlignment.Center);

            var actual = ptt.ToString();
            const string expected =
                """
                +---+---+---+
                | a |   | x |
                |   |   | x |
                |   | b | x |
                |   |   | x |
                |   |   | x |
                +---+---+---+
                |   |   | x |
                |   |   | x |
                | A | B | x |
                |   |   | x |
                |   |   | x |
                +---+---+---+
                |   |   | x |
                |   |   | x |
                |   | 1 | x |
                |   |   | x |
                | 0 |   | x |
                +---+---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
