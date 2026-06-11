using System.Collections.Generic;

namespace GGUIML.AST.Procedures {
    internal abstract class BaseParserProcedure {
        public abstract string DebugProcedureName { get; }
        public readonly List<ParserWarning> Warnings = new List<ParserWarning> ();
        public abstract bool ProcedureValid (string lineState, ParserState state);
        public abstract void RunProcedure (ref string lineState, ref ParserState state);
        public virtual bool MustTerminateLine (string lineState, ParserState state) {
            return lineState == "";
        }
    }
}