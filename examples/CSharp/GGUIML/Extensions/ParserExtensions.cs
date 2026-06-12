using System;
using System.Linq;

using GGUIML.AST.Types;

namespace GGUIML.Extensions {
    internal static class ParserExtensions {
        public static bool CanParse<T> (this Func<T> parser) {
            try {
                parser.Invoke ();
                return true;
            }
            catch (FormatException) {
                return false;
            }
        }

        public static INumeric ParseNumeric (this string str) {
            if (str.EndsWith ("%")) {
                return NumericPercent.Parse (str);
            }
            if (str.Contains (".")) {
                return NumericDecimal.Parse (str);
            }
            if (str == "DYNAMIC" || str.StartsWith ("SQUARE")) {
                return NumericFlag.Parse (str);
            }

            return NumericInt.Parse (str);
        }
    }
}