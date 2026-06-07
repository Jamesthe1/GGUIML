using GGUIML.Exceptions;

namespace GGUIML.AST.Procedures {
    internal class HintTextProcedure : BaseParserProcedure {
        public override bool ProcedureValid (ref string lineState, ref ParserState state) {
            if (state.InterpreterMode == ParserState.InterpMode.HintText) {
                return true;
            }
            return lineState.StartsWith ("##") || lineState.StartsWith ("##(");
        }

        public override void RunProcedure (ref string lineState, ref ParserState state) {
            state.storedHintText += ExtractHintText (ref lineState, ref state) + "\n";
        }

        private string ExtractHintText (ref string lineState, ref ParserState state) {
            string innerText;
            if (lineState.StartsWith ("##(") && state.InterpreterMode == ParserState.InterpMode.None) {
                lineState = lineState.Remove (0, 3);
                state.InterpreterMode = ParserState.InterpMode.HintText;
            }
            if (lineState.Contains (")##")) {
                if (state.InterpreterMode != ParserState.InterpMode.HintText)
                    throw new GUILParseException ("Invalid tooltip closure", state.lineNumber);
                
                int endIdx = lineState.IndexOf (")##");
                innerText = lineState.Substring (0, endIdx).Trim ();
                lineState = lineState.Remove (0, endIdx + 3);
                
                if (lineState.Trim ().Length > 0)
                    throw new GUILParseException ("Invalid text after tooltip closure", state.lineNumber);
                
                state.InterpreterMode = ParserState.InterpMode.None;
                return innerText;
            }
            if (lineState.StartsWith ("##") && state.InterpreterMode == ParserState.InterpMode.None) {
                innerText = lineState.Remove (0, 2).Trim ();
                lineState = "";
                return innerText;
            }

            if (state.InterpreterMode != ParserState.InterpMode.HintText)
                return "";
            
            innerText = lineState;
            lineState = "";
            return innerText;
        }
    }
}