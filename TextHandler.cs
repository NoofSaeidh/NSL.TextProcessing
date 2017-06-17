using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NHunspell;
using NSL.DotNet.Helpers;
using NSL.TextProcessing.Common;
using NSL.TextProcessing.Common.Highlight;
using HighIndex = System.Collections.Generic.KeyValuePair<int, NSL.TextProcessing.Common.Highlight.Highlight>;

namespace NSL.TextProcessing {
    public class TextHandler : IDisposable {
        #region Common
        public readonly char[] PossibleHyphens = new char[]{DefaultHyphyn, '‑' };
        public const char DefaultHyphyn = '-';
        public List<char> Hyphens { get; set; }
        public Hunspell Hunspell { get; set; }
        public Text Text { get; }

        public TextHandler(Text text) {
            ExceptionHelper.CheckNull(text, nameof(text));
            Text = text;
        }

        public TextHandler(Text text, Hunspell hunspell = null, bool allPossibleHyphens = true) : this(text) {
            Hunspell = hunspell;

            if (allPossibleHyphens) Hyphens = PossibleHyphens.ToList();
            else Hyphens = new List<char> { DefaultHyphyn };
        }

        public void Dispose() {
            Hunspell?.Dispose();
        }

        public override string ToString() => Text.ToString();

        private string[] GetSentence(int index, out int newindex) {
            var begin = 0;
            var end = Text.Items.Count;
            newindex = index;

            for (var i = index - 1 ; i >= 0 ; i--) {
                var breakFlag = false;
                foreach (var schar in sentenceChars) {
                    if (Text.Items[i].Right.Contains(schar)) {
                        begin = i + 1;
                        breakFlag = true;
                        break;
                    }
                }
                if (breakFlag) break;

            }
            for (var i = index + 1 ; i < Text.Items.Count ; i++) {
                var breakFlag = false;
                foreach (var schar in sentenceChars) {
                    if (Text.Items[i].Right.Contains(schar)) {
                        end = i - 1;
                        breakFlag = true;
                        break;
                    }
                }
                if (breakFlag) break;
            }

            //if (begin == end) return null;
            newindex = index - begin;
            return Text.Items.GetRange(begin, end - begin).Select(x => x.String).ToArray();
        }
        private HighlightWord[] GetHighlighSentence(int index, int minMax, params HighIndex[] highlighs) {
            var begin = 0;
            var end = Text.Items.Count - 1;

            //search for begging of sentence
            for (var i = index - 1 ; i >= 0 ; i--) {
                var breakFlag = false;
                foreach (var schar in sentenceChars) {
                    if (Text.Items[i].Right.Contains(schar)) {
                        begin = i + 1;
                        breakFlag = true;
                        break;
                    }
                }
                if (breakFlag) break;

            }

            //search for ending of sentence
            for (var i = index + 1 ; i < Text.Items.Count ; i++) {
                if (i < minMax) continue;
                var breakFlag = false;
                foreach (var schar in sentenceChars) {
                    if (Text.Items[i].Right.Contains(schar)) {
                        end = i - 1;
                        breakFlag = true;
                        break;
                    }
                }
                if (breakFlag) break;
            }

            var sentece = new HighlightWord[end-begin+1];
            //make highlighted sentence
            for (int i = begin, j = 0 ; i <= end ; i++, j++) {
                var high = highlighs.Where(x=>x.Key==i).FirstOrDefault().Value;
                sentece[j] = new HighlightWord(Text.Items[i].String, high);
            }

            return sentece;
        }
        private readonly char[] sentenceChars = new char[]{'.','\n','\r','!','?'};
        #endregion

