using System.Collections.Generic;

namespace GGUIML.AST.Procedures {
    internal abstract class BaseParserProcedure {
        public readonly List<ParserWarning> Warnings = new List<ParserWarning> ();
        public abstract bool ProcedureValid (ref string lineState, ref ParserState state);
        public abstract void RunProcedure (ref string lineState, ref ParserState state);
        public virtual bool TerminateLine (ref string lineState, ref ParserState state) {
            return lineState == "";
        }
    }
}