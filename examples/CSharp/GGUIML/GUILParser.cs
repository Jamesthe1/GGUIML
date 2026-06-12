using System;
using System.Collections.Generic;
using System.IO;

using GGUIML.AST;
using GGUIML.AST.Procedures;
using GGUIML.Exceptions;

namespace GGUIML {
    public class GUILParser<TBaseImpl> {
        private static readonly BaseParserProcedure[] Procedures = new BaseParserProcedure[] {
            new CommentProcedure (),
            new IndentProcedure (),
            new ArrayProcedure (),
            new StringProcedure (),
            new HintTextProcedure (),
            new TypeargProcedure (),
            new PromoteProcedure (),
            new ModuleProcedure (),
            new ElementProcedure (),
        };

        public Element[] Elements { get; private set; }

        public class AssociatedElement {
            public Element Element { get; set; }
            public TBaseImpl Implementation { get; set; }
        }

        private List<AssociatedElement> associations = new List<AssociatedElement> ();

        // TODO: Separate class that contains our resulting complete tree, with warnings
        private List<ParserWarning> warnings = new List<ParserWarning> ();

        private RawTree rawTree;

        private void DebugPrintRawTree (List<RawNode> nodes, int depth = 0) {
            for (int i = 0; i < nodes.Count; i++) {
                string line = new string(' ', depth * 2);
                if (depth > 0) {
                    if (i == nodes.Count - 1)
                        line += "╘ ";
                    else
                        line += "╞ ";
                }
                line += nodes[i].ToString ();
                Console.WriteLine (line);
                DebugPrintRawTree (nodes[i].children, depth + 1);
            }
        }

        public void DebugPrintRawTree () {
            Console.WriteLine (warnings.Count + " warnings");
            for (int w = 0; w < warnings.Count; w++) {
                ParserWarning warning = warnings[w];
                Console.WriteLine (warning.Message + $" (line {warning.LineNumber})");
            }
            DebugPrintRawTree (rawTree.rootNodes);
        }

        public GUILParser (Stream guilStream) {
            using (StreamReader reader = new StreamReader (guilStream)) {
                string line = reader.ReadLine ();
                if (line.TrimEnd () != "!GGUIML")
                    throw new GUILParseException ("Invalid identifier at beginning of file, should be !GGUIML", 0);
                rawTree = ParseElementsAndChildren (reader);
            }
        }

        private RawTree ParseElementsAndChildren (StreamReader reader) {
            ParserState state = new ParserState ();
            while (true) {
                state.lineNumber++;
                Console.WriteLine ($"/// LINE {state.lineNumber} ///");

                string lineState = reader.ReadLine ();
                Console.WriteLine ("Data acquired");
                if (lineState == null || lineState.Trim () == "")
                    break;
                
                foreach (BaseParserProcedure procedure in Procedures) {
                    if (procedure.ProcedureValid (lineState, state)) {
                        Console.WriteLine ("Running procedure: " + procedure.DebugProcedureName);
                        procedure.RunProcedure (ref lineState, ref state);
                        if (procedure.MustTerminateLine (lineState, state))
                            break;
                    }
                }
            }
            return state.rawTree;
        }

        // TODO: Provide a "ScaleToViewport" function that creates a "ScaledTree" with fully-resolved references, and positions as pixels. It should walk along the tree and fire a publically-exposed event with appropriate contexts (root position, padding, parent) on each element
        // This will also populate the "associations" member
    }
}