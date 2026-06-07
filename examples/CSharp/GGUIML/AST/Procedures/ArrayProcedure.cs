using GGUIML.Exceptions;

namespace GGUIML.AST.Procedures {
    internal class ArrayProcedure : BaseParserProcedure {
        public override bool ProcedureValid (ref string lineState, ref ParserState state) {
            if (state.InterpreterMode == ParserState.InterpMode.Array) {
                return state.indent > state.argIndent;
            }
            return false;
        }
        
        public override void RunProcedure (ref string lineState, ref ParserState state) {
            if (state.InterpreterMode == ParserState.InterpMode.Array && !lineState.StartsWith ("-")) {
                throw new GUILParseException ("Invalid appearance of: " + lineState, state.lineNumber);
            }
            state.currentArgument.Data += lineState + "\n";
            lineState = "";
        }
    }
}