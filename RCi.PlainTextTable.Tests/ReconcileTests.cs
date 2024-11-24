using System.Collections.Immutable;

namespace RCi.PlainTextTable.Tests
{
    [Parallelizable(ParallelScope.All)]
    public static class ReconcileTests
    {
        #region // available unicode characters

        // ╴    N O O O
        // ╵    O N O O
        // ╶    O O N O
        // ╷    O O O N
        // ╸    B O O O
        // ╹    O B O O
        // ╺    O O B O
        // ╻    O O O B

        // ─    N O N O
        // ━    B O B O
        // ╼    N O B O
        // ╾    B O N O

        // │    O N O N
        // ┃    O B O B
        // ╽    O N O B
        // ╿    O B O N

        // ┌    O O N N
        // ┍    O O B N
        // ┎    O O N B
        // ┏    O O B B

        // ┐    N O O N
        // ┑    B O O N
        // ┒    N O O B
        // ┓    B O O B

        // └    O N N O
        // ┕    O N B O
        // ┖    O B N O
        // ┗    O B B O

        // ┘    N N O O
        // ┙    B N O O
        // ┚    N B O O
        // ┛    B B O O

        // ├    O N N N 
        // ┝    O N B N
        // ┞    O B N N
        // ┟    O N N B
        // ┠    O B N B
        // ┡    O B B N
        // ┢    O N B B
        // ┣    O B B B

        // ┤    N N O N
        // ┥    B N O N
        // ┦    N B O N
        // ┧    N N O B
        // ┨    N B O B
        // ┩    B B O N
        // ┪    B N O B
        // ┫    B B O B

        // ┬    N O N N
        // ┭    B O N N
        // ┮    N O B N
        // ┯    B O B N
        // ┰    N O N B
        // ┱    B O N B
        // ┲    N O B B
        // ┳    B O B B

        // ┴    N N N O
        // ┵    B N N O
        // ┶    N N B O
        // ┷    B N B O
        // ┸    N B N O
        // ┹    B B N O
        // ┺    N B B O
        // ┻    B B B O

        // ┼    N N N N
        // ┽    B N N N
        // ┾    N N B N
        // ┿    B N B N
        // ╀    N B N N
        // ╁    N N N B
        // ╂    N B N B
        // ╃    B B N N
        // ╄    N B B N
        // ╅    B N N B
        // ╆    N N B B
        // ╇    B B B N
        // ╈    B N B B
        // ╉    B B N B
        // ╊    N B B B
        // ╋    B B B B

        // ═    W O W O
        // ║    O W O W

        // ╒    O O W N
        // ╓    O O N W
        // ╔    O O W W

        // ╕    W O O N
        // ╖    N O O W
        // ╗    W O O W

        // ╘    O N W O
        // ╙    O W N O
        // ╚    O W W O

        // ╛    W N O O
        // ╜    N W O O
        // ╝    W W O O

        // ╞    O N W N
        // ╟    O W N W
        // ╠    O W W W

        // ╡    W N O N
        // ╢    N W O W
        // ╣    W W O W

        // ╤    W O W N
        // ╥    N O N W
        // ╦    W O W W

        // ╧    W N W O
        // ╨    N W N O
        // ╩    W W W O

        // ╪    W N W N
        // ╫    N W N W
        // ╬    W W W W

        #endregion

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
            (' ', "O O O N"), // adjusted
            (' ', "O O O W"), // adjusted
            (' ', "O O N O"), // adjusted
            ('┌', "O O N N"),
            ('╓', "O O N W"),
            (' ', "O O W O"), // adjusted
            ('╒', "O O W N"),
            ('╔', "O O W W"),
            (' ', "O N O O"), // adjusted
            ('│', "O N O N"),
            (' ', "O N O W"), // adjusted
            ('└', "O N N O"),
            ('├', "O N N N"),
            ('└', "O N N W"), // adjusted
            ('╘', "O N W O"),
            ('╞', "O N W N"),
            ('╔', "O N W W"), // adjusted
            (' ', "O W O O"), // adjusted
            (' ', "O W O N"), // adjusted
            ('║', "O W O W"),
            ('╙', "O W N O"),
            ('┌', "O W N N"), // adjusted
            ('╟', "O W N W"),
            ('╚', "O W W O"),
            ('╚', "O W W N"), // adjusted
            ('╠', "O W W W"),
            (' ', "N O O O"), // adjusted
            ('┐', "N O O N"),
            ('╖', "N O O W"),
            ('─', "N O N O"),
            ('┬', "N O N N"),
            ('╥', "N O N W"),
            (' ', "N O W O"), // adjusted
            ('┐', "N O W N"), // adjusted
            ('╔', "N O W W"), // adjusted
            ('┘', "N N O O"),
            ('┤', "N N O N"),
            ('┘', "N N O W"), // adjusted
            ('┴', "N N N O"),
            ('┼', "N N N N"),
            ('┴', "N N N W"), // adjusted
            ('┘', "N N W O"), // adjusted
            ('┤', "N N W N"), // adjusted
            ('╔', "N N W W"), // adjusted
            ('╜', "N W O O"),
            ('┐', "N W O N"), // adjusted
            ('╢', "N W O W"),
            ('╨', "N W N O"),
            ('┬', "N W N N"), // adjusted
            ('╫', "N W N W"),
            ('╚', "N W W O"), // adjusted
            ('╚', "N W W N"), // adjusted
            ('╠', "N W W W"), // adjusted
            (' ', "W O O O"), // adjusted
            ('╕', "W O O N"),
            ('╗', "W O O W"),
            (' ', "W O N O"), // adjusted
            ('┌', "W O N N"), // adjusted
            ('╗', "W O N W"), // adjusted
            ('═', "W O W O"),
            ('╤', "W O W N"),
            ('╦', "W O W W"),
            ('╛', "W N O O"),
            ('╡', "W N O N"),
            ('╗', "W N O W"),
            ('└', "W N N O"), // adjusted
            ('├', "W N N N"), // adjusted
            ('╗', "W N N W"), // adjusted
            ('╧', "W N W O"),
            ('╪', "W N W N"),
            ('╦', "W N W W"), // adjusted
            ('╝', "W W O O"),
            ('╝', "W W O N"), // adjusted
            ('╣', "W W O W"),
            ('╝', "W W N O"), // adjusted
            ('╝', "W W N N"), // adjusted
            ('╣', "W W N W"), // adjusted
            ('╩', "W W W O"),
            ('╩', "W W W N"), // adjusted
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

            ptt[0, 0].Text("a");
            ptt[0, 1].Text("b");
            ptt[0, 2].Text("c");
            ptt[0, 3].Text("d");

            ptt[1, 0].Text("A");
            ptt[1, 1].Text("B").RightBorder(top).BottomBorder(left);
            ptt[1, 2].Text("C").LeftBorder(top).BottomBorder(right);
            ptt[1, 3].Text("D");

            ptt[2, 0].Text("0");
            ptt[2, 1].Text("1").RightBorder(bottom).TopBorder(left);
            ptt[2, 2].Text("2").LeftBorder(bottom).TopBorder(right);
            ptt[2, 3].Text("3");

            ptt[3, 0].Text("!");
            ptt[3, 1].Text("@");
            ptt[3, 2].Text("#");
            ptt[3, 3].Text("$");

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
