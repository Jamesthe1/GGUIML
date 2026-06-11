namespace GGUIML.AST.Procedures {
    internal class PromoteProcedure : BaseParserProcedure {
        public override string DebugProcedureName => "Parent promotion";

        public override bool ProcedureValid (string lineState, ParserState state) {
            return state.currentNode != null;   // This would only be set null if indentation were less than or equal to the previous node
        }

        public override void RunProcedure (ref string lineState, ref ParserState state) {
            state.currentSequence.Push (state.currentNode);
            state.currentNode = null;
        }

        public override bool MustTerminateLine (string lineState, ParserState state) {
            return false;
        }
    }
}