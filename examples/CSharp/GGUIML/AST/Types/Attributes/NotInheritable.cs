namespace GGUIML.AST.Types.Attributes {
    /// <summary>
    /// Marks the property or field as invalid for inheritance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotInheritableAttribute : Attribute {

    }
}