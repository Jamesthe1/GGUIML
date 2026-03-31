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
    }

    public struct NumericPercent : INumeric<float> {
        private float innerValue;
        public float Value { get => innerValue * 100f; set => innerValue = value / 100f; }
        public object BoxedValue {
            get => Value;
            set => Value = (float)value;
        }

        public static readonly NumericPercent Full = new NumericPercent { Value = 100f };
    }

    public struct NumericDecimal : INumeric<float> {
        public float Value { get; set; }
        public object BoxedValue {
            get => Value;
            set => Value = (float)value;
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

        public static readonly NumericFlag Dynamic = new NumericFlag { Value = (byte)Flag.DYNAMIC };
    }
}