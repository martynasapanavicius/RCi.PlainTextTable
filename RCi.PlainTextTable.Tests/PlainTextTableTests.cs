namespace RCi.PlainTextTable.Tests
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
            ptt[0, 0].SetText("a");

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
            ptt[99, 99].SetText("a");

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
            ptt[0, 0].SetText("a");
            ptt[0, 1].SetText("b");

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
            ptt[0, 0].SetText("a");
            ptt[0, 99].SetText("b");

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
            ptt[0, 0].SetText("a");
            ptt[1, 0].SetText("b");

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
            ptt[0, 0].SetText("a");
            ptt[1, 0].SetText("b");

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
            ptt[0, 0].SetText("a");
            ptt[0, 1].SetText("b");
            ptt[1, 0].SetText("A");
            ptt[1, 1].SetText("B");

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
            ptt[0, 0].SetText("a");
            ptt[0, 99].SetText("b");
            ptt[99, 0].SetText("A");
            ptt[99, 99].SetText("B");

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
            ptt[0, 0].SetText("a").SetColumnSpan(2);
            ptt[0, 1].SetText("b");
            ptt[1, 0].SetText("A");
            ptt[1, 1].SetText("B");

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
            ptt[0, 0].SetText("a").SetRowSpan(2);
            ptt[0, 1].SetText("b");
            ptt[1, 0].SetText("A");
            ptt[1, 1].SetText("B");

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

            ptt[0, 0].SetText("a");
            ptt[0, 1].SetText("b").SetColumnSpan(2).SetRowSpan(2);
            ptt[0, 2].SetText("c");
            ptt[0, 3].SetText("d");

            ptt[1, 0].SetText("A");
            ptt[1, 1].SetText("B");
            ptt[1, 2].SetText("C");
            ptt[1, 3].SetText("D");

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

            ptt[0, 0].SetText("a");
            ptt[0, 1].SetText("b").SetColumnSpan(2).SetRowSpan(2);
            ptt[0, 2].SetText("c");

            ptt[1, 0].SetText("A").SetColumnSpan(2).SetRowSpan(2);
            ptt[1, 1].SetText("B");

            ptt[2, 0].SetText("0");
            ptt[2, 1].SetText("1");

            ptt[3, 0].SetText("!");
            ptt[3, 1].SetText("@");
            ptt[3, 2].SetText("#");
            ptt[3, 3].SetText("$");

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

            ptt[0, 0].SetText("a");
            ptt[0, 1].SetText("b").SetColumnSpan(2).SetRowSpan(2);
            ptt[0, 2].SetText("c");

            ptt[1, 0].SetText("A").SetColumnSpan(2).SetRowSpan(2);
            ptt[1, 1].SetText("B");

            ptt[2, 0].SetText("0");
            ptt[2, 1].SetText("1");

            ptt[3, 0].SetText("!");
            ptt[3, 1].SetText("@");
            ptt[3, 2].SetText("#");
            ptt[3, 3].SetText("$");

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

            ptt[0, 0].SetText("1234").SetColumnSpan(3);

            ptt[1, 0].SetText("a");
            ptt[1, 1].SetText("b");
            ptt[1, 2].SetText("c");

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

            ptt[0, 0].SetText("12345").SetColumnSpan(3);

            ptt[1, 0].SetText("a");
            ptt[1, 1].SetText("b");
            ptt[1, 2].SetText("c");

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

            ptt[0, 0].SetText("123456").SetColumnSpan(3);

            ptt[1, 0].SetText("a");
            ptt[1, 1].SetText("b");
            ptt[1, 2].SetText("c");

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

            ptt[0, 0].SetText("1234567").SetColumnSpan(3);

            ptt[1, 0].SetText("a");
            ptt[1, 1].SetText("b");
            ptt[1, 2].SetText("c");

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

            ptt[0, 0].SetText("12345678").SetColumnSpan(3);

            ptt[1, 0].SetText("a");
            ptt[1, 1].SetText("b");
            ptt[1, 2].SetText("c");

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

            ptt[0, 0].SetText("""
                           0
                           1
                           2
                           3
                           """).SetRowSpan(3);

            ptt[0, 1].SetText("a");
            ptt[1, 1].SetText("b");
            ptt[2, 1].SetText("c");

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

            ptt[0, 0].SetText("""
                           0
                           1
                           2
                           3
                           4
                           """).SetRowSpan(3);

            ptt[0, 1].SetText("a");
            ptt[1, 1].SetText("b");
            ptt[2, 1].SetText("c");

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

            ptt[0, 0].SetText("""
                           0
                           1
                           2
                           3
                           4
                           5
                           """).SetRowSpan(3);

            ptt[0, 1].SetText("a");
            ptt[1, 1].SetText("b");
            ptt[2, 1].SetText("c");

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

            ptt[0, 0].SetText("""
                           0
                           1
                           2
                           3
                           4
                           5
                           6
                           """).SetRowSpan(3);

            ptt[0, 1].SetText("a");
            ptt[1, 1].SetText("b");
            ptt[2, 1].SetText("c");

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

            ptt[0, 0].SetText("""
                           0
                           1
                           2
                           3
                           4
                           5
                           6
                           7
                           """).SetRowSpan(3);

            ptt[0, 1].SetText("a");
            ptt[1, 1].SetText("b");
            ptt[2, 1].SetText("c");

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

            ptt[0, 0].SetText("0").SetBorders(Border.None);
            ptt[0, 1].SetText("1");
            ptt[0, 2].SetText("2");
            ptt[0, 3].SetText("3");
            ptt[0, 4].SetText("4");
            ptt[0, 5].SetText("5");
            ptt[0, 6].SetText("6");
            ptt[0, 7].SetText("7");
            ptt[0, 8].SetText("8");
            ptt[0, 9].SetText("9");

            ptt[1, 0].SetText("0'");
            ptt[1, 1].SetText("1'''''''''''''''''''''''''''''''").SetColumnSpan(3).SetRowSpan(2);
            ptt[1, 2].SetText("2'");
            ptt[1, 3].SetText("3'''''''''''''''''''''''''''''''").SetBorders(Border.Bold).SetColumnSpan(2).SetRowSpan(3);
            ptt[1, 4].SetText("4'");
            ptt[1, 5].SetText("5'").SetBorders(Border.Normal, Border.Normal, Border.Normal, Border.None);
            ptt[1, 6].SetText("6'");
            ptt[1, 7].SetText("7'");
            ptt[1, 8].SetText("8'");
            ptt[1, 9].SetText("9'");

            ptt[2, 0].SetText($"0*{Environment.NewLine}0**");
            ptt[2, 1].SetText("1*").SetColumnSpan(2).SetRowSpan(2);
            ptt[2, 2].SetText("2*");
            ptt[2, 3].SetText("3*").SetBorders(Border.Bold, Border.None, Border.Bold, Border.Bold);
            ptt[2, 4].SetText("4*");
            ptt[2, 5].SetText("5*");
            ptt[2, 6].SetText("6*");
            ptt[2, 7].SetText("7*");
            ptt[2, 8].SetText("8*").SetBorders(Border.None);
            ptt[2, 9].SetText("9*").SetBorders(Border.None);

            ptt[3, 0].SetText("aaaaaaaaaaaaa").SetBorders(Border.Bold).SetColumnSpan(2);
            ptt[3, 1].SetText("b");
            ptt[3, 2].SetText("c");
            ptt[3, 3].SetText("d");
            ptt[3, 4].SetText("e");
            ptt[3, 5].SetText("f");
            ptt[3, 6].SetText("g");
            ptt[3, 7].SetText("h");
            ptt[3, 8].SetText("i");
            ptt[3, 9].SetText("j");

            ptt[4, 0].SetText("A");
            ptt[4, 1].SetText("B");
            ptt[4, 2].SetText("C");
            ptt[4, 3].SetText("D");
            ptt[4, 4].SetText("E");
            ptt[4, 5].SetText("F");
            ptt[4, 6].SetText("G");
            ptt[4, 7].SetText("H");
            ptt[4, 8].SetText("I");
            ptt[4, 9].SetText("J");

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
            ptt[0, 0].SetText("a").Delete();

            var actual = ptt.ToString();
            Assert.That(actual, Is.EqualTo(string.Empty));
        }

        [Test]
        public static void DeleteQuad()
        {
            var ptt = new PlainTextTable();
            ptt[0, 0].SetText("a").Delete();
            ptt[0, 1].SetText("b");
            ptt[1, 0].SetText("A");
            ptt[1, 1].SetText("B");

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
            ptt[0, 0].SetText("a");
            ptt[0, 1].SetText("b");
            ptt[1, 0].SetText("A");
            ptt[1, 1].SetText("B");

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
            ptt[0, 0].SetText("a");
            ptt[0, 1].SetText("b");
            ptt[1, 0].SetText("A");
            ptt[1, 1].SetText("B");

            ptt.Column(0).Delete();

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
