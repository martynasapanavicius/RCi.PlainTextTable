using System;

namespace RCi.Toolbox.Tests
{
    [Parallelizable(ParallelScope.All)]
    public static class ColumnTests
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
            ptt.AppendColumn().SetText("a", "A");
            ptt.AppendColumn().SetText("b", "B");

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
        public static void SetText()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().SetText("a", "b");
            ptt.AppendRow().SetText("A", "B");
            ptt.Column(0).SetText("x");

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
        public static void SetMargin()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().SetText("a", "b");
            ptt.AppendRow().SetText("A", "B");
            ptt.Column(0).SetMargin(Margin.Empty, new Margin(3));
            ptt.Column(1).SetMargin(new Margin(2));

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
        public static void SetBorders()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().SetText("a", "b");
            ptt.AppendRow().SetText("A", "B");
            ptt.Column(0).SetBorders(Borders.Normal, Borders.Bold);
            ptt.Column(1).SetBorders(Borders.None);

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
        public static void SetHorizontalAlignment()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().SetText("a", "b");
            ptt.AppendRow().SetText("A", "B");
            ptt.AppendRow().SetText("0", "1");
            ptt.AppendRow().SetText("xxxxx", "xxxxx");
            ptt.Column(0).SetHorizontalAlignment(HorizontalAlignment.Left, HorizontalAlignment.Center, HorizontalAlignment.Right);
            ptt.Column(1).SetHorizontalAlignment(HorizontalAlignment.Center);

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
        public static void SetVerticalAlignment()
        {
            var ptt = new PlainTextTable();
            ptt.AppendRow().SetText("a", "b", $"x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x");
            ptt.AppendRow().SetText("A", "B", $"x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x");
            ptt.AppendRow().SetText("0", "1", $"x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x");
            ptt.Column(0).SetVerticalAlignment(VerticalAlignment.Top, VerticalAlignment.Center, VerticalAlignment.Bottom);
            ptt.Column(1).SetVerticalAlignment(VerticalAlignment.Center);

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
