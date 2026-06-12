using System.Collections.Generic;
using System.Linq;

namespace GGUIML.AST.Types {
    internal class EnumType : RawType {
        public string[][] OptionGroups { get; private set; }

        private string[] OptionNames => OptionGroups.Select (o => string.Join ("|", o)).ToArray ();
        public override string TypeName => $"enum[{string.Join (" and/or ", OptionNames)}]";

        public EnumType (params string[][] optionGroups) {
            OptionGroups = optionGroups;
        }

        public EnumType (params string[] options) {
            OptionGroups = new string[][] { options };
        }

        // Auto-splits if OptionGroups are more than 1
        public override bool CanParse (string data) {
            string[] parts = { data };
            if (OptionGroups.Length > 1)
                parts = data.Split ('-');
            
            int partIdx = 0;
            foreach (string[] optGrp in OptionGroups) {
                if (optGrp.Contains (parts[partIdx]))
                    partIdx++;
                if (partIdx == parts.Length)
                    return true;
            }
            return false;
        }
    }
}