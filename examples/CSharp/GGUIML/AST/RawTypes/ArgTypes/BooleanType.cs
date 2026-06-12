using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    internal class BooleanType : RawType {
        public override string TypeName => "boolean";
        public override bool CanParse (string data) {
            return ParserExtensions.CanParse (() => BooleanStruct.Parse (data));
        }
    }
}