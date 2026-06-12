using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    internal class ScaleType : RawType {
        public override string TypeName => "scale";
        public override bool CanParse (string data) {
            return ParserExtensions.CanParse (() => ScaleStruct.Parse (data));
        }
    }
}