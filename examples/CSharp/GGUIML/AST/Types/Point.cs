using System;
using System.Linq;
using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    public struct Point : IArgumentType {
        public INumeric X { get; set; }
        public INumeric Y { get; set; }
        public INumeric Z { get; set; }

        public static readonly Point Zero = new Point { X = NumericInt.Zero, Y = NumericInt.Zero, Z = NumericInt.Zero };
        public static readonly Point ZeroDynamic = new Point { X = NumericInt.Zero, Y = NumericInt.Zero, Z = NumericFlag.Dynamic };

        public static Point Parse (string data) {
            if (!data.Contains ('(') || !data.Contains (')') || !data.Contains (','))
                throw new FormatException ("Not valid point format");
            
            // Remove front and back to get rid of parentheses, then split by comma and remove whitespace
            string[] parts = data.Remove (data.Length - 1).Remove (0, 1).Split (',').Select (s => s.Trim ()).ToArray ();
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
    }
}