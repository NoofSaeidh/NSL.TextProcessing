using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSL.TextProcessing.Common.Highlight;

namespace NSL.TextProcessing.Common {
    public delegate HyphenAction RemoveHyphenCallback(HighlightWord[] words, bool spelled);

    //public delegate SpellCheckAction SpellCheckCallback(HighlightHandler handler, bool greedy);
    public delegate WordBreakAction RemoveWordBreakCallback(HighlightWord[] words, bool greedy);
}
