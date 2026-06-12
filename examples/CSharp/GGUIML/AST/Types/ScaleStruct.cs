using System;
using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    public struct ScaleStruct : IArgumentType {
        public INumeric X { get; set; }
        public INumeric Y { get; set; }

        public static readonly ScaleStruct Dynamic = new ScaleStruct { X = NumericFlag.Dynamic, Y = NumericFlag.Dynamic };

        public static ScaleStruct Parse (string data) {
            string[] parts = data.Split ('x');
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
    }
}