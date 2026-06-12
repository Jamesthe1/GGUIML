using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    internal class PointType : RawType {
        public override string TypeName => "point";
        public override bool CanParse (string data) {
            return ParserExtensions.CanParse (() => Point.Parse (data));
        }
    }
}