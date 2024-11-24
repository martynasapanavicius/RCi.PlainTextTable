using System;
using RCi.PlainTextTable;

namespace Sandbox
{
    internal class Program
    {
        static void Main()
        {
            //var nl = Environment.NewLine;
            var ptt = new PlainTextTable
            {
                BorderStyle = BorderStyle.UnicodeSingle,
                DefaultBorders = new(Border.Normal),
            };

            ptt[0, 0].Text("a");
            ptt[0, 1].Text("b").Borders(Border.None);
            ptt[0, 2].Text("c");
            ptt[0, 3].Text("d");
            ptt[0, 4].Text("3");

            ptt[1, 0].Text("A");
            ptt[1, 1].Text("B").Borders(Border.None);
            ptt[1, 2].Text("C");
            ptt[1, 3].Text("D").Borders(Border.None);
            ptt[1, 4].Text("E");

            ptt[2, 0].Text("0");
            ptt[2, 1].Text("1").Borders(Border.None);
            ptt[2, 2].Text("2").Borders(Border.None);
            ptt[2, 3].Text("3").Borders(Border.None);
            ptt[2, 4].Text("4");

            ptt[3, 0].Text("!");
            ptt[3, 1].Text("@");
            ptt[3, 2].Text("#");
            ptt[3, 3].Text("$");
            ptt[3, 4].Text("%");


            var str = ptt.ToString();
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
        }
    }
}
