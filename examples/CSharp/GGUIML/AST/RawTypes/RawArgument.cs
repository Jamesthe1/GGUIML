namespace GGUIML.AST {
    internal interface IRawArgument {
        string Name { get; set; }
        string Data { get; set; }
        int Position { get; set; }
        int LineNumber { get; set; }
    }

    internal struct RawValue : IRawArgument {
        public string Name { get; set; }
        public string Data { get; set; }
        public int Position { get; set; }
        public int LineNumber { get; set; }
    }

    internal struct RawReference : IRawArgument {
        public string Name { get; set; }
        public string Data { get; set; }
        public int Position { get; set; }
        public int LineNumber { get; set; }
    }
}