using System;
using System.Linq;

namespace GGUIML.AST.Types {
    public interface INumeric : IArgumentType {
        object BoxedValue { get; set; }
    }

    public interface INumeric<T> : INumeric where T : IEquatable<T>, IComparable<T> {
        T Value { get; set; }
    }

    public struct NumericInt : INumeric<int> {
        public int Value { get; set; }
        public object BoxedValue {
            get => Value;
            set => Value = (int)value;
        }

        public static readonly NumericInt Zero = new NumericInt { Value = 0 };

        public static NumericInt Parse (string data) {
            return new NumericInt {
                Value = int.Parse (data)
            };
        }
    }

    public struct NumericPercent : INumeric<float> {
        private float innerValue;
        public float Value { get => innerValue * 100f; set => innerValue = value / 100f; }
        public object BoxedValue {
            get => Value;
            set => Value = (float)value;
        }

        public static readonly NumericPercent Full = new NumericPercent { Value = 100f };

        public static NumericPercent Parse (string data) {
            if (!data.EndsWith ("%"))
                throw new FormatException ("Data does not match percentage format");
            return new NumericPercent {
                Value = float.Parse (data.Remove (data.Length - 1))
            };
        }
    }

    public struct NumericDecimal : INumeric<float> {
        public float Value { get; set; }
        public object BoxedValue {
            get => Value;
            set => Value = (float)value;
        }

        public static NumericDecimal Parse (string data) {
            if (!data.Contains ("."))
                throw new FormatException ("Decimals must contain a period");
            return new NumericDecimal {
                Value = float.Parse (data)
            };
        }
    }

    public struct NumericFlag : INumeric<byte> {
        public enum Flag {
            DYNAMIC = 1 << 0,
            SQUARE = 1 << 1
        }

        public byte Value { get; set; }
        public object BoxedValue {
            get => Value;
            set => Value = (byte)value;
        }

        public int AssociatedValue { get; set; }

        public static readonly NumericFlag Dynamic = new NumericFlag { Value = (byte)Flag.DYNAMIC };

        public static NumericFlag Parse (string data) {
            if (data == "DYNAMIC") {
                return Dynamic;
            }
            if (data.StartsWith ("SQUARE")) {   // A scale will pass this on
                int mult = 1;
                if (data.Contains ('*'))
                    mult = int.Parse (data.Split ('*')[1]);
                return new NumericFlag {
                    Value = (byte)Flag.SQUARE,
                    AssociatedValue = mult
                };
            }

            throw new FormatException ("Invalid flag data");
        }
    }
}