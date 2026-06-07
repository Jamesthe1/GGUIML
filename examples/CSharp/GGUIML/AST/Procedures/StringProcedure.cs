using GGUIML.Exceptions;

namespace GGUIML.AST.Procedures {
    internal class StringProcedure : BaseParserProcedure {
        public override bool ProcedureValid (ref string lineState, ref ParserState state) {
            if (state.InterpreterMode == ParserState.InterpMode.String || state.InterpreterMode == ParserState.InterpMode.PureString) {
                return true;
            }
            return false;
        }

        public override void RunProcedure (ref string lineState, ref ParserState state) {
            if (state.indent <= state.argIndent) {
                throw new GUILParseException ("Invalid indentation, should be more than the argument's indentation", state.lineNumber);
            }
            int quotePos = -1;
            if (state.InterpreterMode == ParserState.InterpMode.String) {
                quotePos = lineState.IndexOf ('\"');
            }
            else if (state.InterpreterMode == ParserState.InterpMode.PureString) {
                quotePos = lineState.IndexOf ('\'');
            }
            CatchTermination (quotePos, ref lineState, ref state);
            state.currentArgument.Data += lineState;
            if (quotePos == -1)
                lineState += "\n";
            lineState = "";
        }

        private void CatchTermination (int quotePos, ref string lineState, ref ParserState state) {
            if (quotePos == -1)
                return;
            if (lineState.Trim ().Length > quotePos + 1)
                throw new GUILParseException ("Invalid text after closing quote", state.lineNumber);
            lineState = lineState.Substring (0, quotePos);
        }
    }
}