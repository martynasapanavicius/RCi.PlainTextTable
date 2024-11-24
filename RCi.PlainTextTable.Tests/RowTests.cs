namespace RCi.PlainTextTable.Tests
{
    [Parallelizable(ParallelScope.All)]
    public static class RowTests
    {
        [Test]
        public static void AppendRowEmpty()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow();

            var actual = ptt.ToString();
            Assert.That(actual, Is.EqualTo(string.Empty));
        }

        [Test]
        public static void AppendRow()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().Text("a", "b");
            ptt.AppendRow().Text("A", "B");

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
            ptt.Row(0).Text("x");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+---+
                | x | x |
                +---+---+
                | A | B |
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
            ptt.Row(0).Margin(new Margin(0), new Margin(3));
            ptt.Row(1).Margin(new Margin(2));

            var actual = ptt.ToString();
            const string expected =
                """
                +-----+-------+
                |a    |       |
                |     |       |
                |     |       |
                |     |   b   |
                |     |       |
                |     |       |
                |     |       |
                +-----+-------+
                |     |       |
                |     |       |
                |  A  |  B    |
                |     |       |
                |     |       |
                +-----+-------+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Borders()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().Text("a", "b");
            ptt.AppendRow().Text("A", "B");
            ptt.Row(0).Borders(new Borders(Border.Normal), new Borders(Border.Bold));
            ptt.Row(1).Borders(new Borders(Border.None));

            var actual = ptt.ToString();
            const string expected =
                """
                +---#===#
                | a # b #
                +---#===#
                  A   B  
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void HorizontalAlignment_()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().Text("a", "b", "c");
            ptt.AppendRow().Text("A", "B", "C");
            ptt.AppendRow().Text("xxxxx", "xxxxx", "xxxxx");
            ptt.Row(0).HorizontalAlignment(HorizontalAlignment.Left, HorizontalAlignment.Center, HorizontalAlignment.Right);
            ptt.Row(1).HorizontalAlignment(HorizontalAlignment.Center);

            var actual = ptt.ToString();
            const string expected =
                """
                +-------+-------+-------+
                | a     |   b   |     c |
                +-------+-------+-------+
                |   A   |   B   |   C   |
                +-------+-------+-------+
                | xxxxx | xxxxx | xxxxx |
                +-------+-------+-------+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void VerticalAlignment_()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().Text("a", "b", "c", $"x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x");
            ptt.AppendRow().Text("A", "B", "C", $"x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x");
            ptt.Row(0).VerticalAlignment(VerticalAlignment.Top, VerticalAlignment.Center, VerticalAlignment.Bottom);
            ptt.Row(1).VerticalAlignment(VerticalAlignment.Center);

            var actual = ptt.ToString();
            const string expected =
                """
                +---+---+---+---+
                | a |   |   | x |
                |   |   |   | x |
                |   | b |   | x |
                |   |   |   | x |
                |   |   | c | x |
                +---+---+---+---+
                |   |   |   | x |
                |   |   |   | x |
                | A | B | C | x |
                |   |   |   | x |
                |   |   |   | x |
                +---+---+---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
