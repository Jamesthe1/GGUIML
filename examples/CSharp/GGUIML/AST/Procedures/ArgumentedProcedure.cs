using System;
using System.Collections.Generic;
using System.Linq;
using GGUIML.Exceptions;
using GGUIML.Extensions;

namespace GGUIML.AST.Procedures {
    internal abstract class ArgumentedProcedure : BaseParserProcedure {
        protected List<IRawArgument> ParseArgs (string[] args, ref ParserState state, Action<IRawArgument> onDiscard, bool assignable = true) {
            int lineNum = state.lineNumber;
            List<IRawArgument> parsedArgs = new List<IRawArgument> ();
            bool lineOpen = false;

            args.ForEachIter ((arg, argPos) => {
                IRawArgument rawArgument;
                if (arg.NoQuotesContains ('@') || arg.NoQuotesContains ('$') || arg.StartsWith ("INHERIT")) {
                    Console.WriteLine ("REFERENCE: " + arg);
                    if (!assignable)
                        throw new GUILParseException ("Argument references are not allowed for this type", lineNum);
                    rawArgument = new RawReference {
                        Position = argPos,
                        LineNumber = lineNum
                    };
                }
                else {
                    rawArgument = new RawValue {
                        Position = argPos,
                        LineNumber = lineNum
                    };
                }

                if (arg.ProtectedContains ('=')) {
                    Console.WriteLine ("ASSIGNMENT: " + arg);
                    if (!assignable)
                        throw new GUILParseException ("Assignments are not allowed for this type", lineNum);
                    
                    string[] namedArg = arg.ProtectedSplit ('=');
                    if (namedArg.Length != 2 && (namedArg.Length > 2 || argPos != args.Length - 1)) // Only thrown when above 2, or the assignment is empty and not last
                        throw new GUILParseException ("Invalid number of equal signs when assigning to named value", lineNum);
                    rawArgument.Name = namedArg[0];
                    if (namedArg.Length == 2)
                        rawArgument.Data = namedArg[1];
                    else
                        lineOpen = true;

                    parsedArgs.Add (rawArgument);
                }
                else if (!assignable) {
                    Console.WriteLine ("DECLARATION: " + arg);
                    rawArgument.Name = arg;
                    parsedArgs.Add (rawArgument);
                }
                else if (rawArgument is RawReference) {
                    throw new GUILParseException ("Invalid implicit assignment of variable reference", lineNum);
                }
                else {
                    Console.WriteLine ("DISCARDING: " + arg);
                    rawArgument.Data = arg;
                    onDiscard?.Invoke (rawArgument);
                }
            });

            if (lineOpen) {
                state.currentArgument = parsedArgs.Last ();
                state.InterpreterMode = ParserState.InterpMode.Array;
                state.argIndent = state.indent;
            }

            return parsedArgs;
        }

        // TODO: Find unclosed strings, change interp mode to String or PureString

        public override bool MustTerminateLine (string lineState, ParserState state) {
            return true;
        }
    }
}