namespace GGUIML.Exceptions {
    public class GUILParseException : Exception {
        public int LineNumber { get; private set; }

        public GUILParseException (string msg, int lineNum) : base (msg) {
            LineNumber = lineNum;
        }

        public override string Message => base.Message + $" (line {LineNumber})";
    }
}