using System;
using System.Linq;
using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    internal class NumericType : RawType {
        public Type[] Restrictions { get; private set; }

        public override string TypeName => "numeric";

        public NumericType () {}
        public NumericType (Type[] restrictions) {
            Restrictions = restrictions;
        }

        public override bool CanParse (string data) {
            try {
                INumeric result = data.ParseNumeric ();
                if (Restrictions.Length == 0)
                    return true;
                
                return Restrictions.Contains (result.GetType ());
            }
            catch (FormatException) {
                return false;
            }
        }
    }
}