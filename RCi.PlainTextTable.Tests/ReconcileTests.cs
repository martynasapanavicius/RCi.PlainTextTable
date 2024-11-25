using System.Collections.Immutable;

namespace RCi.PlainTextTable.Tests
{
    [Parallelizable(ParallelScope.All)]
    public static class ReconcileTests
    {
        private static readonly ImmutableArray<(char Expected, string Neighbours)> TABLE_SINGLE =
        [
            #region // data

            (' ', "O O O O"),
            ('╴', "N O O O"),
            ('╵', "O N O O"),
            ('╶', "O O N O"),
            ('╷', "O O O N"),
            ('╸', "B O O O"),
            ('╹', "O B O O"),
            ('╺', "O O B O"),
            ('╻', "O O O B"),
            ('─', "N O N O"),
            ('━', "B O B O"),
            ('╼', "N O B O"),
            ('╾', "B O N O"),
            ('│', "O N O N"),
            ('┃', "O B O B"),
            ('╽', "O N O B"),
            ('╿', "O B O N"),
            ('┌', "O O N N"),
            ('┍', "O O B N"),
            ('┎', "O O N B"),
            ('┏', "O O B B"),
            ('┐', "N O O N"),
            ('┑', "B O O N"),
            ('┒', "N O O B"),
            ('┓', "B O O B"),
            ('└', "O N N O"),
            ('┕', "O N B O"),
            ('┖', "O B N O"),
            ('┗', "O B B O"),
            ('┘', "N N O O"),
            ('┙', "B N O O"),
            ('┚', "N B O O"),
            ('┛', "B B O O"),
            ('├', "O N N N"),
            ('┝', "O N B N"),
            ('┞', "O B N N"),
            ('┟', "O N N B"),
            ('┠', "O B N B"),
            ('┡', "O B B N"),
            ('┢', "O N B B"),
            ('┣', "O B B B"),
            ('┤', "N N O N"),
            ('┥', "B N O N"),
            ('┦', "N B O N"),
            ('┧', "N N O B"),
            ('┨', "N B O B"),
            ('┩', "B B O N"),
            ('┪', "B N O B"),
            ('┫', "B B O B"),
            ('┬', "N O N N"),
            ('┭', "B O N N"),
            ('┮', "N O B N"),
            ('┯', "B O B N"),
            ('┰', "N O N B"),
            ('┱', "B O N B"),
            ('┲', "N O B B"),
            ('┳', "B O B B"),
            ('┴', "N N N O"),
            ('┵', "B N N O"),
            ('┶', "N N B O"),
            ('┷', "B N B O"),
            ('┸', "N B N O"),
            ('┹', "B B N O"),
            ('┺', "N B B O"),
            ('┻', "B B B O"),
            ('┼', "N N N N"),
            ('┽', "B N N N"),
            ('┾', "N N B N"),
            ('┿', "B N B N"),
            ('╀', "N B N N"),
            ('╁', "N N N B"),
            ('╂', "N B N B"),
            ('╃', "B B N N"),
            ('╄', "N B B N"),
            ('╅', "B N N B"),
            ('╆', "N N B B"),
            ('╇', "B B B N"),
            ('╈', "B N B B"),
            ('╉', "B B N B"),
            ('╊', "N B B B"),
            ('╋', "B B B B"),
            
            #endregion
        ];

