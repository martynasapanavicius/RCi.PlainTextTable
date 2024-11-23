using System;
using RCi.PlainTextTable;

namespace Sandbox
{
    internal class Program
    {
        static void Main()
        {
            //var nl = Environment.NewLine;
            var ptt = new PlainTextTable();

            ptt[0, 0].Text("a").RowSpan(2);
            ptt[0, 1].Text("b");
            ptt[0, 2].Text("c");

            var str = ptt.ToString();
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
        }
    }
}
