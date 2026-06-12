using System;
using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    internal class AssociativeArrayType<T> : RawType {
        public readonly Func<string, T> Parser;

        public AssociativeArrayType (Func<string, T> valueParser) {
            Parser = valueParser;
        }

        public override string TypeName => "assoc-array";
        public override bool CanParse (string data) {
            return ParserExtensions.CanParse (() => AssociativeArray<T>.Parse (data, Parser));
        }
    }
}