using System;
using System.Collections.Generic;
using System.Linq;
using GGUIML.AST;
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
            { "scale", s => ParserExtensions.CanParse (s.ParseScale) },
            { "order", typeof (Element.SortOrder).EnumContainsConverted },
            { "style", s => ParserExtensions.CanParse (s.ParseString) },
            { "appearance", typeof (Element.AppearanceType).EnumContainsConverted },
            { "type", Element.ElementNames.ContainsKey },
            { "name", s => ParserExtensions.CanParse (s.ParseString) }
        };

        public Queue<string> argQueue = new Queue<string> ();   // TODO: In parser func, refresh, iterate, validate

        public void RefreshArgumentQueue () {
            argQueue = new Queue<string> (ArgumentChecks.Keys);
        }

        private static bool AlignmentCharacterMatches (string str, params char[] acceptedChars) {
            bool argsOk = true;
            foreach (char c in acceptedChars)
                argsOk = argsOk && str.Contains (c);
            if (argsOk)
                return AlignmentMatches (str.Substring (0, str.IndexOf (acceptedChars[0])));
            else
                return false;
        }

        private static bool AlignmentMatches (string str) {
            bool argsOk;
            Type vaType = typeof (Element.ElementAlignment.VerticalAlignment);
            Type haType = typeof (Element.ElementAlignment.HorizontalAlignment);
            if (str.Contains ('-')) {
                string[] subargs = str.Split ('-');
                if (subargs.Length > 2)
                    throw new FormatException ("Alignment contains invalid number of hyphens");
                argsOk = vaType.EnumContainsConverted (subargs[0]) && haType.EnumContainsConverted (subargs[1]);
            }
            else {
                argsOk = vaType.EnumContainsConverted (str) || haType.EnumContainsConverted (str);
            }
            return argsOk;
        }
    }
}