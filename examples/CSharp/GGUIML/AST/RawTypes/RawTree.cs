using System.Collections.Generic;

namespace GGUIML.AST {
    internal class RawTree {
        public List<RawNode> rootNodes = new List<RawNode> ();

        public int Count {
            get => rootNodes.Count;
        }

        public void AddRoot (RawNode node) {
            rootNodes.Add (node);
        }
    }
}