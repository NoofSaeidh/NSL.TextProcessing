using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NSL.TextProcessing.Common {
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class WordPair {
        public string Left { get; internal set; }
        public string Right { get; internal set; }
        public string String => Left + Right;
        public WordPair(string left, string right) {
            Left = left;
            Right = right;
        }
        private static readonly string wordSplitPattern = "([a-zA-Zа-яА-ЯёЁ]*)([^a-zA-Zа-яА-ЯёЁ]*)";
        internal static WordPair[] Match(string line) {
            var res = Regex.Matches(line, wordSplitPattern);
            var ret = new WordPair[res.Count - 1]; //TODO: exclude last element in regex
            for (int i = 0 ; i < res.Count - 1 ; i++) { //TODO: exclude last element in regex
                ret[i] = new WordPair(res[i].Groups[1].Value, res[i].Groups[2].Value);

            }
            return ret;
        }

        internal WordPair(string value) {
            var res = Match(value).First();
            Left = res.Left;
            Right = res.Right;
        }

        public static explicit operator WordPair(string value) => Match(value).First();

        public override bool Equals(object obj) => String.Equals(obj);
        public override int GetHashCode() => String.GetHashCode();
        public override string ToString() => String;

        private string DebuggerDisplay => "\"" + Left + "\"   \"" + Right + "\"";
    }
}
