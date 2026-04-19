using GGUIML.AST.Types;
using GGUIML.AST.Types.Attributes;

namespace GGUIML.AST {
    public abstract class Element : IArgumentType {
        public static Dictionary<string, Type> ElementNames = new Dictionary<string, Type> {
            { "window", typeof(WindowElement) },
            { "table", typeof(TableElement) },
            { "label", typeof(LabelElement) },
            { "textbox", typeof(TextboxElement) },
            { "button", typeof(ButtonElement) },
            { "image", typeof(ImageElement) },
            { "rect", typeof(RectElement) },
            { "graph", typeof(GraphElement) },
            { "list", typeof(ListElement) },
            { "break", typeof(BreakpointElement) },
            { "progress", typeof(ProgressElement) },
        };

        public struct ElementAlignment : IArgumentType {
            public enum VerticalAlignment {
                Top,
                Center,
                Bottom
            }
            public enum HorizontalAlignment {
                Left,
                Center,
                Right
            }
            public VerticalAlignment Vertical { get; set; }
            public HorizontalAlignment Horizontal { get; set; }
            
            public IPotentialReference<RectStruct> Spacing { get; set; }

            [NotInheritable]
            public IPotentialReference<Point> Offset { get; set; }

            public static readonly ElementAlignment Centered =
                new ElementAlignment {
                    Horizontal = HorizontalAlignment.Center,
                    Vertical = VerticalAlignment.Center,
                    Spacing = new ExplicitValue<RectStruct> (RectStruct.Zero),
                    Offset = new ExplicitValue<Point> (Point.ZeroDynamic)
                };
        }

        public string Hint { get; set; } = "";  // We will not use null as there is no real use to do so.

        public IPotentialReference<ElementAlignment> Alignment { get; set; } = new ExplicitValue<ElementAlignment> (ElementAlignment.Centered);
        [NotInheritable]
        public IPotentialReference<ElementAlignment> InnerAlignment { get; set; } = new ExplicitValue<ElementAlignment> (ElementAlignment.Centered);

        [NotInheritable]
        public virtual IPotentialReference<ScaleStruct> Scale { get; set; }
        
        public enum SortOrder {
            Vertical,
            Horizontal
        }
        public virtual IPotentialReference<SortOrder> Order { get; set; } = new ExplicitValue<SortOrder> (SortOrder.Horizontal);

        public IPotentialReference<string> Style { get; set; } = new ExplicitValue<string> ("default");

        [NotInheritable]
        public IPotentialReference<string> Name { get; set; }

        public enum AppearanceType {
            Visible,
            Locked,
            Invisible
        }
        public IPotentialReference<AppearanceType> Appearance { get; set; } = new ExplicitValue<AppearanceType> (AppearanceType.Visible);

        public Element[] Children { get; set; } // This is an array because we don't want to scale this element when we're done building it

        internal int lineNumber = 0;
        internal List<RawReference> refStorage = new List<RawReference> ();
    }

    public class RectElement : Element {
        [NotInheritable]
        public IPotentialReference<ScaleStruct> InnerScale { get; set; } = new ExplicitValue<ScaleStruct> (ScaleStruct.Dynamic);
        [NotInheritable]
        public IPotentialReference<bool> Scrollable { get; set; } = new ExplicitValue<bool> (false);
    }

    public class WindowElement : RectElement {
        [NotInheritable]
        public IPotentialReference<LineSplitString> HeaderText { get; set; }
        [NotInheritable]
        public IPotentialReference<bool> Minimizable { get; set; } = new ExplicitValue<bool> (false);
        [NotInheritable]
        public IPotentialReference<bool> Maximizable { get; set; } = new ExplicitValue<bool> (false);
        [NotInheritable]
        public IPotentialReference<bool> Closable { get; set; } = new ExplicitValue<bool> (false);
        [NotInheritable]
        public IPotentialReference<bool> Resizable { get; set; } = new ExplicitValue<bool> (true);
        [NotInheritable]
        public IPotentialReference<bool> Movable { get; set; } = new ExplicitValue<bool> (true);
        [NotInheritable]
        public IPotentialReference<bool> Headerless { get; set; } = new ExplicitValue<bool> (false);
        [NotInheritable]
        public IPotentialReference<bool> Borderless { get; set; } = new ExplicitValue<bool> (false);
    }

