namespace GGUIML.AST.Types.Attributes {
    /// <summary>
    /// Marks the argument as required for the parser.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RequiredArgumentAttribute : Attribute {

    }
}