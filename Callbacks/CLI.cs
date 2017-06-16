using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSL.TextProcessing.Common.Highlight;

namespace NSL.TextProcessing.Callbacks {
    internal class CLI {
        public static void Highligth(HighlightWord[] words) {

            foreach (var word in words) {

                if (word.Highlight == Highlight.High)
                    Console.ForegroundColor = ConsoleColor.Red;

                else if (word.Highlight == Highlight.Normal)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;

                else if (word.Highlight == Highlight.Light)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                else
                    Console.ResetColor();

                Console.Write(word.Value.Replace("\n", "").Replace("\r", "") + " ");

            }
        }
    }
}
