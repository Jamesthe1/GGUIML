using System;
using System.Linq;
using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    internal class NumericType : RawType {
        public Type[] Restrictions { get; private set; }

        public override string TypeName => "numeric";

        public NumericType () {}
        public NumericType (params Type[] restrictions) {
            Restrictions = restrictions;
        }

        public override bool CanParse (string data) {
            try {
                INumeric result = data.ParseNumeric ();
                return MatchesRestrictions (result);
            }
            catch (FormatException) {
                return false;
            }
        }

        public bool MatchesRestrictions (INumeric numeric) {
            if (Restrictions.Length == 0)
                    return true;
                
            return Restrictions.Contains (numeric.GetType ());
        }
    }
}