using System.Linq;
using GGUIML.Exceptions;
using GGUIML.Extensions;

namespace GGUIML.AST.Procedures {
    internal class ModuleProcedure : ArgumentedProcedure {
        public override bool ProcedureValid (ref string lineState, ref ParserState state) {
            return lineState.StartsWith ("MODULE") || lineState.StartsWith ("TEMPLATE") || lineState.StartsWith ("IMPORT");
        }

        public override void RunProcedure (ref string lineState, ref ParserState state) {
            string[] args = lineState.Trim ().ProtectedSplit (' ', '\t');
            if (args.Count () < 2)
                throw new GUILParseException ($"{args[0]} is missing a name", state.lineNumber);
            if (state.storedHintText != "") {
                state.storedHintText = "";
                Warnings.Add (new ParserWarning ("Hint text is not allowed for modules, templates, or imports", state.lineNumber - 1));
            }

            int lineNum = state.lineNumber;

            if (args[0] == "IMPORT") {
                state.currentNode = new RawImport {
                    name = args[1],
                    lineNumber = lineNum,
                    indentation = state.indent
                };
            }
            else {
                state.currentNode = new RawModule {
                    template = args[0] == "TEMPLATE",
                    name = args[1],
                    lineNumber = lineNum,
                    indentation = state.indent
                };
            }

            state.currentNode.baseArgs = ParseArgs (args.Skip (2).ToArray (), ref state, null, state.currentNode is RawImport); // TODO: Add onDiscard for imports, check types and if the arg can be skipped should they not match
        }

        public override bool TerminateLine (ref string lineState, ref ParserState state) {
            return true;
        }
    }
}