using System.Collections.Generic;

namespace GGUIML.AST {
    internal abstract class RawNode {
        public int lineNumber;
        public int indentation;
        public List<IRawArgument> baseArgs = new List<IRawArgument> ();
        public List<RawNode> children = new List<RawNode> ();

        public override string ToString () {
            string result = lineNumber + ": ";
            for (int i = 0; i < baseArgs.Count; i++) {
                IRawArgument arg = baseArgs[i];
                result += arg.Name + "=" + arg.Data + $" (position {arg.Position}, line {arg.LineNumber})";
                if (i < baseArgs.Count - 1)
                    result += ", ";
            }
            return result;
        }
    }

    internal class RawElement : RawNode {
        public string hintText;
        public List<IRawArgument> typeArgs = new List<IRawArgument> ();

        public override string ToString () {
            string result = base.ToString () + " <";
            for (int i = 0; i < typeArgs.Count; i++) {
                IRawArgument arg = typeArgs[i];
                result += arg.Name + "=" + arg.Data + $" (position {arg.Position}, line {arg.LineNumber})";
                if (i < baseArgs.Count - 1)
                    result += ", ";
            }
            result += $"> \"{hintText}\"";
            return result;
        }
    }

    internal class RawModule : RawNode {
        public bool template;
        public string name;

        public override string ToString () {
            string modType = template ? "template" : "module";
            return $"[{modType} {name}] " + base.ToString();
        }
    }

    internal class RawImport : RawNode {
        public string name;

        public override string ToString () {
            return $"[import {name}] " + base.ToString();
        }
    }
}