﻿namespace RCi.PlainTextTable.Tests
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
            ptt.AppendRow().SetText("a", "b");
            ptt.AppendRow().SetText("A", "B");

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
            ptt.AppendRow().SetText("a", "b");
            ptt.AppendRow().SetText("A", "B");
            ptt.Row(0).SetText("x");

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
            ptt.AppendRow().SetText("a", "b");
            ptt.AppendRow().SetText("A", "B");
            ptt.Row(0).SetMargin(new Margin(0), new Margin(3));
            ptt.Row(1).SetMargin(new Margin(2));

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
            ptt.AppendRow().SetText("a", "b");
            ptt.AppendRow().SetText("A", "B");
            ptt.Row(0).SetBorders(new Borders(Border.Normal), new Borders(Border.Bold));
            ptt.Row(1).SetBorders(new Borders(Border.None));

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
            ptt.AppendRow().SetText("a", "b", "c");
            ptt.AppendRow().SetText("A", "B", "C");
            ptt.AppendRow().SetText("xxxxx", "xxxxx", "xxxxx");
            ptt.Row(0).SetHorizontalAlignment(HorizontalAlignment.Left, HorizontalAlignment.Center, HorizontalAlignment.Right);
            ptt.Row(1).SetHorizontalAlignment(HorizontalAlignment.Center);

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
            ptt.AppendRow().SetText("a", "b", "c", $"x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x");
            ptt.AppendRow().SetText("A", "B", "C", $"x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x{Environment.NewLine}x");
            ptt.Row(0).SetVerticalAlignment(VerticalAlignment.Top, VerticalAlignment.Center, VerticalAlignment.Bottom);
            ptt.Row(1).SetVerticalAlignment(VerticalAlignment.Center);

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