        #region Processing
        public void RemoveHyphenation(RemoveHyphenCallback callback) {
            ExceptionHelper.CheckNull(callback, nameof(callback));
            ExceptionHelper.CheckNull(Hunspell, nameof(Hunspell));
            ExceptionHelper.CheckNullOrEmpty(Hyphens, nameof(Hyphens));

            for (var i = 0 ; i < Text.Items.Count - 1 ; i++) {
                var item = Text.Items[i];
                var next = Text.Items[i + 1];

                foreach (var hyphen in Hyphens) {
                    if (item.Right.Contains(hyphen)) {

                        var words = GetHighlighSentence(i, i+2
                            , new HighIndex(i, Highlight.Normal)
                            , new HighIndex(i+1, Highlight.Normal));


                        HyphenAction answer;
                        if (Hunspell.Spell(item.Left + next.Left))
                            answer = callback(words, true);
                        else
                            answer = callback(words, false);
                        if (answer == HyphenAction.Remove) {
                            item.Left += next.Left;
                            item.Right = next.Right;
                            Text.Items.RemoveAt(i + 1);
                        }
                        break;
                    }
                }
            }
        }

        public void RemoveWordBreaks(RemoveWordBreakCallback callback
            , bool greedy = false, int maxDepth = 1
            , FillingWordBreakSymbols fillingSymbols = FillingWordBreakSymbols.Spaces) {

            ExceptionHelper.CheckNull(callback, nameof(callback));

            var regex = new Regex(fillingSymbols.GetPattern());

            for (var i = 0 ; i < Text.Items.Count - 1 ; i++) {
                var item = Text.Items[i];

                var isGreedy = false;
                if (Hunspell.Spell(item.Left)) isGreedy = true;
                if (isGreedy && !greedy) continue;
                if (!regex.IsMatch(item.Right)) continue;

                var newWord = item.Left;

                var indexes = new List<HighIndex> {
                    new HighIndex(i, Highlight.Normal)
                };
                for (var k = i + 1 ; k < Text.Items.Count && k - i <= maxDepth ; k++) {
                    var newItem = Text.Items[k];
                    newWord += newItem.Left;

                    indexes.Add(new HighIndex(k, Highlight.Normal));

                    if (Hunspell.Spell(newWord)) {
                        var res = callback(GetHighlighSentence(i, k + 1, indexes.ToArray()), isGreedy);

                        if (res == WordBreakAction.Join) {
                            item.Left = newWord;
                            item.Right = newItem.Right;

                            var maxIter = k - i;
                            for (var iter = 0 ; iter < maxIter ; iter++) {
                                Text.Items.RemoveAt(i + 1);
                                k = Text.Items.Count;
                            }
                        }
                    }
                }
            }
        }

        public void RemoveLines(string pattern, bool ignoreCase = false) {
            Regex regex;
            if (ignoreCase) regex = new Regex("(" + pattern + ")", RegexOptions.IgnoreCase);
            else regex = new Regex("(" + pattern + ")");

            var text = "";
            var start = 0;
            for (var i = 0 ; i < Text.Items.Count ; i++) {
                var item = Text.Items[i];
                text += item.String;

                if (item.Right.Contains(Environment.NewLine) || i == Text.Items.Count - 1) {
                    if (regex.IsMatch(text)) {
                        for (; i >= start ; i--) {
                            Text.Items.RemoveAt(i);
                        }
                    }
                    else start = i + 1;

                    text = "";
                }
            }
        }

        public void Remove(string pattern, bool ignoreCase = false, bool checkSpaces = true) {
            Regex regex;
            if (checkSpaces) pattern = $"( (?:{pattern})|(?:{pattern}) |(?:{pattern}))";
            else pattern = $"({pattern})";

            if (ignoreCase) regex = new Regex(pattern, RegexOptions.IgnoreCase);
            else regex = new Regex(pattern);

            var allText = Text.GetFullText();

            allText = regex.Replace(allText, "");

            Text.Items.Clear();
            Text.AddString(allText);
        }

        public void NormalizeLineEnding() {
            var regex = new Regex("\r|\n|\r\n",RegexOptions.Multiline);
            foreach (var item in Text.Items) {
                if (regex.IsMatch(item.Right)) {
                    item.Right = regex.Replace(item.Right, Environment.NewLine);
                }
            }
        }
        #endregion
    }
}
