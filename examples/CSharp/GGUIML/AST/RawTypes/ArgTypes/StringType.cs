using System;
using System.Linq;

namespace GGUIML.AST.Types {
    internal class StringType : RawType {
        public static char[] SpecialChars => new char[] {
            '$', '@', '[', ']', '{', '}', '\'', '\"', '\\', '.', '!', '?', ':', ';', ',', '*'
        };

        public bool Multiline { get; private set; }
        public bool Whitespaced { get; private set; }
        public bool AllowSpecialChars { get; private set; }

        public override string TypeName {
            get {
                string name = (Multiline ? "single" : "multi") + "-string";
                if (!Whitespaced)
                    name += "-nospace";
                if (!AllowSpecialChars)
                    name += "-nospecial";
                return name;
            }
        }

        public StringType (bool multiline = true, bool whitespaced = true, bool allowSpecial = true) {
            Multiline = multiline;
            Whitespaced = whitespaced;
            AllowSpecialChars = allowSpecial;
        }

        public override bool CanParse (string data) {
            try {
                LineSplitString lss = LineSplitString.Parse (data);
                if (!Multiline)
                    return lss.IndexableString.Length > 1;
                if (!Whitespaced)
                    return lss.RawString.Any (c => char.IsWhiteSpace (c));
                return MatchesSpecialRequirement (lss);
            } catch (FormatException) {
                return false;
            }
        }

        public bool MatchesSpecialRequirement (LineSplitString lss) {
            if (AllowSpecialChars)
                return true;
            
            string prefix = lss.Parsable ? "\\" : "";
            foreach (char special in SpecialChars) {
                if (lss.RawString.Contains (prefix + special))
                    return false;
            }
            return true;
        }
    }
}