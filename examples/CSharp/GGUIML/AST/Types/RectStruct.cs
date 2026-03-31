namespace GGUIML.AST.Types {
    public struct RectStruct : IArgumentType {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Down { get; set; }

        public static readonly RectStruct Zero = new RectStruct { Left = 0, Top = 0, Right = 0, Down = 0 };
    }
}