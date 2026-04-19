namespace GGUIML.AST {
    internal abstract class RawNode {
        public int lineNumber;
        public int indentation;
        public List<IRawArgument> baseArgs = new List<IRawArgument> ();
        public List<RawNode> children = new List<RawNode> ();
    }

    internal class RawElement : RawNode {
        public string hintText;
        public List<IRawArgument> typeArgs = new List<IRawArgument> ();
    }

    internal class RawModule : RawNode {
        public bool template;
    }
}