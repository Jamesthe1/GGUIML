using GGUIML.AST.Types;
using GGUIML.AST.Types.Attributes;

namespace GGUIML.AST {
    public abstract class Element : IArgumentType {
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

            [NotInheritable]
            public IPotentialReference<Point> Offset { get; set; }

            public static readonly ElementAlignment Centered =
                new ElementAlignment {
                    Horizontal = HorizontalAlignment.Center,
                    Vertical = VerticalAlignment.Center,
                    Offset = new ExplicitValue<Point> (Point.ZeroDynamic)
                };
        }

        public string Tooltip { get; set; } = "";   // We will not use null as we want our tooltip to be cleared.

        public IPotentialReference<ElementAlignment> Alignment { get; set; } = new ExplicitValue<ElementAlignment> (ElementAlignment.Centered);
        [NotInheritable]
        public IPotentialReference<ElementAlignment> InnerAlignment { get; set; } = new ExplicitValue<ElementAlignment> (ElementAlignment.Centered);
        public IPotentialReference<RectStruct> InnerPadding { get; set; } = new ExplicitValue<RectStruct> (RectStruct.Zero);

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

        public Element[] Children;  // This is an array because we don't want to scale this element when we're done building it
    }

    public class RectElement : Element {
        public IPotentialReference<ScaleStruct> InnerScale { get; set; } = new ExplicitValue<ScaleStruct> (ScaleStruct.Dynamic);
        public IPotentialReference<bool> Scrollable { get; set; } = new ExplicitValue<bool> (false);
    }

    public class WindowElement : RectElement {
        [NotInheritable]
        public IPotentialReference<LineSplitString> HeaderText { get; set; }
        public IPotentialReference<bool> Minimizable { get; set; } = new ExplicitValue<bool> (false);
        public IPotentialReference<bool> Maximizable { get; set; } = new ExplicitValue<bool> (false);
        public IPotentialReference<bool> Closable { get; set; } = new ExplicitValue<bool> (false);
        public IPotentialReference<bool> Resizable { get; set; } = new ExplicitValue<bool> (true);
        public IPotentialReference<bool> Movable { get; set; } = new ExplicitValue<bool> (true);
        public IPotentialReference<bool> Headerless { get; set; } = new ExplicitValue<bool> (false);
        public IPotentialReference<bool> Borderless { get; set; } = new ExplicitValue<bool> (false);
    }

    public class TableElement : Element {
        public IPotentialReference<ScaleStruct> RowsColumns { get; set; }
        public IPotentialReference<bool> Borderless { get; set; } = new ExplicitValue<bool> (true);

        [NotInheritable]
        public override IPotentialReference<ScaleStruct> Scale { get; set; } = new ExplicitValue<ScaleStruct> (ScaleStruct.Dynamic);
    }

    public class LabelElement : Element {
        [NotInheritable]
        public IPotentialReference<LineSplitString> Text { get; set; } = new ExplicitValue<LineSplitString> (new LineSplitString (""));

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
        public IPotentialReference<EventCall> Event { get; set; }

        public override IPotentialReference<ScaleStruct> Scale { get; set; } = new ExplicitValue<ScaleStruct> (ScaleStruct.Dynamic);
    }

    public class ImageElement : Element {
        public IPotentialReference<string> Path { get; set; }
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
        public IPotentialReference<ListMode> Mode { get; set; }
        [NotInheritable]
        public IPotentialReference<bool> StartSelected { get; set; }

        public override IPotentialReference<SortOrder> Order { get; set; } = new ExplicitValue<SortOrder> (SortOrder.Vertical);
    }

    public class BreakpointElement : Element {}
}