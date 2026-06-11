using GGUIML.Exceptions;

namespace GGUIML.AST.Procedures {
    internal class IndentProcedure : BaseParserProcedure {
        public override string DebugProcedureName => "Indentation acquisition";

        public override bool ProcedureValid (string lineState, ParserState state) {
            return true;
        }

        public override void RunProcedure (ref string lineState, ref ParserState state) {
            state.indent = ExtractIndentation (ref lineState, ref state);
            if (state.indent > 0 && state.rawTree.Count == 0)
                Warnings.Add (new ParserWarning ("Element has indentation but no parent, this may be the sign of a missing element or incorrect parenting", state.lineNumber));
            if (state.indent <= state.argIndent) {
                state.argIndent = -1;
                state.currentArgument = null;
                state.InterpreterMode = state.prevInterpMode;
            }
            if (state.indent <= state.typeargIndent) {
                state.typeargIndent = -1;
                state.InterpreterMode = ParserState.InterpMode.None;
            }
            HandleParenting (ref state);
        }

        private int ExtractIndentation (ref string lineState, ref ParserState state) {
            int indentation = 0;

            bool lineTab = lineState.StartsWith ("\t");
            bool lineSpace = lineState.StartsWith (" ");
            if (!lineTab && !lineSpace)
                return 0;
            
            switch (state.indentMode) {
                case ParserState.IndentMode.Unknown:
                    if (lineTab) {
                        state.indentMode = ParserState.IndentMode.Tabs;
                        goto case ParserState.IndentMode.Tabs;
                    }
                    state.indentMode = ParserState.IndentMode.Spaces;
                    goto case ParserState.IndentMode.Spaces;
                case ParserState.IndentMode.Tabs:
                    while (lineState.StartsWith ("\t")) {   // Not using the variable since it is only evaluated once
                        indentation++;
                        lineState = lineState.Remove (0, 1);

                        if (indentation >= state.indent && (state.InterpreterMode == ParserState.InterpMode.Comment || state.InterpreterMode == ParserState.InterpMode.HintText))
                            break;
                        if (indentation > state.currentNode.indentation + 1 && indentation > state.typeargIndent + 1 && indentation > state.argIndent + 1)
                            Warnings.Add (new ParserWarning ("Multiple indentations detected, this might indicate a missing parent or create undesired behavior", state.lineNumber));
                    }
                    if (lineState.StartsWith (" "))
                        throw new GUILParseException ("Mixed indentations are not allowed", state.lineNumber);
                    break;
                case ParserState.IndentMode.Spaces:
                    while (lineState.StartsWith (" ")) {
                        indentation++;
                        lineState = lineState.Remove (0, 1);

                        if (indentation >= state.indent && state.InterpreterMode != ParserState.InterpMode.None && state.InterpreterMode != ParserState.InterpMode.Typearg)
                            break;
                    }
                    if (lineState.StartsWith ("\t"))
                        throw new GUILParseException ("Mixed indentations are not allowed", state.lineNumber);
                    break;
            }
            return indentation;
        }

        private void HandleParenting (ref ParserState state) {
            if (state.currentNode != null && state.indent <= state.currentNode.indentation)
                state.currentNode = null;
            // Chances are this may not fire but it's good to clear out any parents we are also no longer a part of
            while (state.currentSequence.Count > 0 && state.indent <= state.currentSequence.Peek ().indentation) {
                if (state.InterpreterMode == ParserState.InterpMode.HintText)
                    throw new GUILParseException ("Dangling hint text; this might imply that the text is not aligned with the element's indentation", state.lineNumber);
                state.currentSequence.Pop ();
                break;
            }
        }
    }
}