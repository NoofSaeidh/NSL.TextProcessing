using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSL.DotNet.Attributes;
using NSL.DotNet.Enums;
using System.Reflection;
using NSL.DotNet.Extensions;

namespace NSL.TextProcessing.Common {

    public enum HyphenAction : byte {
        None = 0,
        Remove = 1
    }

    public enum WordBreakAction : byte {
        None = 0,
        Join = 1
    }

    [Flags]
    public enum FillingWordBreakSymbols : byte {
        [StringValue("\\s")] Spaces = 1,
        [StringValue("\\d")] Digits = 2,
        [StringValue("\\W")] NonWords = 4,
        All = Spaces | Digits | NonWords
    }

    internal static class FillingWordBreakSymbolsExtensions {
        internal static string GetPattern(this FillingWordBreakSymbols value) {
            var flags = value.GetFlags();
            var pattern = "";
            foreach (var flag in flags) {
                pattern += flag.GetStringValue();
            }

            return RegexPattern.ToFormat(pattern);
        }

        internal const string RegexPattern = "^[{0}]*$";
    }

    //[Flags]
    //public enum Combines : byte {
    //    [IntValue(0)] None = 0,
    //    [IntValue(1)] One = 1,
    //    [IntValue(2)] Two = 2,
    //    [IntValue(3)] Three = 4,
    //    [IntValue(4)] Four = 8,
    //    [IntValue(5)] Five = 16,
    //    [IntValue(6)] Six = 32,
    //    [IntValue(7)] Seven = 64,
    //    [IntValue(8)] Eight = 128,
    //    All = One | Two | Three | Four | Five | Six | Seven | Eight,
    //}



    //public static class CombinesExtensions {
    //    public static int[] GetInts(this Combines combines) {
    //        return combines.GetFlags().Select(x=>x.GetIntValue()).ToArray();
    //    }

    //    public static int MaxCombineLevel => 8;
    //}
}
