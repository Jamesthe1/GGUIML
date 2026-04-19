using GGUIML.AST;
using GGUIML.Exceptions;
using GGUIML.Extensions;

namespace GGUIML {
    public class GUILParser<TBaseImpl> {
        public class AssociatedElement {
            public Element Element { get; set; }
            public TBaseImpl Implementation { get; set; }
        }

        private class ParserState {
            public enum IndentMode {
                Unknown,
                Tabs,
                Spaces
            }

            public IndentMode indentMode = IndentMode.Unknown;

            public int indent = 0;

            public int lineNumber = 0;

            public Stack<RawNode> currentSequence = new Stack<RawNode> ();

            public List<RawNode> rawTree = new List<RawNode> ();

            public RawNode currentNode = null;

            public IRawArgument currentArgument = null; // TODO: If set and InterpMode is String or PureString, add to the data. Once a specified endpoint is met, set this to null and interpMode to None.

            public string storedHintText = "";

            public enum InterpMode {
                None,
                Comment,
                HintText,
                Typearg,
                String,
                PureString,
            }

            public InterpMode interpMode = InterpMode.None;

            public delegate bool ArgCheck (string str);
            public static Dictionary<string, ArgCheck> ArgumentChecks = new Dictionary<string, ArgCheck> {
                { "alignment/margin/offset", (s) => AlignmentCharacterMatches (s, '[', '(') },
                { "alignment/offset", (s) => AlignmentCharacterMatches (s, '(') },
                { "alignment/margin", (s) => AlignmentCharacterMatches (s, '[') },
                { "alignment", AlignmentMatches },
                { "inner-alignment/padding/offset", (s) => AlignmentCharacterMatches (s, '[', '(') },
                { "inner-alignment/offset", (s) => AlignmentCharacterMatches (s, '(') },
                { "inner-alignment/padding", (s) => AlignmentCharacterMatches (s, '[') },
                { "inner-alignment", AlignmentMatches },
                { "scale", s => ParserExtensions.CanParse (() => s.ParsePoint ()) },
                { "order", s => typeof (Element.SortOrder).EnumContainsConverted (s) },
                { "style", s => ParserExtensions.CanParse (() => s.ParseString ()) },
                { "appearance", s => typeof (Element.AppearanceType).EnumContainsConverted (s) },
                { "type", s => Element.ElementNames.ContainsKey (s) },
                { "name", s => ParserExtensions.CanParse (() => s.ParseString ()) }
            };

            public Queue<string> argQueue = new Queue<string> ();   // TODO: In parser func, refresh, iterate, validate

            public void RefreshArgumentQueue () {
                argQueue = new Queue<string> (ArgumentChecks.Keys);
            }

            private static bool AlignmentCharacterMatches (string str, params char[] acceptedChars) {
                bool argsOk = true;
                foreach (char c in acceptedChars)
                    argsOk = argsOk && str.Contains (c);
                return AlignmentMatches (str.Substring (0, str.IndexOf (acceptedChars[0])));
            }

            private static bool AlignmentMatches (string str) {
                bool argsOk;
                if (str.Contains ('-')) {
                    string[] subargs = str.Split ('-');
                    if (subargs.Length > 0)
                        throw new FormatException ("Alignment contains invalid number of hyphens");
                    argsOk = typeof (Element.ElementAlignment.VerticalAlignment).EnumContainsConverted (subargs[0]);
                    argsOk = argsOk && typeof (Element.ElementAlignment.HorizontalAlignment).EnumContainsConverted (subargs[0]);
                }
                else {
                    argsOk = typeof (Element.ElementAlignment.VerticalAlignment).EnumContainsConverted (str);
                    argsOk = argsOk || typeof (Element.ElementAlignment.HorizontalAlignment).EnumContainsConverted (str);
                }
                return argsOk;
            }
        }

        public Element[] Elements { get; private set; }

        private List<AssociatedElement> associations;

        public struct ParserWarning {
            public string Message { get; set; }
            public int LineNumber { get; set; }

            public ParserWarning (string message, int lineNum) {
                Message = message;
                LineNumber = lineNum;
            }
        }

        // TODO: Separate class that contains our resulting complete tree, with warnings
        // Make ParserWarning a class and add inheriting classes
        private List<ParserWarning> warnings;  // TODO: Make class of string message, int line number

        public GUILParser (Stream guilStream) {
            using (StreamReader reader = new StreamReader (guilStream)) {
                string line = reader.ReadLine ();
                if (line.TrimEnd () != "!GGUIML")
                    throw new GUILParseException ("Invalid identifier at beginning of file, should be !GGUIML", 0);
                ParseElementsAndChildren (reader);
            }
        }

