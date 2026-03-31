namespace GGUIML.AST.Types {
    public struct LineSplitString : IArgumentType {
        public string RawString { get; set; }
        public string[] IndexableString {
            get => RawString.Split ('\n');
            set => RawString = string.Join ("\n", value);
        }

        public bool Parsable { get; set; }

        public LineSplitString (string raw, bool parsable = false) {
            RawString = raw;
            Parsable = parsable;
        }
    }
}