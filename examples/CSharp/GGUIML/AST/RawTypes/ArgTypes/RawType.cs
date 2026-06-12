namespace GGUIML.AST.Types {
    internal abstract class RawType {
        public abstract string TypeName { get; }
        public abstract bool CanParse (string data);
    }
}