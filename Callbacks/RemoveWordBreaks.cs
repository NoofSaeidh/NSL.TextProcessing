using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSL.TextProcessing.Common;
using NSL.TextProcessing.Common.Highlight;

namespace NSL.TextProcessing.Callbacks {

    public static class RemoveWordBreaks {

        public static class CLI {
            public static WordBreakAction Interactive(HighlightWord[] words, bool greedy) {

                Console.Clear();
                if (greedy) Console.WriteLine("Greedy\n\n");
                else Console.WriteLine("Not greedy\n\n");

                Callbacks.CLI.Highligth(words);

                Console.ResetColor();
                Console.WriteLine("\n\nJoin Breaks?\n(y,n)");
                while (true)
                    switch (Console.ReadKey().Key) {
                        case ConsoleKey.Y:
                            return WordBreakAction.Join;
                        case ConsoleKey.N:
                            return WordBreakAction.None;
                    }
            }

            public static WordBreakAction InteractiveOnlyNotGreedy(HighlightWord[] words, bool greedy) {
                if (greedy) return WordBreakAction.None;
                Console.Clear();
                if (greedy) Console.WriteLine("Greedy\n\n");
                else Console.WriteLine("Not greedy\n\n");

                Callbacks.CLI.Highligth(words);

                Console.ResetColor();
                Console.WriteLine("\n\nJoin Breaks?\n(y,n)");
                while (true)
                    switch (Console.ReadKey().Key) {
                        case ConsoleKey.Y:
                            return WordBreakAction.Join;
                        case ConsoleKey.N:
                            return WordBreakAction.None;
                    }
            }
        }

        public static WordBreakAction JoinAllGreedy(HighlightWord[] words, bool greedy) {
            return WordBreakAction.Join;
        }

        public static WordBreakAction JoinAllOnlyNotGreedy(HighlightWord[] words, bool greedy) {
            if (!greedy) return WordBreakAction.Join;
            return WordBreakAction.None;
        }
    }
}
