using System;
using System.Linq;

namespace GGUIML.AST.Types {
    internal class StringType : RawType {
        public bool Multiline { get; private set; }
        public bool Whitespaced { get; private set; }
        // TODO: Implement special character detection

        public override string TypeName => (Multiline ? "single" : "multi") + "-string" + (Whitespaced ? "" : "-nospace");

        public StringType (bool multiline = true, bool whitespaced = true) {
            Multiline = multiline;
            Whitespaced = whitespaced;
        }

        public override bool CanParse (string data) {
            try {
                LineSplitString lss = LineSplitString.Parse (data);
                if (!Multiline)
                    return lss.IndexableString.Length > 1;
                if (!Whitespaced)
                    return lss.RawString.Any (c => char.IsWhiteSpace (c));
                return true;
            } catch (FormatException) {
                return false;
            }
        }
    }
}