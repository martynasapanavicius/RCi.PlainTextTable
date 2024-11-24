using System;
using RCi.PlainTextTable;

namespace Sandbox
{
    internal class Program
    {
        static void Main()
        {
            var ptt = new PlainTextTable
            {
                BorderStyle = BorderStyle.UnicodeDouble,
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

            var str = ptt.ToString();
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
        }
    }
}
