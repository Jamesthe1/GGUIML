namespace GGUIML.AST.Types {
    public struct Point : IArgumentType {
        public INumeric X { get; set; }
        public INumeric Y { get; set; }
        public INumeric Z { get; set; }

        public static readonly Point Zero = new Point { X = NumericInt.Zero, Y = NumericInt.Zero, Z = NumericInt.Zero };
        public static readonly Point ZeroDynamic = new Point { X = NumericInt.Zero, Y = NumericInt.Zero, Z = NumericFlag.Dynamic };
    }
}