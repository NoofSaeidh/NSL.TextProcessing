using System;
using System.Linq;
using NSL.TextProcessing.Common;
using NSL.TextProcessing.Common.Highlight;

namespace NSL.TextProcessing.Callbacks
{
    public static class RemoveHyphenation {

        public static class CLI {
            public static HyphenAction Interactive(HighlightWord[] words, bool spelled) {

                Console.Clear();
                if (spelled) Console.WriteLine("Spelled\n\n");
                else Console.WriteLine("Not spelled\n\n");

                Callbacks.CLI.Highligth(words);

                Console.ResetColor();
                Console.WriteLine("\n\nRemove hyphen?\n(y,n)");
                while (true)
                    switch (Console.ReadKey().Key) {
                        case ConsoleKey.Y:
                            return HyphenAction.Remove;
                        case ConsoleKey.N:
                            return HyphenAction.None;
                    }

            }
        }

        public static HyphenAction RemoveAll(HighlightWord[] words, bool spelled) {
            return HyphenAction.Remove;
        }

        public static HyphenAction RemoveAllSpelled(HighlightWord[] words, bool spelled) {
            if(spelled) return HyphenAction.Remove;
            return HyphenAction.None;
        }
    }
}
