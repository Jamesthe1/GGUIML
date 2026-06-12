using System;

namespace GGUIML.AST.Types {
    public struct BooleanStruct : IArgumentType {
        public bool Value { get; set; }

        public static BooleanStruct Parse (string data) {
            data = data.Trim ();
            if (data != "yes" && data != "no")
                throw new FormatException ("Invalid boolean keywords");
            
            return new BooleanStruct {
                Value = data == "yes"
            };
        }
    }
}