using System;
using System.Collections.Generic;
using System.Linq;
using GGUIML.AST;
using GGUIML.AST.Types;
using GGUIML.Extensions;

namespace GGUIML {
    internal class ParserState {
        public enum IndentMode {
            Unknown,
            Tabs,
            Spaces
        }

        public IndentMode indentMode = IndentMode.Unknown;

        public int indent = 0;

        public int typeargIndent = -1;

        public int argIndent = -1;

        public int lineNumber = 1;

        public Stack<RawNode> currentSequence = new Stack<RawNode> ();

        public RawTree rawTree = new RawTree ();

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
            Array,
        }

        public InterpMode InterpreterMode {
            get => interpMode;
            set {
                prevInterpMode = interpMode;
                interpMode = value;
            }
        }

        private InterpMode interpMode = InterpMode.None;

        public InterpMode prevInterpMode = InterpMode.None;

        private static EnumType AlignmentEnum = new EnumType (
            new string[] { "top", "center", "bottom" },
            new string[] { "left", "center", "right" }
        );
        private static MultiType AlignmentType = new MultiType (AlignmentEnum, new Dictionary<char, RawType> {
            { '[', new RectType () },
            { '(', new PointType () }
        });
        public static Dictionary<string, RawType> ElementArguments = new Dictionary<string, RawType> {
            { "flow-mode", new EnumType ("static", "floating") },
            { "alignment", AlignmentType },
            { "inner-alignment", AlignmentType },
            { "scale", new ScaleType (typeof(NumericInt), typeof(NumericDecimal), typeof(NumericFlag)) },
            { "order", new EnumType ("vertical", "horizontal") },
            { "style", new StringType (multiline: false, allowSpecial: false) },
            { "appearance", new EnumType ("visible", "locked", "invisible") },
            { "type", new EnumType (Element.ElementNames.Keys.ToArray ()) },
            { "name", new StringType (false, false, false) }
        };

        public Queue<string> argQueue = new Queue<string> ();

        public void RefreshArgumentQueue () {
            argQueue = new Queue<string> (ElementArguments.Keys);
        }
    }
}