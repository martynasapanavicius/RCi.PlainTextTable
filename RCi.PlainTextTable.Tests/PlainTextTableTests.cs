﻿namespace RCi.PlainTextTable.Tests
{
    [Parallelizable(ParallelScope.All)]
    public static class PlainTextTableTests
    {
        [Test]
        public static void Empty()
        {
            var ptt = new PlainTextTable();
            var actual = ptt.ToString();
            Assert.That(actual, Is.EqualTo(string.Empty));
        }

        [Test]
        public static void One()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+
                | a |
                +---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void OneFarAway()
        {
            var ptt = new PlainTextTable();
            ptt[99, 99].Text("a");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+
                | a |
                +---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void TwoInOneRow()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a");
            ptt[0, 1].Text("b");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+---+
                | a | b |
                +---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void TwoInOneRowWithGap()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a");
            ptt[0, 99].Text("b");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+---+
                | a | b |
                +---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void TwoInOneColumn()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a");
            ptt[1, 0].Text("b");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+
                | a |
                +---+
                | b |
                +---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void TwoInOneColumnWithGap()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a");
            ptt[1, 0].Text("b");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+
                | a |
                +---+
                | b |
                +---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Quad()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a");
            ptt[0, 1].Text("b");
            ptt[1, 0].Text("A");
            ptt[1, 1].Text("B");

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
        public static void QuadWithGaps()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a");
            ptt[0, 99].Text("b");
            ptt[99, 0].Text("A");
            ptt[99, 99].Text("B");

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
        public static void ColSpan()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a").ColumnSpan(2);
            ptt[0, 1].Text("b");
            ptt[1, 0].Text("A");
            ptt[1, 1].Text("B");

            var actual = ptt.ToString();
            const string expected =
                """
                +-------+---+
                | a     | b |
                +---+---+---+
                | A | B |    
                +---+---+    
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a").RowSpan(2);
            ptt[0, 1].Text("b");
            ptt[1, 0].Text("A");
            ptt[1, 1].Text("B");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+---+    
                | a | b |    
                |   +---+---+
                |   | A | B |
                +---+---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColSpanWithGap()
        {
            var ptt = new PlainTextTable();

            ptt[0, 0].Text("a");
            ptt[0, 1].Text("b").ColumnSpan(2).RowSpan(2);
            ptt[0, 2].Text("c");
            ptt[0, 3].Text("d");

            ptt[1, 0].Text("A");
            ptt[1, 1].Text("B");
            ptt[1, 2].Text("C");
            ptt[1, 3].Text("D");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+---+---+---+    
                | a | b | c | d |    
                +---+   +---+---+---+
                | A |   | B | C | D |
                +---+---+---+---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Overlap()
        {
            var ptt = new PlainTextTable();

            ptt[0, 0].Text("a");
            ptt[0, 1].Text("b").ColumnSpan(2).RowSpan(2);
            ptt[0, 2].Text("c");

            ptt[1, 0].Text("A").ColumnSpan(2).RowSpan(2);
            ptt[1, 1].Text("B");

            ptt[2, 0].Text("0");
            ptt[2, 1].Text("1");

            ptt[3, 0].Text("!");
            ptt[3, 1].Text("@");
            ptt[3, 2].Text("#");
            ptt[3, 3].Text("$");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+-------+---+
                | a | b     | c |
                +-------+   +---+
                | A     |   | B |
                |       +---+---+
                |       | 0 | 1 |
                +---+---+---+---+
                | ! | @ | # | $ |
                +---+---+---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Overlap_NoMargin()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("a");
            ptt[0, 1].Text("b").ColumnSpan(2).RowSpan(2);
            ptt[0, 2].Text("c");

            ptt[1, 0].Text("A").ColumnSpan(2).RowSpan(2);
            ptt[1, 1].Text("B");

            ptt[2, 0].Text("0");
            ptt[2, 1].Text("1");

            ptt[3, 0].Text("!");
            ptt[3, 1].Text("@");
            ptt[3, 2].Text("#");
            ptt[3, 3].Text("$");

            var actual = ptt.ToString();
            const string expected =
                """
                +-+---+-+
                |a|b  |c|
                +---+ +-+
                |A  | |B|
                |   +-+-+
                |   |0|1|
                +-+-+-+-+
                |!|@|#|$|
                +-+-+-+-+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void FractionalGrowth_ColumnSpan_DoesNotExpand()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("1234").ColumnSpan(3);

            ptt[1, 0].Text("a");
            ptt[1, 1].Text("b");
            ptt[1, 2].Text("c");

            var actual = ptt.ToString();
            const string expected =
                """
                +-----+
                |1234 |
                +-+-+-+
                |a|b|c|
                +-+-+-+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void FractionalGrowth_ColumnSpan_Fit()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("12345").ColumnSpan(3);

            ptt[1, 0].Text("a");
            ptt[1, 1].Text("b");
            ptt[1, 2].Text("c");

            var actual = ptt.ToString();
            const string expected =
                """
                +-----+
                |12345|
                +-+-+-+
                |a|b|c|
                +-+-+-+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void FractionalGrowth_ColumnSpan_DoesNotFit_Leftover1()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("123456").ColumnSpan(3);

            ptt[1, 0].Text("a");
            ptt[1, 1].Text("b");
            ptt[1, 2].Text("c");

            var actual = ptt.ToString();
            const string expected =
                """
                +------+
                |123456|
                +-+-+--+
                |a|b|c |
                +-+-+--+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void FractionalGrowth_ColumnSpan_DoesNotFit_Leftover2()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("1234567").ColumnSpan(3);

            ptt[1, 0].Text("a");
            ptt[1, 1].Text("b");
            ptt[1, 2].Text("c");

            var actual = ptt.ToString();
            const string expected =
                """
                +-------+
                |1234567|
                +-+--+--+
                |a|b |c |
                +-+--+--+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void FractionalGrowth_ColumnSpan_Fit2()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("12345678").ColumnSpan(3);

            ptt[1, 0].Text("a");
            ptt[1, 1].Text("b");
            ptt[1, 2].Text("c");

            var actual = ptt.ToString();
            const string expected =
                """
                +--------+
                |12345678|
                +--+--+--+
                |a |b |c |
                +--+--+--+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void FractionalGrowth_RowSpan_DoesNotExpand()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("""
                           0
                           1
                           2
                           3
                           """).RowSpan(3);

            ptt[0, 1].Text("a");
            ptt[1, 1].Text("b");
            ptt[2, 1].Text("c");

            var actual = ptt.ToString();
            const string expected =
                """
                +-+-+
                |0|a|
                |1+-+
                |2|b|
                |3+-+
                | |c|
                +-+-+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void FractionalGrowth_RowSpan_Fit()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("""
                           0
                           1
                           2
                           3
                           4
                           """).RowSpan(3);

            ptt[0, 1].Text("a");
            ptt[1, 1].Text("b");
            ptt[2, 1].Text("c");

            var actual = ptt.ToString();
            const string expected =
                """
                +-+-+
                |0|a|
                |1+-+
                |2|b|
                |3+-+
                |4|c|
                +-+-+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void FractionalGrowth_RowSpan_DoesNotFit_Leftover1()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("""
                           0
                           1
                           2
                           3
                           4
                           5
                           """).RowSpan(3);

            ptt[0, 1].Text("a");
            ptt[1, 1].Text("b");
            ptt[2, 1].Text("c");

            var actual = ptt.ToString();
            const string expected =
                """
                +-+-+
                |0|a|
                |1+-+
                |2|b|
                |3+-+
                |4|c|
                |5| |
                +-+-+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void FractionalGrowth_RowSpan_DoesNotFit_Leftover2()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("""
                           0
                           1
                           2
                           3
                           4
                           5
                           6
                           """).RowSpan(3);

            ptt[0, 1].Text("a");
            ptt[1, 1].Text("b");
            ptt[2, 1].Text("c");

            var actual = ptt.ToString();
            const string expected =
                """
                +-+-+
                |0|a|
                |1+-+
                |2|b|
                |3| |
                |4+-+
                |5|c|
                |6| |
                +-+-+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void FractionalGrowth_RowSpan_Fit2()
        {
            var ptt = new PlainTextTable
            {
                DefaultMargin = 0,
            };

            ptt[0, 0].Text("""
                           0
                           1
                           2
                           3
                           4
                           5
                           6
                           7
                           """).RowSpan(3);

            ptt[0, 1].Text("a");
            ptt[1, 1].Text("b");
            ptt[2, 1].Text("c");

            var actual = ptt.ToString();
            const string expected =
                """
                +-+-+
                |0|a|
                |1| |
                |2+-+
                |3|b|
                |4| |
                |5+-+
                |6|c|
                |7| |
                +-+-+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(BorderStyle.Ascii)]
        [TestCase(BorderStyle.UnicodeSingle)]
        [TestCase(BorderStyle.UnicodeDouble)]
        public static void ComplexSingle(BorderStyle style)
        {
            var ptt = new PlainTextTable
            {
                BorderStyle = style,
                DefaultHorizontalAlignment = HorizontalAlignment.Left,
                DefaultVerticalAlignment = VerticalAlignment.Center,
            };

            ptt[0, 0].Text("0").Borders(Border.None);
            ptt[0, 1].Text("1");
            ptt[0, 2].Text("2");
            ptt[0, 3].Text("3");
            ptt[0, 4].Text("4");
            ptt[0, 5].Text("5");
            ptt[0, 6].Text("6");
            ptt[0, 7].Text("7");
            ptt[0, 8].Text("8");
            ptt[0, 9].Text("9");

            ptt[1, 0].Text("0'");
            ptt[1, 1].Text("1'''''''''''''''''''''''''''''''").ColumnSpan(3).RowSpan(2);
            ptt[1, 2].Text("2'");
            ptt[1, 3].Text("3'''''''''''''''''''''''''''''''").Borders(Border.Bold).ColumnSpan(2).RowSpan(3);
            ptt[1, 4].Text("4'");
            ptt[1, 5].Text("5'").Borders(Border.Normal, Border.Normal, Border.Normal, Border.None);
            ptt[1, 6].Text("6'");
            ptt[1, 7].Text("7'");
            ptt[1, 8].Text("8'");
            ptt[1, 9].Text("9'");

            ptt[2, 0].Text($"0*{Environment.NewLine}0**");
            ptt[2, 1].Text("1*").ColumnSpan(2).RowSpan(2);
            ptt[2, 2].Text("2*");
            ptt[2, 3].Text("3*").Borders(Border.Bold, Border.None, Border.Bold, Border.Bold);
            ptt[2, 4].Text("4*");
            ptt[2, 5].Text("5*");
            ptt[2, 6].Text("6*");
            ptt[2, 7].Text("7*");
            ptt[2, 8].Text("8*").Borders(Border.None);
            ptt[2, 9].Text("9*").Borders(Border.None);

            ptt[3, 0].Text("aaaaaaaaaaaaa").Borders(Border.Bold).ColumnSpan(2);
            ptt[3, 1].Text("b");
            ptt[3, 2].Text("c");
            ptt[3, 3].Text("d");
            ptt[3, 4].Text("e");
            ptt[3, 5].Text("f");
            ptt[3, 6].Text("g");
            ptt[3, 7].Text("h");
            ptt[3, 8].Text("i");
            ptt[3, 9].Text("j");

            ptt[4, 0].Text("A");
            ptt[4, 1].Text("B");
            ptt[4, 2].Text("C");
            ptt[4, 3].Text("D");
            ptt[4, 4].Text("E");
            ptt[4, 5].Text("F");
            ptt[4, 6].Text("G");
            ptt[4, 7].Text("H");
            ptt[4, 8].Text("I");
            ptt[4, 9].Text("J");

            var actual = ptt.ToString();

            const string expectedAscii =
                """
                        +----------+-----------+-----------+----+----------------+-----------------+----+----+----+                        
                  0     | 1        | 2         | 3         | 4  | 5              | 6               | 7  | 8  | 9  |                        
                +-------+----------------------------------+----#==================================#----+----+----+----+----+----+         
                | 0'    |                                  | 2' #                                  # 4' | 5' | 6' | 7' | 8' | 9' |         
                +-------+ 1''''''''''''''''''''''''''''''' +----#----------------+                 #----#    #----+----+----+----+         
                | 0*    |                                  |                     |'''''''''''''''' # 2* # 3* # 4* | 5* | 6* | 7* | 8*   9* 
                | 0**   |                                  | 1*                  |                 #    #    #    |    |    |    |         
                #==================#-----------+-----------+                     |                 #----#====#----+----+----+----+----+    
                # aaaaaaaaaaaaa    # b         | c         |                     |                 # d  | e  | f  | g  | h  | i  | j  |    
                #==================#-----------+-----------+----#==================================#----+----+----+----+----+----+----+    
                | A     | B        | C         | D         | E  | F              | G               | H  | I  | J  |                        
                +-------+----------+-----------+-----------+----+----------------+-----------------+----+----+----+                        
                """;

            const string expectedUnicodeSingle =
                """
                        ┌──────────┬───────────┬───────────┬────┬────────────────┬─────────────────┬────┬────┬────┐                        
                  0     │ 1        │ 2         │ 3         │ 4  │ 5              │ 6               │ 7  │ 8  │ 9  │                        
                ┌───────┼──────────┴───────────┴───────────┼────╆━━━━━━━━━━━━━━━━┷━━━━━━━━━━━━━━━━━╅────┼────┼────┼────┬────┬────┐         
                │ 0'    │                                  │ 2' ┃                                  ┃ 4' │ 5' │ 6' │ 7' │ 8' │ 9' │         
                ├───────┤ 1''''''''''''''''''''''''''''''' ├────┸────────────────┐                 ┠────┧    ┟────┼────┼────┼────┤         
                │ 0*    │                                  │                     │'''''''''''''''' ┃ 2* ┃ 3* ┃ 4* │ 5* │ 6* │ 7* │ 8*   9* 
                │ 0**   │                                  │ 1*                  │                 ┃    ┃    ┃    │    │    │    │         
                ┢━━━━━━━┷━━━━━━━━━━┱───────────┬───────────┤                     │                 ┠────╄━━━━╃────┼────┼────┼────┼────┐    
                ┃ aaaaaaaaaaaaa    ┃ b         │ c         │                     │                 ┃ d  │ e  │ f  │ g  │ h  │ i  │ j  │    
                ┡━━━━━━━┯━━━━━━━━━━╃───────────┼───────────┼────┮━━━━━━━━━━━━━━━━┿━━━━━━━━━━━━━━━━━╃────┼────┼────┼────┴────┴────┴────┘    
                │ A     │ B        │ C         │ D         │ E  │ F              │ G               │ H  │ I  │ J  │                        
                └───────┴──────────┴───────────┴───────────┴────┴────────────────┴─────────────────┴────┴────┴────┘                        
                """;

            const string expectedUnicodeDouble =
                """
                        ┌──────────┬───────────┬───────────┬────┬────────────────┬─────────────────┬────┬────┬────┐                        
                  0     │ 1        │ 2         │ 3         │ 4  │ 5              │ 6               │ 7  │ 8  │ 9  │                        
                ┌───────┼──────────┴───────────┴───────────┼────╔════════════════╧═════════════════╗────┼────┼────┼────┬────┬────┐         
                │ 0'    │                                  │ 2' ║                                  ║ 4' │ 5' │ 6' │ 7' │ 8' │ 9' │         
                ├───────┤ 1''''''''''''''''''''''''''''''' ├────╨────────────────┐                 ╟────┤    ├────┼────┼────┼────┤         
                │ 0*    │                                  │                     │'''''''''''''''' ║ 2* ║ 3* ║ 4* │ 5* │ 6* │ 7* │ 8*   9* 
                │ 0**   │                                  │ 1*                  │                 ║    ║    ║    │    │    │    │         
                ╔═══════╧══════════╗───────────┬───────────┤                     │                 ╟────╚════╝────┼────┼────┼────┼────┐    
                ║ aaaaaaaaaaaaa    ║ b         │ c         │                     │                 ║ d  │ e  │ f  │ g  │ h  │ i  │ j  │    
                ╚═══════╤══════════╝───────────┼───────────┼────┬════════════════╪═════════════════╝────┼────┼────┼────┴────┴────┴────┘    
                │ A     │ B        │ C         │ D         │ E  │ F              │ G               │ H  │ I  │ J  │                        
                └───────┴──────────┴───────────┴───────────┴────┴────────────────┴─────────────────┴────┴────┴────┘                        
                """;

            var expected = style switch
            {
                BorderStyle.Ascii => expectedAscii,
                BorderStyle.UnicodeSingle => expectedUnicodeSingle,
                BorderStyle.UnicodeDouble => expectedUnicodeDouble,
                _ => throw new ArgumentOutOfRangeException(nameof(style)),
            };
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void DeleteOne()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a").Delete();

            var actual = ptt.ToString();
            Assert.That(actual, Is.EqualTo(string.Empty));
        }

        [Test]
        public static void DeleteQuad()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a").Delete();
            ptt[0, 1].Text("b");
            ptt[1, 0].Text("A");
            ptt[1, 1].Text("B");

            var actual = ptt.ToString();
            const string expected =
                """
                +---+    
                | b |    
                +---+---+
                | A | B |
                +---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void DeleteRow()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a");
            ptt[0, 1].Text("b");
            ptt[1, 0].Text("A");
            ptt[1, 1].Text("B");

            ptt.Row(0).Delete();

            var actual = ptt.ToString();
            const string expected =
                """
                +---+---+
                | A | B |
                +---+---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void DeleteColumn()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].Text("a");
            ptt[0, 1].Text("b");
            ptt[1, 0].Text("A");
            ptt[1, 1].Text("B");

            ptt.Col(0).Delete();

            var actual = ptt.ToString();
            const string expected =
                """
                +---+
                | b |
                +---+
                | B |
                +---+
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
