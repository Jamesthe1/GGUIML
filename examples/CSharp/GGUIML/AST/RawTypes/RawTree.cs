namespace GGUIML.AST {
    internal class RawTree {
        public List<RawNode> rootNodes;

        public int Count {
            get => rootNodes.Count;
        }

        public void AddRoot (RawNode node) {
            rootNodes.Add (node);
        }
    }
}