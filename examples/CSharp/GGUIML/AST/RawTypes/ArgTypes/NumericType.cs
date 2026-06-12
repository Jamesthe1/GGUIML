using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    internal class NumericType : RawType {
        public override string TypeName => "numeric";
        // TODO: Optional numeric type restriction
        public override bool CanParse (string data) {
            return ParserExtensions.CanParse (data.ParseNumeric);
        }
    }
}