using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSL.TextProcessing.Common.Highlight {
    public struct HighlightWord {
        public readonly string Value;
        public readonly Highlight Highlight;
        public override string ToString() => Value;

        public HighlightWord(string value, Highlight highligh = Highlight.None) {
            Value = value;
            Highlight = highligh;
        }
    }


    public enum Highlight {
        None = 0,
        Light = 1,
        Normal = 2,
        High = 3
    }
}
