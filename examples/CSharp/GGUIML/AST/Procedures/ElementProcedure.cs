using System;
using System.Collections.Generic;
using System.Linq;
using GGUIML.Exceptions;
using GGUIML.Extensions;

namespace GGUIML.AST.Procedures {
    internal class ElementProcedure : ArgumentedProcedure {
        public override string DebugProcedureName => "Element declaration";

        public override bool ProcedureValid (string lineState, ParserState state) {
            return true;
        }

        public override void RunProcedure (ref string lineState, ref ParserState state) {
            if (state.storedHintText.EndsWith ("\n"))
                state.storedHintText = state.storedHintText.Remove (state.storedHintText.Length - 1);

            string[] args = lineState.Trim ().ProtectedSplit (' ', '\t');
            state.currentNode = new RawElement {    // Intentional override of previous element
                hintText = state.storedHintText,
                lineNumber = state.lineNumber,
                indentation = state.indent
            };
            if (state.currentSequence.Count > 0)
                state.currentSequence.Peek ().children.Add (state.currentNode);
            else
                state.rawTree.AddRoot (state.currentNode);
            state.storedHintText = "";  // Clearing out hint text for the next iteration

            List<IRawArgument> inferredArgs = new List<IRawArgument> ();
            state.currentNode.baseArgs = ParseArgs (args, ref state, inferredArgs.Add);
            // Inferred arguments are examined separately because of named arguments taking inference
            state.RefreshArgumentQueue ();
            foreach (IRawArgument arg in inferredArgs) {
                arg.Name = GetArgumentName (arg.Data, state);   // This will throw an exception if no matching argument is found
                Console.WriteLine ($"Inferred arg {arg.Data}: {arg.Name}");
                state.currentNode.baseArgs.Add (arg);
            }
        }
        
        private string GetArgumentName (string arg, ParserState state) {
            while (state.argQueue.Count > 0) {
                string argName = state.argQueue.Dequeue ();
                if (state.currentNode.baseArgs.Exists (otherArg => otherArg.Name == argName))
                    continue;
                if (ParserState.ElementArguments[argName].CanParse (arg))
                    return argName;
            }
            throw new GUILParseException ($"No valid inference could be found for argument: {arg}", state.lineNumber);
        }
    }
}