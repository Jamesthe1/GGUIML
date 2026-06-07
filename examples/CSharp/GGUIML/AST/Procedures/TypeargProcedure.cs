using System.Collections.Generic;
using System.Linq;
using GGUIML.Exceptions;
using GGUIML.Extensions;

namespace GGUIML.AST.Procedures {
    internal class TypeargProcedure : ArgumentedProcedure {
        public override bool ProcedureValid (ref string lineState, ref ParserState state) {
            if (state.InterpreterMode == ParserState.InterpMode.Typearg) {
                return state.indent > state.typeargIndent;
            }
            return lineState.StartsWith ("TYPEARG");
        }

        public override void RunProcedure (ref string lineState, ref ParserState state) {
            string[] args = lineState.Trim ().ProtectedSplit (' ', '\t');
            RawElement currentElement = state.currentNode as RawElement;
            if (args[0] == "TYPEARG") {
                if (state.currentNode == null)
                    throw new GUILParseException ("Type arguments declared but no element of which to give them to, check indentation", state.lineNumber);
                if (state.indent <= state.currentNode.indentation)   // We might not actually reach a case where indentation is *less* than our element, but better safe than sorry
                    throw new GUILParseException ("Type arguments have incorrect indentation", state.lineNumber);
                if (currentElement == null)
                    throw new GUILParseException ("Type arguments cannot be declared on modules, templates, or imports", state.lineNumber);
                
                state.InterpreterMode = ParserState.InterpMode.Typearg;
                state.typeargIndent = state.indent;
                args = args.Skip (1).ToArray ();
                if (args.Length == 0)
                    return;
            }
            
            int lineNum = state.lineNumber;
            List<IRawArgument> rawArgs = ParseArgs (args, ref state, arg => throw new GUILParseException ($"Type argument ({arg}) needs to be assigned by name", lineNum));
            currentElement.typeArgs.AddRange (rawArgs);
        }

        public override bool TerminateLine (ref string lineState, ref ParserState state) {
            return true;
        }
    }
}