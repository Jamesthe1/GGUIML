namespace GGUIML.AST.Types {
    public interface IPotentialReference {
        object BoxedValue { get; set; }
    }

    public interface IPotentialReference<T> : IPotentialReference {
        T Value { get; set; }
    }

    public struct ExplicitValue<T> : IPotentialReference<T> {
        public T Value { get; set; }
        public object BoxedValue {
            get => Value;
            set => Value = (T)value;
        }

        public ExplicitValue (T value) {
            Value = value;
        }
    }

    public interface IReference : IArgumentType {
        string RawReferenceString { get; set; }
    }

    public interface IReference<T> : IReference, IPotentialReference<T> {}

    public struct ElementReference : IReference<Element> {
        public string RawReferenceString { get; set; }

        public Element Value { get; set; }
        public object BoxedValue {
            get => Value;
            set => Value = (Element)value;
        }
    }

    public struct VariableReference<T> : IReference<T> {
        public string RawReferenceString { get; set; }

        public T Value { get; set; }
        public object BoxedValue {
            get => Value;
            set => Value = (T)value;
        }
    }
    // TODO: In the parser, reduce references iteratively until the final reference is achieved
    // Look out for cyclic references
}