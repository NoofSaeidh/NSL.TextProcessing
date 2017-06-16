using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSL.TextProcessing.Common;

namespace NSL.TextProcessing {
    public class Text : IEnumerable<string> {
        #region private
        private const int defaultCapacity = 1024;
        #endregion

        #region internal

        internal readonly List<WordPair> Items;

        #endregion

        #region public

        public string this[int index] => Items[index].ToString();

        public Text(int capacity = defaultCapacity) {
            Items = new List<WordPair>(capacity);
        }

        public Text(string value) : this() {
            AddString(value);
        }

        public void AddString(string value) {
            Items.AddRange(WordPair.Match(value).ToList());
        }

        public IEnumerable<Text> SplitOnLines(bool includeNewLineSymbol = false) {
            var res = new List<Text>();
            var text = new Text(defaultCapacity/4);
            foreach (var item in Items) {
                if (item.String.Contains(Environment.NewLine)){
                    if (includeNewLineSymbol)
                        text.Items.Add(item);
                    else
                        text.Items.Add(WordPair.Match(item.ToString().Replace(Environment.NewLine, ""))[0]);

                    res.Add(text);
                    text = new Text();
                }
                else text.Items.Add(item);
            }
            if (Items.Count != 0) {
                res.Add(text);
            }
            return res;
        }

        public IEnumerable<Text> Split(string symbols = "\n", bool includeSymbols = false) {
            var res = new List<Text>();
            var text = new Text();
            foreach (var item in Items) {
                if (item.String.Contains(symbols)) {
                    if (includeSymbols)
                        text.Items.Add(item);
                    else
                        text.Items.Add(WordPair.Match(item.ToString().Replace(symbols, ""))[0]);

                    res.Add(text);
                    text = new Text();
                }
                else text.Items.Add(item);
            }
            if (Items.Count != 0) {
                res.Add(text);
            }
            return res;
        }

        public static Text Join(IEnumerable<Text> value, string symbol = "\n") {
            var text = new Text();
            foreach (var item in value) {
                foreach (var wordpair in item.Items) {
                    text.Items.Add(wordpair);
                }
                var last = text.Items.Last();
                if (!last.Right.EndsWith(symbol))
                    last = new WordPair(last.Left, last.Right + symbol);
            }
            return text;
        }

        public static Text JoinByLines(IEnumerable<Text> value) {
            var text = new Text();
            foreach (var item in value) {
                foreach (var wordpair in item.Items) {
                    text.Items.Add(wordpair);
                }
                var last = text.Items.Last();
                if (!last.Right.EndsWith(Environment.NewLine))
                    last = new WordPair(last.Left, last.Right + Environment.NewLine);
            }
            return text;
        }

        public string GetFullText() => string.Join("", this);

        #endregion

        #region inherit

        public IEnumerator<string> GetEnumerator() {
            foreach (var item in Items) {
                yield return item.ToString();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear() => Items.Clear();

        public override string ToString() => $"Words Count: {Items.Count}";

        public override bool Equals(object obj) => Items.Equals(obj);

        public override int GetHashCode() => Items.GetHashCode();

        #endregion

        #region operators

        public static explicit operator Text(string value) => new Text(value);

        #endregion
    }
}
