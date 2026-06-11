using GGUIML.Exceptions;
using GGUIML.Extensions;

namespace GGUIML.AST.Procedures {
    internal class CommentProcedure : BaseParserProcedure {
        public override string DebugProcedureName => "Comment removal";

        public override bool ProcedureValid (string lineState, ParserState state) {
            // This function is intended to not be ran on tooltips; this check here prevents treating the appearance of comments on the same line
            if (lineState.StartsWith ("##") || lineState.StartsWith ("##("))
                return false;
            
            int cmPos = lineState.NoQuotesFindIndex ('#');
            int cmLongPos = lineState.NoQuotesFindIndex ("#(");
            if (cmPos == -1 && cmLongPos == -1)
                return false;
            
            if (cmPos != -1 && (cmPos + 1 >= lineState.Length || lineState.Substring (cmPos, 2) != "##")) {
                return true;
            }
            if (cmLongPos != -1 && (cmLongPos - 1 < 0 || lineState.Substring (cmLongPos - 1, 3) != "##(")) {
                return true;
            }

            return state.InterpreterMode == ParserState.InterpMode.Comment;
        }

        public override void RunProcedure (ref string lineState, ref ParserState state) {
            if (lineState.NoQuotesContains (")#")) {
                if (state.InterpreterMode != ParserState.InterpMode.Comment)
                    throw new GUILParseException ("Invalid comment closure", state.lineNumber);
                
                int idx = lineState.IndexOf (")#");
                if (idx + 3 >= lineState.Length || lineState.Substring (idx, 3) != ")##") {
                    lineState = lineState.Substring (idx + 2);
                    state.InterpreterMode = ParserState.InterpMode.None;
                    return;
                }
            }
            else if (state.InterpreterMode != ParserState.InterpMode.Comment) {
                if (lineState.Contains ("#(")) {
                    int idx = lineState.IndexOf ("#(");
                    if (lineState.Substring (idx - 1, 3) != "##(") {
                        lineState = lineState.Substring (0, idx);
                        state.InterpreterMode = ParserState.InterpMode.Comment;
                        return;
                    }
                }
                if (lineState.Contains ("#")) {
                    int idx = lineState.IndexOf ("#");
                    if (lineState.Substring (idx, 2) != "##") { // "IndexOf" will only trigger on the first encounter from the start of the string, no need to check backwards
                        lineState = lineState.Substring (0, idx);
                        return;
                    }
                }
            }

            // We can safely presume we are in "Comment" mode now
            lineState = "";
        }

        public override bool MustTerminateLine (string lineState, ParserState state) {
            return base.MustTerminateLine (lineState, state) || state.InterpreterMode == ParserState.InterpMode.Comment;
        }
    }
}