using System;
using System.Linq;

namespace GGUIML.AST.Types {
    internal class ScaleType : RawType {
        public NumericType AxesType { get; private set; }

        public override string TypeName => "scale";

        public ScaleType () {}
        public ScaleType (params Type[] restrictions) {
            Type[] requiredTypes = new Type[] { typeof (NumericFlag) };
            AxesType = new NumericType (restrictions.Concat (requiredTypes).Distinct ().ToArray ());
        }

        public override bool CanParse (string data) {
            try {
                ScaleStruct result = ScaleStruct.Parse (data);
                return MatchesRestrictions (result);
            }
            catch (FormatException) {
                return false;
            }
        }

        public bool MatchesRestrictions (ScaleStruct scale) {
            return AxesType.MatchesRestrictions (scale.X) && AxesType.MatchesRestrictions (scale.Y);
        }
    }
}