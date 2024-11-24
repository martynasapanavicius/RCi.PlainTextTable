using System;
using RCi.PlainTextTable;

namespace Sandbox
{
    internal class Program
    {
        static void Main()
        {
            static Border ParseBorder(char code) => code switch
            {
                'O' => Border.None,
                'N' => Border.Normal,
                'B' => Border.Bold,
                _ => throw new NotSupportedException(nameof(code)),
            };

            var neighbours = "BOBO";

            var left = ParseBorder(neighbours[0]);
            var top = ParseBorder(neighbours[1]);
            var right = ParseBorder(neighbours[2]);
            var bottom = ParseBorder(neighbours[3]);

            var ptt = new PlainTextTable
            {
                BorderStyle = BorderStyle.UnicodeDouble,
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

            //ptt[0, 0].Text("a").RightBorder(top).BottomBorder(left);
            //ptt[0, 1].Text("b").LeftBorder(top).BottomBorder(right);
            //ptt[1, 0].Text("c").RightBorder(bottom).TopBorder(left);
            //ptt[1, 1].Text("d").LeftBorder(bottom).TopBorder(right);

            var str = ptt.ToString();
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
        }
    }
}