        private void ParseElementsAndChildren (StreamReader reader) {
            ParserState state = new ParserState ();
            while (true) {
                state.lineNumber++;

                string lineState = reader.ReadLine ();
                if (lineState == null)
                    break;
                
                state.indent = ExtractIndentation (ref lineState, ref state);

                // Wanting to warn just in case someone forgets to make a parent element
                if (state.indent > 0 && state.rawTree.Count == 0)
                    warnings.Add (new ParserWarning ("Element has indentation but no parent, this may be the sign of a missing element or incorrect parenting", state.lineNumber));

                if (state.currentNode != null)
                    HandleParenting (ref state);
                
                if (state.interpMode != ParserState.InterpMode.HintText)
                    HandleComments (ref lineState, ref state);  // Chances are this may change the state to "Comment", so we're not going to add an "else" to the next line
                if (lineState == "" || state.interpMode == ParserState.InterpMode.Comment)
                    continue;
                
                state.storedHintText += ExtractHintText (ref lineState, ref state);
                if (lineState == "" || state.interpMode == ParserState.InterpMode.HintText) // Just in case line state isn't cleared fsr, this state check is a fallback
                    continue;
                
                string[] args = lineState.Trim ().ProtectedSplit (' ', '\t');
                
                if (args[0] == "TYPEARG" || state.interpMode == ParserState.InterpMode.Typearg) {
                    HandleTypeargs (args, ref state);   // This will change the interp mode, so the check below is needed
                    if (state.interpMode == ParserState.InterpMode.Typearg)
                        continue;
                }

                if (state.currentNode != null && state.indent > state.currentNode.indentation) {
                    // We can safely deduce that this is not a typearg or other definition, so we should be promoting the current element to parent
                    state.currentSequence.Push (state.currentNode);
                    state.currentNode = null;
                }

                if (state.currentNode != null)
                    warnings.Add (new ParserWarning ("Element still exists while creating a new one, this is probably the sign of an error with the parser", state.lineNumber));

                if (args[0] == "MODULE" || args[0] == "TEMPLATE") {
                    HandleModules (args, ref state);
                    continue;
                }

                state.currentNode = new RawElement {
                    hintText = state.storedHintText,
                    lineNumber = state.lineNumber,
                    indentation = state.indent
                };
                if (state.currentSequence.Count > 0)
                    state.currentSequence.Last ().children.Add (state.currentNode);
                else
                    state.rawTree.Add (state.currentNode);
                state.storedHintText = "";  // Clearing out hint text for the next iteration

                List<string> inferredArgs = new List<string> ();
                args.ForEachIter ((arg, argPos) => {
                    IRawArgument rawArgument;
                    if (arg.Contains ('@') || arg.Contains ('$') || arg.StartsWith ("INHERIT")) {
                        rawArgument = new RawReference {
                            Position = argPos,
                            LineNumber = state.lineNumber
                        };
                    }
                    else {
                        rawArgument = new RawValue {
                            Position = argPos,
                            LineNumber = state.lineNumber
                        };
                    }

                    if (rawArgument.Data.Contains ('=')) {
                        string[] namedArg = arg.ProtectedSplit ('=');
                        if (namedArg.Length > 2)
                            throw new GUILParseException ("Invalid number of equal signs when assigning to named value", state.lineNumber);
                        rawArgument.Name = namedArg[0];
                        rawArgument.Data = namedArg[1];
                    }
                    else {
                        inferredArgs.Add (arg);
                    }

                    state.currentNode.baseArgs.Add (rawArgument);
                });
                // Inferred arguments are examined separately because of named arguments taking inference
                state.RefreshArgumentQueue ();
                inferredArgs.ForEachIter ((arg, argPos) => {
                    string argName = GetArgumentName (arg, ref state);   // This will throw an exception if no matching argument is found
                    IRawArgument rawArgument = new RawValue {
                        Name = argName,
                        Data = arg,
                        Position = argPos,
                        LineNumber = state.lineNumber,
                    };
                    state.currentNode.baseArgs.Add (rawArgument);
                });
            }
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
                    // TODO: if there is more than one additional indentation than the state, and the state is not in "tooltip" mode, throw error
                    while (lineState.StartsWith ("\t")) {
                        indentation++;
                        lineState.Remove (0, 1);

                        if (indentation >= state.indent && state.interpMode != ParserState.InterpMode.None)
                            break;
                    }
                    break;
                case ParserState.IndentMode.Spaces:
                    while (lineState.StartsWith (" ")) {
                        indentation++;
                        lineState.Remove (0, 1);

                        if (indentation >= state.indent && state.interpMode != ParserState.InterpMode.None)
                            break;
                    }
                    break;
            }
            return indentation;
        }

        private void HandleParenting (ref ParserState state) {
            if (state.indent <= state.currentNode.indentation) {
                state.currentNode = null;
                // Chances are this may not fire but it's good to clear out any parents we are no longer a part of
                while (state.indent <= state.currentSequence.Peek ().indentation) {
                    if (state.currentSequence.Count == 0)
                        break;
                    state.currentSequence.Pop ();
                }
            }
        }

