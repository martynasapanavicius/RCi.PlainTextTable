namespace RCi.PlainTextTable.Tests
{
    [Parallelizable(ParallelScope.All)]
    public static class ControlsTests
    {
        private static readonly string ALPHABET = new
        (
            Enumerable.Range(0, char.MaxValue)
                .Select(x => (char)x)
                .Where(char.IsAsciiLetterLower)
                .OrderBy(x => x)
                .ToArray()
        );

        private static PlainTextTable Create5x5()
        {
            var ptt = new PlainTextTable
            {
                BorderStyle = BorderStyle.UnicodeSingle,
            };
            var i = 0;
            for (var row = 0; row < 5; row++)
            {
                for (var col = 0; col < 5; col++)
                {
                    ptt[row, col].SetText(ALPHABET[i++]);
                }
            }
            return ptt;
        }

        [Test]
        public static void Create5x5Verify()
        {
            var ptt = Create5x5();

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        // ptt movement

        [Test]
        public static void PlainTextTable_Row()
        {
            var ptt = Create5x5();

            ptt.Row(1).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ┢━━━╈━━━╈━━━╈━━━╈━━━┪
                ┃ f ┃ g ┃ h ┃ i ┃ j ┃
                ┡━━━╇━━━╇━━━╇━━━╇━━━┩
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void PlainTextTable_Row_FromEnd()
        {
            var ptt = Create5x5();

            ptt.Row(^2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ┢━━━╈━━━╈━━━╈━━━╈━━━┪
                ┃ p ┃ q ┃ r ┃ s ┃ t ┃
                ┡━━━╇━━━╇━━━╇━━━╇━━━┩
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void PlainTextTable_Column()
        {
            var ptt = Create5x5();

            ptt.Column(1).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┲━━━┱───┬───┬───┐
                │ a ┃ b ┃ c │ d │ e │
                ├───╊━━━╉───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╊━━━╉───┼───┼───┤
                │ k ┃ l ┃ m │ n │ o │
                ├───╊━━━╉───┼───┼───┤
                │ p ┃ q ┃ r │ s │ t │
                ├───╊━━━╉───┼───┼───┤
                │ u ┃ v ┃ w │ x │ y │
                └───┺━━━┹───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void PlainTextTableColumn_FromEnd()
        {
            var ptt = Create5x5();

            ptt.Column(^2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┲━━━┱───┐
                │ a │ b │ c ┃ d ┃ e │
                ├───┼───┼───╊━━━╉───┤
                │ f │ g │ h ┃ i ┃ j │
                ├───┼───┼───╊━━━╉───┤
                │ k │ l │ m ┃ n ┃ o │
                ├───┼───┼───╊━━━╉───┤
                │ p │ q │ r ┃ s ┃ t │
                ├───┼───┼───╊━━━╉───┤
                │ u │ v │ w ┃ x ┃ y │
                └───┴───┴───┺━━━┹───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void PlainTextTable_FirstRow()
        {
            var ptt = Create5x5();

            ptt.FirstRow().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┏━━━┳━━━┳━━━┳━━━┳━━━┓
                ┃ a ┃ b ┃ c ┃ d ┃ e ┃
                ┡━━━╇━━━╇━━━╇━━━╇━━━┩
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void PlainTextTable_LastRow()
        {
            var ptt = Create5x5();

            ptt.LastRow().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ┢━━━╈━━━╈━━━╈━━━╈━━━┪
                ┃ u ┃ v ┃ w ┃ x ┃ y ┃
                ┗━━━┻━━━┻━━━┻━━━┻━━━┛
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void PlainTextTable_FirstColumn()
        {
            var ptt = Create5x5();

            ptt.FirstColumn().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┏━━━┱───┬───┬───┬───┐
                ┃ a ┃ b │ c │ d │ e │
                ┣━━━╉───┼───┼───┼───┤
                ┃ f ┃ g │ h │ i │ j │
                ┣━━━╉───┼───┼───┼───┤
                ┃ k ┃ l │ m │ n │ o │
                ┣━━━╉───┼───┼───┼───┤
                ┃ p ┃ q │ r │ s │ t │
                ┣━━━╉───┼───┼───┼───┤
                ┃ u ┃ v │ w │ x │ y │
                ┗━━━┹───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void PlainTextTable_LastColumn()
        {
            var ptt = Create5x5();

            ptt.LastColumn().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┲━━━┓
                │ a │ b │ c │ d ┃ e ┃
                ├───┼───┼───┼───╊━━━┫
                │ f │ g │ h │ i ┃ j ┃
                ├───┼───┼───┼───╊━━━┫
                │ k │ l │ m │ n ┃ o ┃
                ├───┼───┼───┼───╊━━━┫
                │ p │ q │ r │ s ┃ t ┃
                ├───┼───┼───┼───╊━━━┫
                │ u │ v │ w │ x ┃ y ┃
                └───┴───┴───┴───┺━━━┛
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        // base

        [Test]
        public static void RowBase_Row()
        {
            var ptt = Create5x5();

            ptt.FirstRow().Slice(1, 1).MoveDown().Row().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ┢━━━╈━━━╈━━━╈━━━╈━━━┪
                ┃ f ┃ g ┃ h ┃ i ┃ j ┃
                ┡━━━╇━━━╇━━━╇━━━╇━━━┩
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnBase_Column()
        {
            var ptt = Create5x5();

            ptt.FirstColumn().Slice(1, 1).MoveRight().Column().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┲━━━┱───┬───┬───┐
                │ a ┃ b ┃ c │ d │ e │
                ├───╊━━━╉───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╊━━━╉───┼───┼───┤
                │ k ┃ l ┃ m │ n │ o │
                ├───╊━━━╉───┼───┼───┤
                │ p ┃ q ┃ r │ s │ t │
                ├───╊━━━╉───┼───┼───┤
                │ u ┃ v ┃ w │ x │ y │
                └───┺━━━┹───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        // first

        [Test]
        public static void Row_First()
        {
            var ptt = Create5x5();

            ptt.Row(1).First().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ┢━━━╅───┼───┼───┼───┤
                ┃ f ┃ g │ h │ i │ j │
                ┡━━━╃───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan_First()
        {
            var ptt = Create5x5();

            ptt.Row(1).Slice(1, ^1).First().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───╆━━━╅───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╄━━━╃───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Column_First()
        {
            var ptt = Create5x5();

            ptt.Column(1).First().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┲━━━┱───┬───┬───┐
                │ a ┃ b ┃ c │ d │ e │
                ├───╄━━━╃───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnSpan_First()
        {
            var ptt = Create5x5();

            ptt.Column(1).Slice(1, ^1).First().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───╆━━━╅───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╄━━━╃───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        // last

        [Test]
        public static void Row_Last()
        {
            var ptt = Create5x5();

            ptt.Row(1).Last().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───╆━━━┪
                │ f │ g │ h │ i ┃ j ┃
                ├───┼───┼───┼───╄━━━┩
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan_Last()
        {
            var ptt = Create5x5();

            ptt.Row(1).Slice(1, ^1).Last().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───╆━━━╅───┤
                │ f │ g │ h ┃ i ┃ j │
                ├───┼───┼───╄━━━╃───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Column_Last()
        {
            var ptt = Create5x5();

            ptt.Column(1).Last().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───╆━━━╅───┼───┼───┤
                │ u ┃ v ┃ w │ x │ y │
                └───┺━━━┹───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnSpan_Last()
        {
            var ptt = Create5x5();

            ptt.Column(1).Slice(1, ^1).Last().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───╆━━━╅───┼───┼───┤
                │ p ┃ q ┃ r │ s │ t │
                ├───╄━━━╃───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        // slice

        [Test]
        public static void Row_Slice()
        {
            var variations = new Func<Row, RowSpan>[]
            {
                x => x.Slice(1, 2),
                x => x.Slice(new Range(1, ^2)),
                x => x.Slice(1, ^2),
            };

            foreach (var variation in variations)
            {
                var ptt = Create5x5();

                variation(ptt.Row(1)).SetBorders(Border.Bold);

                var actual = ptt.ToString();
                const string expected =
                    """
                    ┌───┬───┬───┬───┬───┐
                    │ a │ b │ c │ d │ e │
                    ├───╆━━━╈━━━╅───┼───┤
                    │ f ┃ g ┃ h ┃ i │ j │
                    ├───╄━━━╇━━━╃───┼───┤
                    │ k │ l │ m │ n │ o │
                    ├───┼───┼───┼───┼───┤
                    │ p │ q │ r │ s │ t │
                    ├───┼───┼───┼───┼───┤
                    │ u │ v │ w │ x │ y │
                    └───┴───┴───┴───┴───┘
                    """;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public static void Row_SliceSplit()
        {
            var variations = new Func<Row, RowSpan>[]
            {
                x => x.Slice(^4),
                x => x.Slice(1),
            };

            foreach (var variation in variations)
            {
                var ptt = Create5x5();

                variation(ptt.Row(1)).SetBorders(Border.Bold);

                var actual = ptt.ToString();
                const string expected =
                    """
                    ┌───┬───┬───┬───┬───┐
                    │ a │ b │ c │ d │ e │
                    ├───╆━━━╈━━━╈━━━╈━━━┪
                    │ f ┃ g ┃ h ┃ i ┃ j ┃
                    ├───╄━━━╇━━━╇━━━╇━━━┩
                    │ k │ l │ m │ n │ o │
                    ├───┼───┼───┼───┼───┤
                    │ p │ q │ r │ s │ t │
                    ├───┼───┼───┼───┼───┤
                    │ u │ v │ w │ x │ y │
                    └───┴───┴───┴───┴───┘
                    """;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public static void Column_Slice()
        {
            var variations = new Func<Column, ColumnSpan>[]
            {
                x => x.Slice(1, 2),
                x => x.Slice(new Range(1, ^2)),
                x => x.Slice(1, ^2),
            };

            foreach (var variation in variations)
            {
                var ptt = Create5x5();

                variation(ptt.Column(1)).SetBorders(Border.Bold);

                var actual = ptt.ToString();
                const string expected =
                    """
                    ┌───┬───┬───┬───┬───┐
                    │ a │ b │ c │ d │ e │
                    ├───╆━━━╅───┼───┼───┤
                    │ f ┃ g ┃ h │ i │ j │
                    ├───╊━━━╉───┼───┼───┤
                    │ k ┃ l ┃ m │ n │ o │
                    ├───╄━━━╃───┼───┼───┤
                    │ p │ q │ r │ s │ t │
                    ├───┼───┼───┼───┼───┤
                    │ u │ v │ w │ x │ y │
                    └───┴───┴───┴───┴───┘
                    """;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public static void Column_SliceSplit()
        {
            var variations = new Func<Column, ColumnSpan>[]
            {
                x => x.Slice(^4),
                x => x.Slice(1),
            };

            foreach (var variation in variations)
            {
                var ptt = Create5x5();

                variation(ptt.Column(1)).SetBorders(Border.Bold);

                var actual = ptt.ToString();
                const string expected =
                    """
                    ┌───┬───┬───┬───┬───┐
                    │ a │ b │ c │ d │ e │
                    ├───╆━━━╅───┼───┼───┤
                    │ f ┃ g ┃ h │ i │ j │
                    ├───╊━━━╉───┼───┼───┤
                    │ k ┃ l ┃ m │ n │ o │
                    ├───╊━━━╉───┼───┼───┤
                    │ p ┃ q ┃ r │ s │ t │
                    ├───╊━━━╉───┼───┼───┤
                    │ u ┃ v ┃ w │ x │ y │
                    └───┺━━━┹───┴───┴───┘
                    """;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public static void RowSpan_Slice()
        {
            var variations = new Func<RowSpan, RowSpan>[]
            {
                x => x.Slice(0, 2),
                x => x.Slice(new Range(0, ^1)),
                x => x.Slice(0, ^1),
            };

            foreach (var variation in variations)
            {
                var ptt = Create5x5();

                variation(ptt.Row(1).Slice(1, 3)).SetBorders(Border.Bold);

                var actual = ptt.ToString();
                const string expected =
                    """
                    ┌───┬───┬───┬───┬───┐
                    │ a │ b │ c │ d │ e │
                    ├───╆━━━╈━━━╅───┼───┤
                    │ f ┃ g ┃ h ┃ i │ j │
                    ├───╄━━━╇━━━╃───┼───┤
                    │ k │ l │ m │ n │ o │
                    ├───┼───┼───┼───┼───┤
                    │ p │ q │ r │ s │ t │
                    ├───┼───┼───┼───┼───┤
                    │ u │ v │ w │ x │ y │
                    └───┴───┴───┴───┴───┘
                    """;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public static void RowSpan_SliceSplit()
        {
            var variations = new Func<RowSpan, RowSpan>[]
            {
                x => x.Slice(1),
                x => x.Slice(^2),
            };

            foreach (var variation in variations)
            {
                var ptt = Create5x5();

                variation(ptt.Row(1).Slice(1, 3)).SetBorders(Border.Bold);

                var actual = ptt.ToString();
                const string expected =
                    """
                    ┌───┬───┬───┬───┬───┐
                    │ a │ b │ c │ d │ e │
                    ├───┼───╆━━━╈━━━╅───┤
                    │ f │ g ┃ h ┃ i ┃ j │
                    ├───┼───╄━━━╇━━━╃───┤
                    │ k │ l │ m │ n │ o │
                    ├───┼───┼───┼───┼───┤
                    │ p │ q │ r │ s │ t │
                    ├───┼───┼───┼───┼───┤
                    │ u │ v │ w │ x │ y │
                    └───┴───┴───┴───┴───┘
                    """;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public static void ColumnSpan_Slice()
        {
            var variations = new Func<ColumnSpan, ColumnSpan>[]
            {
                x => x.Slice(0, 2),
                x => x.Slice(new Range(0, ^1)),
                x => x.Slice(0, ^1),
            };

            foreach (var variation in variations)
            {
                var ptt = Create5x5();

                variation(ptt.Column(1).Slice(1, 3)).SetBorders(Border.Bold);

                var actual = ptt.ToString();
                const string expected =
                    """
                    ┌───┬───┬───┬───┬───┐
                    │ a │ b │ c │ d │ e │
                    ├───╆━━━╅───┼───┼───┤
                    │ f ┃ g ┃ h │ i │ j │
                    ├───╊━━━╉───┼───┼───┤
                    │ k ┃ l ┃ m │ n │ o │
                    ├───╄━━━╃───┼───┼───┤
                    │ p │ q │ r │ s │ t │
                    ├───┼───┼───┼───┼───┤
                    │ u │ v │ w │ x │ y │
                    └───┴───┴───┴───┴───┘
                    """;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public static void ColumnSpan_SliceSplit()
        {
            var variations = new Func<ColumnSpan, ColumnSpan>[]
            {
                x => x.Slice(1),
                x => x.Slice(^2),
            };

            foreach (var variation in variations)
            {
                var ptt = Create5x5();

                variation(ptt.Column(1).Slice(1, 3)).SetBorders(Border.Bold);

                var actual = ptt.ToString();
                const string expected =
                    """
                    ┌───┬───┬───┬───┬───┐
                    │ a │ b │ c │ d │ e │
                    ├───┼───┼───┼───┼───┤
                    │ f │ g │ h │ i │ j │
                    ├───╆━━━╅───┼───┼───┤
                    │ k ┃ l ┃ m │ n │ o │
                    ├───╊━━━╉───┼───┼───┤
                    │ p ┃ q ┃ r │ s │ t │
                    ├───╄━━━╃───┼───┼───┤
                    │ u │ v │ w │ x │ y │
                    └───┴───┴───┴───┴───┘
                    """;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        // move

        [Test]
        public static void Row_MoveUp()
        {
            var ptt = Create5x5();

            ptt.LastRow().MoveUp().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ┢━━━╈━━━╈━━━╈━━━╈━━━┪
                ┃ p ┃ q ┃ r ┃ s ┃ t ┃
                ┡━━━╇━━━╇━━━╇━━━╇━━━┩
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Row_MoveDown()
        {
            var ptt = Create5x5();

            ptt.FirstRow().MoveDown().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ┢━━━╈━━━╈━━━╈━━━╈━━━┪
                ┃ f ┃ g ┃ h ┃ i ┃ j ┃
                ┡━━━╇━━━╇━━━╇━━━╇━━━┩
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Column_MoveLeft()
        {
            var ptt = Create5x5();

            ptt.LastColumn().MoveLeft().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┲━━━┱───┐
                │ a │ b │ c ┃ d ┃ e │
                ├───┼───┼───╊━━━╉───┤
                │ f │ g │ h ┃ i ┃ j │
                ├───┼───┼───╊━━━╉───┤
                │ k │ l │ m ┃ n ┃ o │
                ├───┼───┼───╊━━━╉───┤
                │ p │ q │ r ┃ s ┃ t │
                ├───┼───┼───╊━━━╉───┤
                │ u │ v │ w ┃ x ┃ y │
                └───┴───┴───┺━━━┹───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Column_MoveRight()
        {
            var ptt = Create5x5();

            ptt.FirstColumn().MoveRight().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┲━━━┱───┬───┬───┐
                │ a ┃ b ┃ c │ d │ e │
                ├───╊━━━╉───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╊━━━╉───┼───┼───┤
                │ k ┃ l ┃ m │ n │ o │
                ├───╊━━━╉───┼───┼───┤
                │ p ┃ q ┃ r │ s │ t │
                ├───╊━━━╉───┼───┼───┤
                │ u ┃ v ┃ w │ x │ y │
                └───┺━━━┹───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Row_MoveUpToFirst()
        {
            var ptt = Create5x5();

            ptt.Row(3).MoveUpToFirst().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┏━━━┳━━━┳━━━┳━━━┳━━━┓
                ┃ a ┃ b ┃ c ┃ d ┃ e ┃
                ┡━━━╇━━━╇━━━╇━━━╇━━━┩
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Row_MoveDownToLast()
        {
            var ptt = Create5x5();

            ptt.Row(3).MoveDownToLast().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ┢━━━╈━━━╈━━━╈━━━╈━━━┪
                ┃ u ┃ v ┃ w ┃ x ┃ y ┃
                ┗━━━┻━━━┻━━━┻━━━┻━━━┛
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Column_MoveLeftToFirst()
        {
            var ptt = Create5x5();

            ptt.Column(3).MoveLeftToFirst().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┏━━━┱───┬───┬───┬───┐
                ┃ a ┃ b │ c │ d │ e │
                ┣━━━╉───┼───┼───┼───┤
                ┃ f ┃ g │ h │ i │ j │
                ┣━━━╉───┼───┼───┼───┤
                ┃ k ┃ l │ m │ n │ o │
                ┣━━━╉───┼───┼───┼───┤
                ┃ p ┃ q │ r │ s │ t │
                ┣━━━╉───┼───┼───┼───┤
                ┃ u ┃ v │ w │ x │ y │
                ┗━━━┹───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Column_MoveRightToLast()
        {
            var ptt = Create5x5();

            ptt.Column(3).MoveRightToLast().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┲━━━┓
                │ a │ b │ c │ d ┃ e ┃
                ├───┼───┼───┼───╊━━━┫
                │ f │ g │ h │ i ┃ j ┃
                ├───┼───┼───┼───╊━━━┫
                │ k │ l │ m │ n ┃ o ┃
                ├───┼───┼───┼───╊━━━┫
                │ p │ q │ r │ s ┃ t ┃
                ├───┼───┼───┼───╊━━━┫
                │ u │ v │ w │ x ┃ y ┃
                └───┴───┴───┴───┺━━━┛
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan_MoveUp()
        {
            var ptt = Create5x5();

            ptt.LastRow().Slice(1, 2).MoveUp().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───╆━━━╈━━━╅───┼───┤
                │ p ┃ q ┃ r ┃ s │ t │
                ├───╄━━━╇━━━╃───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan_MoveDown()
        {
            var ptt = Create5x5();

            ptt.FirstRow().Slice(1, 2).MoveDown().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───╆━━━╈━━━╅───┼───┤
                │ f ┃ g ┃ h ┃ i │ j │
                ├───╄━━━╇━━━╃───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnSpan_MoveLeft()
        {
            var ptt = Create5x5();

            ptt.LastColumn().Slice(1, 2).MoveLeft().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───╆━━━╅───┤
                │ f │ g │ h ┃ i ┃ j │
                ├───┼───┼───╊━━━╉───┤
                │ k │ l │ m ┃ n ┃ o │
                ├───┼───┼───╄━━━╃───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnSpan_MoveRight()
        {
            var ptt = Create5x5();

            ptt.FirstColumn().Slice(1, 2).MoveRight().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───╆━━━╅───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╊━━━╉───┼───┼───┤
                │ k ┃ l ┃ m │ n │ o │
                ├───╄━━━╃───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan_MoveUpToFirst()
        {
            var ptt = Create5x5();

            ptt.Row(3).Slice(1, 2).MoveUpToFirst().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┲━━━┳━━━┱───┬───┐
                │ a ┃ b ┃ c ┃ d │ e │
                ├───╄━━━╇━━━╃───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan_MoveDownToLast()
        {
            var ptt = Create5x5();

            ptt.Row(3).Slice(1, 2).MoveDownToLast().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───╆━━━╈━━━╅───┼───┤
                │ u ┃ v ┃ w ┃ x │ y │
                └───┺━━━┻━━━┹───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnSpan_MoveLeftToFirst()
        {
            var ptt = Create5x5();

            ptt.Column(3).Slice(1, 2).MoveLeftToFirst().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ┢━━━╅───┼───┼───┼───┤
                ┃ f ┃ g │ h │ i │ j │
                ┣━━━╉───┼───┼───┼───┤
                ┃ k ┃ l │ m │ n │ o │
                ┡━━━╃───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnSpan_MoveRightToLast()
        {
            var ptt = Create5x5();

            ptt.Column(3).Slice(1, 2).MoveRightToLast().SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───╆━━━┪
                │ f │ g │ h │ i ┃ j ┃
                ├───┼───┼───┼───╊━━━┫
                │ k │ l │ m │ n ┃ o ┃
                ├───┼───┼───┼───╄━━━┩
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        // skip

        [Test]
        public static void Row_Skip()
        {
            var ptt = Create5x5();

            ptt.Row(1).Skip(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───╆━━━╈━━━╈━━━┪
                │ f │ g ┃ h ┃ i ┃ j ┃
                ├───┼───╄━━━╇━━━╇━━━┩
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Row_SkipLast()
        {
            var ptt = Create5x5();

            ptt.Row(1).SkipLast(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ┢━━━╈━━━╈━━━╅───┼───┤
                ┃ f ┃ g ┃ h ┃ i │ j │
                ┡━━━╇━━━╇━━━╃───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Column_Skip()
        {
            var ptt = Create5x5();

            ptt.Column(1).Skip(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───╆━━━╅───┼───┼───┤
                │ k ┃ l ┃ m │ n │ o │
                ├───╊━━━╉───┼───┼───┤
                │ p ┃ q ┃ r │ s │ t │
                ├───╊━━━╉───┼───┼───┤
                │ u ┃ v ┃ w │ x │ y │
                └───┺━━━┹───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Column_SkipLast()
        {
            var ptt = Create5x5();

            ptt.Column(1).SkipLast(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┲━━━┱───┬───┬───┐
                │ a ┃ b ┃ c │ d │ e │
                ├───╊━━━╉───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╊━━━╉───┼───┼───┤
                │ k ┃ l ┃ m │ n │ o │
                ├───╄━━━╃───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan_Skip()
        {
            var ptt = Create5x5();

            ptt.Row(1).Slice(1, ^1).Skip(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───╆━━━╅───┤
                │ f │ g │ h ┃ i ┃ j │
                ├───┼───┼───╄━━━╃───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan_SkipLast()
        {
            var ptt = Create5x5();

            ptt.Row(1).Slice(1, ^1).SkipLast(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───╆━━━╅───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╄━━━╃───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnSpan_Skip()
        {
            var ptt = Create5x5();

            ptt.Column(1).Slice(1, ^1).Skip(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───┼───┼───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───╆━━━╅───┼───┼───┤
                │ p ┃ q ┃ r │ s │ t │
                ├───╄━━━╃───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnSpan_SkipLast()
        {
            var ptt = Create5x5();

            ptt.Column(1).Slice(1, ^1).SkipLast(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───╆━━━╅───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╄━━━╃───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        // take

        [Test]
        public static void Row_Take()
        {
            var ptt = Create5x5();

            ptt.Row(1).Take(3).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ┢━━━╈━━━╈━━━╅───┼───┤
                ┃ f ┃ g ┃ h ┃ i │ j │
                ┡━━━╇━━━╇━━━╃───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Row_TakeLast()
        {
            var ptt = Create5x5();

            ptt.Row(1).TakeLast(3).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───╆━━━╈━━━╈━━━┪
                │ f │ g ┃ h ┃ i ┃ j ┃
                ├───┼───╄━━━╇━━━╇━━━┩
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Column_Take()
        {
            var ptt = Create5x5();

            ptt.Column(1).Take(3).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┲━━━┱───┬───┬───┐
                │ a ┃ b ┃ c │ d │ e │
                ├───╊━━━╉───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╊━━━╉───┼───┼───┤
                │ k ┃ l ┃ m │ n │ o │
                ├───╄━━━╃───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void Column_TakeLast()
        {
            var ptt = Create5x5();

            ptt.Column(1).SkipLast(3).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┲━━━┱───┬───┬───┐
                │ a ┃ b ┃ c │ d │ e │
                ├───╊━━━╉───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╄━━━╃───┼───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan_Take()
        {
            var ptt = Create5x5();

            ptt.Row(1).Slice(1, ^1).Take(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───╆━━━╈━━━╅───┼───┤
                │ f ┃ g ┃ h ┃ i │ j │
                ├───╄━━━╇━━━╃───┼───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void RowSpan_TakeLast()
        {
            var ptt = Create5x5();

            ptt.Row(1).Slice(1, ^1).TakeLast(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───╆━━━╈━━━╅───┤
                │ f │ g ┃ h ┃ i ┃ j │
                ├───┼───╄━━━╇━━━╃───┤
                │ k │ l │ m │ n │ o │
                ├───┼───┼───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnSpan_Take()
        {
            var ptt = Create5x5();

            ptt.Column(1).Slice(1, ^1).Take(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───╆━━━╅───┼───┼───┤
                │ f ┃ g ┃ h │ i │ j │
                ├───╊━━━╉───┼───┼───┤
                │ k ┃ l ┃ m │ n │ o │
                ├───╄━━━╃───┼───┼───┤
                │ p │ q │ r │ s │ t │
                ├───┼───┼───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public static void ColumnSpan_TakeLast()
        {
            var ptt = Create5x5();

            ptt.Column(1).Slice(1, ^1).TakeLast(2).SetBorders(Border.Bold);

            var actual = ptt.ToString();
            const string expected =
                """
                ┌───┬───┬───┬───┬───┐
                │ a │ b │ c │ d │ e │
                ├───┼───┼───┼───┼───┤
                │ f │ g │ h │ i │ j │
                ├───╆━━━╅───┼───┼───┤
                │ k ┃ l ┃ m │ n │ o │
                ├───╊━━━╉───┼───┼───┤
                │ p ┃ q ┃ r │ s │ t │
                ├───╄━━━╃───┼───┼───┤
                │ u │ v │ w │ x │ y │
                └───┴───┴───┴───┴───┘
                """;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