    public class TableElement : Element {
        [NotInheritable]
        public IPotentialReference<ScaleStruct> RowsColumns { get; set; }
        [NotInheritable]
        public IPotentialReference<bool> Borderless { get; set; } = new ExplicitValue<bool> (true);

        [NotInheritable]
        public override IPotentialReference<ScaleStruct> Scale { get; set; } = new ExplicitValue<ScaleStruct> (ScaleStruct.Dynamic);
    }

    public class LabelElement : Element {
        [NotInheritable]
        public IPotentialReference<LineSplitString> Text { get; set; } = new ExplicitValue<LineSplitString> (new LineSplitString (""));
        [NotInheritable]
        public IPotentialReference<INumeric> FontSize { get; set; }
        [NotInheritable]
        public IPotentialReference<bool> Justified { get; set; }

        [NotInheritable]
        public override IPotentialReference<ScaleStruct> Scale { get; set; } = new ExplicitValue<ScaleStruct> (ScaleStruct.Dynamic);
    }

    public class TextboxElement : LabelElement {
        [NotInheritable]
        public IPotentialReference<LineSplitString> EmptyText { get; set; } = new ExplicitValue<LineSplitString> (new LineSplitString(""));
        [NotInheritable]
        public IPotentialReference<int> MaxLines { get; set; } = new ExplicitValue<int> (1);

        [NotInheritable]
        public override IPotentialReference<ScaleStruct> Scale { get; set; } =
            new ExplicitValue<ScaleStruct> (
                new ScaleStruct {
                    X = NumericPercent.Full,
                    Y = NumericFlag.Dynamic
                }
            );
    }

    public class ButtonElement : Element {
        public struct EventCall {
            public string Event { get; set; }
            public IArgumentType[] Arguments { get; set; }
        }
        [NotInheritable]
        public IPotentialReference<EventCall> Event { get; set; }

        [NotInheritable]
        public override IPotentialReference<ScaleStruct> Scale { get; set; } = new ExplicitValue<ScaleStruct> (ScaleStruct.Dynamic);
    }

    public class ImageElement : Element {
        [NotInheritable]
        public IPotentialReference<string> Path { get; set; }
        [NotInheritable]
        public IPotentialReference<string> OfflinePath { get; set; } = new ExplicitValue<string> ("");
        [NotInheritable]
        public IPotentialReference<LineSplitString> AltText { get; set; }
    }

    public class GraphElement : Element {
        public interface IGraphData {}
        public interface IGraphData<T> : IGraphData {
            [NotInheritable]
            IPotentialReference<T> Value { get; set; }
        }
        public struct NumericPoint : IGraphData<INumeric> {
            public IPotentialReference<INumeric> Value { get; set; }
        }
        public struct NestedPlot : IGraphData<Dictionary<string, NumericPoint>> {
            public IPotentialReference<Dictionary<string, NumericPoint>> Value { get; set; }
        }

        [NotInheritable]
        public IPotentialReference<Dictionary<string, IGraphData>> Data { get; set; }
    }

    public class ListElement : Element {
        public enum ListMode {
            Ordered,
            Unordered,
            Radio,
            Checkbox
        }
        [NotInheritable]
        public IPotentialReference<ListMode> Mode { get; set; }
        [NotInheritable]
        public IPotentialReference<bool> StartSelected { get; set; }

        public override IPotentialReference<SortOrder> Order { get; set; } = new ExplicitValue<SortOrder> (SortOrder.Vertical);
    }

    public class BreakpointElement : Element {}

    public class ProgressElement : Element {
        [NotInheritable]
        public IPotentialReference<NumericPercent> Value { get; set; }
    }
}