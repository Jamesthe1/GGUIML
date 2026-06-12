using System;

namespace GGUIML.AST.Types {
    public struct LineSplitString : IArgumentType {
        public string RawString { get; set; }
        // TODO: Handle escaped characters when IndexableString's `get` is called, if parsable
        public string[] IndexableString {
            get => RawString.Split ('\n');
            set => RawString = string.Join ("\n", value);
        }

        public bool Parsable { get; set; }

        public LineSplitString (string raw, bool parsable = false) {
            RawString = raw;
            Parsable = parsable;
        }

        public static LineSplitString Parse (string data) {
            LineSplitString splitString = new LineSplitString ();
            if (data.StartsWith ("'") && data.EndsWith ("'"))
                splitString.Parsable = false;
            else if (data.StartsWith ("\"") && data.EndsWith ("\""))
                splitString.Parsable = true;
            else
                throw new FormatException ("String not encapsulated in quotes");
            
            splitString.RawString = data.Substring (1, data.Length - 2);
            return splitString;
        }
    }
}