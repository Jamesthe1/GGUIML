using GGUIML.AST.Types;

namespace GGUIML.Extensions {
    internal static class ParserExtensions {
        public static bool CanParse (this Action parser) {
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
                return new NumericPercent {
                    Value = float.Parse (str.Remove (str.Length - 1))
                };
            }
            if (str.Contains (".")) {
                return new NumericDecimal {
                    Value = float.Parse (str)
                };
            }
            if (str == "DYNAMIC") {
                return NumericFlag.Dynamic;
            }
            if (str.StartsWith ("SQUARE")) {
                int mult = 1;
                if (str.Contains ('*'))
                    mult = int.Parse (str.Split ('*')[1]);
                return new NumericFlag {
                    Value = (byte)NumericFlag.Flag.SQUARE,
                    AssociatedValue = mult
                };
            }

            return new NumericInt {
                Value = int.Parse (str)
            };
        }

        public static Point ParsePoint (this string str) {
            if (!str.Contains ('(') || !str.Contains (')') || !str.Contains (','))
                throw new FormatException ("Not valid point format");
            
            // Remove front and back to get rid of parentheses, then split by comma and remove whitespace
            string[] parts = (string[])str.Remove (str.Length - 1).Remove (0, 1).Split (',').Select (s => s.Trim ());
            if (parts.Length > 3)
                throw new FormatException ("Too many arguments for point");
            
            Point point = new Point {
                X = parts[0].ParseNumeric (),
                Y = parts[1].ParseNumeric ()
            };
            if (parts.Length == 3)
                point.Z = parts[2].ParseNumeric ();
            
            return point;
        }

        public static ScaleStruct ParseScale (this string str) {
            string[] parts = str.Split ('x');
            if (parts.Length != 2)
                throw new FormatException ("Invalid number of axes");
            
            ScaleStruct scale = new ScaleStruct {
                X = parts[0].ParseNumeric (),
                Y = parts[1].ParseNumeric ()
            };
            if (scale.X is NumericFlag && scale.Y is NumericFlag) {
                byte sqX = (byte)((byte)scale.X.BoxedValue & (byte)NumericFlag.Flag.SQUARE);
                byte sqY = (byte)((byte)scale.Y.BoxedValue & (byte)NumericFlag.Flag.SQUARE);
                if (sqX != 0 && sqY != 0)
                    throw new FormatException ("Both axes cannot be square");
            }

            return scale;
        }

        public static LineSplitString ParseString (this string str) {
            LineSplitString splitString = new LineSplitString ();
            if (str.StartsWith ("'") && str.EndsWith ("'"))
                splitString.Parsable = false;
            else if (str.StartsWith ("\"") && str.EndsWith ("\""))
                splitString.Parsable = true;
            else
                throw new FormatException ("String not encapsulated in quotes");
            
            splitString.RawString = str.Remove (str.Length - 1).Remove (0, 1);
            return splitString;
        }
    }
}