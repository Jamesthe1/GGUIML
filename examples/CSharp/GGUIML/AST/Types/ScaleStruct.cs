namespace GGUIML.AST.Types {
    public struct ScaleStruct : IArgumentType {
        public INumeric X { get; set; }
        public INumeric Y { get; set; }

        public static readonly ScaleStruct Dynamic = new ScaleStruct { X = NumericFlag.Dynamic, Y = NumericFlag.Dynamic };
    }
}