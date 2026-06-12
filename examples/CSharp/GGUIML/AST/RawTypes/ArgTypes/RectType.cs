using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    internal class RectType : RawType {
        public override string TypeName => "rect";
        public override bool CanParse (string data) {
            return ParserExtensions.CanParse (() => RectStruct.Parse (data));
        }
    }
}