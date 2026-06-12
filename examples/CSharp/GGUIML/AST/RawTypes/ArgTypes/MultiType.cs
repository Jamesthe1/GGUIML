using System.Collections.Generic;
using System.Linq;

namespace GGUIML.AST.Types {
    internal class MultiType : RawType {
        public RawType RequiredType { get; private set; }
        public Dictionary<char, RawType> OptionalTypes { get; private set; } = new Dictionary<char, RawType> ();

        private string OptionalTypeNames => string.Join ("|", OptionalTypes.Values.Select (t => t.TypeName).ToArray ());
        public override string TypeName => $"multi:{RequiredType.TypeName}({OptionalTypeNames})";

        public MultiType (RawType required, Dictionary<char, RawType> optional) {
            RequiredType = required;
            OptionalTypes = optional;
        }

        public override bool CanParse (string data) {
            RawType parser = RequiredType;
            string subdata = "";
            Queue<char> optKeys = new Queue<char> (OptionalTypes.Keys);
            for (int i = 0; i < data.Length; i++) {
                char c = data[i];
                if (c == optKeys.Peek () || i == data.Length - 1) {
                    if (!parser.CanParse (subdata))
                        return false;
                    subdata = "";
                    parser = OptionalTypes[c];
                    optKeys.Dequeue ();
                }
                subdata += c;
            }
            return true;
        }
    }
}