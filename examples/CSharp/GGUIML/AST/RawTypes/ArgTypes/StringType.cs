using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    internal class StringType : RawType {
        public override string TypeName => "string";
        public override bool CanParse (string data) {
            return ParserExtensions.CanParse (() => LineSplitString.Parse (data));
        }
    }
}