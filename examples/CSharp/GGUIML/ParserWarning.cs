
namespace GGUIML {
    public struct ParserWarning {
        public string Message { get; set; }
        public int LineNumber { get; set; }

        public ParserWarning (string message, int lineNum) {
            Message = message;
            LineNumber = lineNum;
        }

        public override string ToString () {
            return Message + $" (line {LineNumber})";
        }
    }
}