        private void HandleTypeargs (string[] args, ref ParserState state) {
            if (args[0] == "TYPEARG") {
                if (state.currentNode == null)
                    throw new GUILParseException ("Type arguments declared but no element of which to give them to, check indentation", state.lineNumber);
                if (state.indent <= state.currentNode.indentation)   // We might not actually reach a case where indentation is *less* than our element, but better safe than sorry
                    throw new GUILParseException ("Type arguments have incorrect indentation", state.lineNumber);
                if (state.currentNode is RawElement == false)
                    throw new GUILParseException ("Type arguments cannot be declared on modules or templates", state.lineNumber);
                
                state.interpMode = ParserState.InterpMode.Typearg;
            }

            if (args[0] != "TYPEARG" && state.indent <= state.currentNode.indentation + 1) {
                state.interpMode = ParserState.InterpMode.None;
                return;
            }
            // TODO: Parse named arguments
        }

        private void HandleModules (string[] args, ref ParserState state) {
            state.currentNode = new RawModule {
                template = args[0] == "TEMPLATE",
                lineNumber = state.lineNumber,
                indentation = state.indent
            };

            int lineNum = state.lineNumber;

            args.Skip (1).ForEachIter ((s, i) => new RawValue {
                Name = s,
                Data = null,
                LineNumber = lineNum,
                Position = i
            });
        }

        private string ExtractHintText (ref string lineState, ref ParserState state) {
            string innerText;
            if (lineState.StartsWith ("##(") && state.interpMode == ParserState.InterpMode.None) {
                lineState = lineState.Remove (0, 3);
                state.interpMode = ParserState.InterpMode.HintText;
            }
            if (lineState.Contains (")##")) {
                if (state.interpMode != ParserState.InterpMode.HintText)
                    throw new GUILParseException ("Invalid tooltip closure", state.lineNumber);
                
                int endIdx = lineState.IndexOf (")##");
                innerText = lineState.Substring (0, endIdx).Trim ();
                lineState = lineState.Remove (0, endIdx + 3);
                
                if (lineState.Trim ().Length > 0)
                    throw new GUILParseException ("Invalid text after tooltip closure", state.lineNumber);
                
                state.interpMode = ParserState.InterpMode.None;
                return innerText;
            }
            if (lineState.StartsWith ("##") && state.interpMode == ParserState.InterpMode.None) {
                lineState = lineState.Remove (0, 2);
            }

            if (state.interpMode != ParserState.InterpMode.HintText)
                return "";
            
            innerText = lineState.Trim ();
            lineState = "";
            return innerText;
        }

        private void HandleComments (ref string lineState, ref ParserState state) {
            // This function is intended to not be ran on tooltips; this check here prevents treating the appearance of comments on the same line
            if (lineState.StartsWith ("##") || lineState.StartsWith ("##("))
                return;

            if (lineState.Contains ("#(") && state.interpMode != ParserState.InterpMode.Comment) {
                int idx = lineState.IndexOf ("#(");
                if (idx == 0 || lineState.Substring (idx - 1, 3) != "##(") {    // Tooltip sanity check
                    lineState = lineState.Substring (0, idx);
                    state.interpMode = ParserState.InterpMode.Comment;
                }
            }
            if (lineState.Contains (")#")) {
                if (state.interpMode != ParserState.InterpMode.Comment)
                    throw new GUILParseException ("Invalid comment closure", state.lineNumber);
                
                int idx = lineState.IndexOf (")#");
                if (lineState.Substring (0, idx + 3) != ")##") {
                    lineState = lineState.Substring (idx + 2);
                    state.interpMode = ParserState.InterpMode.None;
                }
            }
            if (lineState.Contains ("#") && state.interpMode != ParserState.InterpMode.Comment) {
                int idx = lineState.IndexOf ("#");
                if (lineState.Substring (idx, 2) != "##") { // "IndexOf" will only trigger on the first encounter from the start of the string, no need to check backwards
                    lineState = lineState.Substring (0, idx);
                }
            }
        }

        private string GetArgumentName (string arg, ref ParserState state) {
            while (state.argQueue.Count > 0) {
                string argName = state.argQueue.Dequeue ();
                if (state.currentNode.baseArgs.Exists (otherArg => otherArg.Name == argName))
                    continue;
                if (ParserState.ArgumentChecks[argName].Invoke (arg))
                    return argName;
            }
            throw new GUILParseException ($"No valid inference could be found for argument: {arg}", state.lineNumber);
        }

        // TODO: Provide a "ScaleToViewport" function that creates a "ScaledTree" with fully-resolved references, and positions as pixels. It should walk along the tree and fire a publically-exposed event with appropriate contexts (root position, padding, parent) on each element
    }
}