        private static readonly ImmutableArray<(char Expected, string Neighbours)> TABLE_DOUBLE =
        [
            #region // data

            (' ', "O O O O"),
            ('╷', "O O O N"),
            (' ', "O O O W"),
            ('╶', "O O N O"),
            ('┌', "O O N N"),
            ('╓', "O O N W"),
            (' ', "O O W O"),
            ('╒', "O O W N"),
            ('╔', "O O W W"),
            ('╵', "O N O O"),
            ('│', "O N O N"),
            ('│', "O N O W"),
            ('└', "O N N O"),
            ('├', "O N N N"),
            ('├', "O N N W"),
            ('╘', "O N W O"),
            ('╞', "O N W N"),
            ('╔', "O N W W"),
            (' ', "O W O O"),
            ('│', "O W O N"),
            ('║', "O W O W"),
            ('╙', "O W N O"),
            ('├', "O W N N"),
            ('╟', "O W N W"),
            ('╚', "O W W O"),
            ('╚', "O W W N"),
            ('╠', "O W W W"),
            ('╴', "N O O O"),
            ('┐', "N O O N"),
            ('╖', "N O O W"),
            ('─', "N O N O"),
            ('┬', "N O N N"),
            ('╥', "N O N W"),
            ('─', "N O W O"),
            ('┬', "N O W N"),
            ('╔', "N O W W"),
            ('┘', "N N O O"),
            ('┤', "N N O N"),
            ('┤', "N N O W"),
            ('┴', "N N N O"),
            ('┼', "N N N N"),
            ('┼', "N N N W"),
            ('┘', "N N W O"),
            ('┤', "N N W N"),
            ('╔', "N N W W"),
            ('╜', "N W O O"),
            ('┐', "N W O N"),
            ('╢', "N W O W"),
            ('╨', "N W N O"),
            ('┼', "N W N N"),
            ('╫', "N W N W"),
            ('╚', "N W W O"),
            ('╚', "N W W N"),
            ('╠', "N W W W"),
            (' ', "W O O O"),
            ('╕', "W O O N"),
            ('╗', "W O O W"),
            ('─', "W O N O"),
            ('┬', "W O N N"),
            ('╗', "W O N W"),
            ('═', "W O W O"),
            ('╤', "W O W N"),
            ('╦', "W O W W"),
            ('╛', "W N O O"),
            ('╡', "W N O N"),
            ('╗', "W N O W"),
            ('┴', "W N N O"),
            ('┼', "W N N N"),
            ('╗', "W N N W"),
            ('╧', "W N W O"),
            ('╪', "W N W N"),
            ('╦', "W N W W"),
            ('╝', "W W O O"),
            ('╝', "W W O N"),
            ('╣', "W W O W"),
            ('╝', "W W N O"),
            ('╝', "W W N N"),
            ('╣', "W W N W"),
            ('╩', "W W W O"),
            ('╩', "W W W N"),
            ('╬', "W W W W"),

            #endregion
        ];

        private static string BuildTable((char Expected, string Neighbours) testCase, bool isDoubleStyle)
        {
            static Border ParseBorder(char code, bool isDoubleStyle) => isDoubleStyle
                ? code switch
                {
                    'O' => Border.None,
                    'N' => Border.Normal,
                    'W' => Border.Bold,
                    _ => throw new NotSupportedException(nameof(code)),
                }
                : code switch
                {
                    'O' => Border.None,
                    'N' => Border.Normal,
                    'B' => Border.Bold,
                    _ => throw new NotSupportedException(nameof(code)),
                };

            var left = ParseBorder(testCase.Neighbours[0], isDoubleStyle);
            var top = ParseBorder(testCase.Neighbours[2], isDoubleStyle);
            var right = ParseBorder(testCase.Neighbours[4], isDoubleStyle);
            var bottom = ParseBorder(testCase.Neighbours[6], isDoubleStyle);

            var ptt = new PlainTextTable
            {
                BorderStyle = isDoubleStyle
                    ? BorderStyle.UnicodeDouble
                    : BorderStyle.UnicodeSingle,
                DefaultBorders = new Borders(Border.Normal),
                DefaultMargin = new Margin(0),
            };

            ptt[0, 0].SetText("a");
            ptt[0, 1].SetText("b");
            ptt[0, 2].SetText("c");
            ptt[0, 3].SetText("d");

            ptt[1, 0].SetText("A");
            ptt[1, 1].SetText("B").SetRightBorder(top).SetBottomBorder(left);
            ptt[1, 2].SetText("C").SetLeftBorder(top).SetBottomBorder(right);
            ptt[1, 3].SetText("D");

            ptt[2, 0].SetText("0");
            ptt[2, 1].SetText("1").SetRightBorder(bottom).SetTopBorder(left);
            ptt[2, 2].SetText("2").SetLeftBorder(bottom).SetTopBorder(right);
            ptt[2, 3].SetText("3");

            ptt[3, 0].SetText("!");
            ptt[3, 1].SetText("@");
            ptt[3, 2].SetText("#");
            ptt[3, 3].SetText("$");

            return ptt.ToString();
        }

        private static void AssertActual(string pttStr, char expected)
        {
            TestContext.WriteLine(pttStr);
            Assert.That(pttStr.Length, Is.EqualTo(97));

            var lines = pttStr.Split(Environment.NewLine);
            Assert.That(lines.Length, Is.EqualTo(9));

            var actual = lines[4][4];
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(TABLE_SINGLE))]
        public static void ReconcileBorderCornersSingle((char Expected, string Neighbours) testCase)
        {
            var pttStr = BuildTable(testCase, false);
            AssertActual(pttStr, testCase.Expected);
        }

        [TestCaseSource(nameof(TABLE_DOUBLE))]
        public static void ReconcileBorderCornersDouble((char Expected, string Neighbours) testCase)
        {
            var pttStr = BuildTable(testCase, true);
            AssertActual(pttStr, testCase.Expected);
        }
    }
}
