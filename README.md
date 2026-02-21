# GGUIML

The GGUIML (Global Graphical User Interface Markup Language), or simply GUIL ("gill"), is a lightweight markup language for abstract user interfaces. It is intended to be style-agnostic and widely compatible with various graphics APIs.

GGUIML is not a competitor to SDL, OpenGL, or other graphics APIs, but is instead complimentary to them. The language is purely for layout purposes for any given UI API, and the rest is determined by the application designer.

## Definitions

The key words "MUST", "MUST NOT", "REQUIRED", "SHALL", "SHALL
NOT", "SHOULD", "SHOULD NOT", "RECOMMENDED",  "MAY", and
"OPTIONAL" in this document are to be interpreted as described in
[RFC 2119](https://datatracker.ietf.org/doc/html/rfc2119).

- "User": Any entity interfacing with a program that presents information created by the designer.
- "Designer": A programmer or utility creating GGUIML to be handled by a program.
- "Program": A software that uses an implementation of GGUIML.
- "Implementation": An API or software that parses GGUIML.

## Interpretation

A text file for the language MUST start with `!GGUIML` and a newline afterward; and the file extension may be `.ggui` or `.guil`.

The topmost element is the document. It MAY have no parent, but the implementation or program SHOULD document if it does have one.

If a feature or element is unavailable in a given API, it MUST be discarded and SHOULD give a warning to the designer, but it SHOULD NOT stop the program. Custom elements MUST NOT be present. The strict palette of elements in the language ensures that one GUI file is compatible across multiple users.

A numeric MUST either be:
- Integers which contain no suffix, also referred to as "pixels".
- Percentages which MUST be suffixed with `%`, the number MAY include a decimal point.
- Decimals, which MUST include a decimal point.
- `DYNAMIC`, a special keyword used to indicate that this item is flexible.

A string MUST either be surrounded by single-quotes (`'`) or double-quotes (`"`). The escape character `\` conforms to ISO C escape sequences and MUST be interpreted as such, only if it is present in double-quotes.

A boolean MUST either be `on` or `off`, representing `true` and `false` in the context of a UI. The intent is to better present features rather than states.

A single-line comment MUST begin with a single hashtag/pound symbol (`#`), whereas tooltip text MUST begin with two (`##`). A multi-line comment MUST begin with `#(` and end with `)#`, and a multi-line tooltip text MUST begin with `##(` and end with `)##`.

A special keyword, `INHERIT`, MUST inherit the value of its parent if no reference is defined (see [inheritance and references](#Inheritance_and_references) for more information), and is available for all arguments unless otherwise stated. In the event that `INHERIT` is not applicable, the implementation MUST give an error to the designer.

A variable reference MUST be an option for all variables. See [inheritance and references](#Inheritance_and_references) for more information.

All types are explicitly defined by their variables. If an argument does not match the specified type, the implementation or program MUST give an error to the designer.

The implementation or program MAY include an option to "enforce offline content," which prevents the usage of URLs to acquire content.

Indentation MAY either be tab characters OR spaces. There MUST NOT be any mixed whitespace for indentation. Empty lines MUST be ignored.

### Element syntax

An implementation MUST use the following syntax for declaring an element:

```
## Tooltip text
alignment(position) inner-alignment(inner-position) style scale type name appearance
```

All arguments, except for tooltip text, MAY be preceded with their name to either change the argument's position, or to more clearly demonstrate their intended context; otherwise, it MUST be interpreted as the first argument by context.

Tooltip text MAY either appear as a tooltip OR at the bottom of the window. It MAY be omitted. `INHERIT` MUST NOT be used.

The following alignments MUST be available:

- Vertical:
	- top
	- center
	- bottom
- Horizontal:
	- left
	- center
	- right

An alignment MUST be typed as `vertical-horizontal`. The alignment MUST default to `center-center` and MAY be omitted when no position is defined. Additionally, if one part of the alignment is defined, the other side of the alignment MAY be omitted (e.g. `left` translates to `center-left`, and `top` translates to `top-center`).

The position MAY be defined, and MUST be written as `x,y,z` or `x,y`. Any numeric MUST be accepted, except on `z` which must be an integer, and defaults to pixels. Percentages MUST be interpreted as a percentage of the parent's width and height. The position MUST default to `0,0,DYNAMIC`. The Z-order must sort elements on similar layers, from first-to-last as back-to-front respectively. `INHERIT` MUST NOT be used. If excluded, the designer SHOULD NOT leave empty parentheses (`()`).

The inner alignment and position regard the starting offset of the inner contents, including the background of the element. This MUST follow the same alignment and position rules as above.

The style argument is a string that refers to a style provided by the program. This is OPTIONAL to the designer and MUST default to `'default'`, or the parent's style if it is a child element.

The scale MUST be written as `WIDTHxHEIGHT`, where any numeric is accepted and defaults to pixels. This MUST NOT be omitted, unless otherwise stated. `INHERIT` MUST NOT be used.

The type of element is REQUIRED and MUST be any one of the following:

- `window`
- `table`
- `label`
- `textbox`
- `button`
- `image`
- `rect`
- `scroll-rect`

The name MUST be written as a string, MUST be unique to other elements in the file (otherwise an error must be raised), and MUST not contain `$` or `@` as this is reserved (see [inheritance and references](#Inheritance_and_references)). This MAY be omitted by the designer and MAY default to a random UUIDv4. The default naming method MUST be disclosed in the implementation. `INHERIT` MUST NOT be used.

The appearance argument is OPTIONAL to the designer and MUST be any one of the following:

- `visible`: MUST present its contents and allow interaction. This does not override the appearance of its children. This MUST be the default.
- `locked`: MUST present its contents but MUST NOT allow any interaction for itself or any child.
- `invisible`: MUST NOT present any of its contents nor allow any interaction, and MUST NOT capture any input.

### Element type arguments

Some pre-defined types come with additional variables that MAY be required. They MUST appear at least on the next line of the declaration, indented once more than the declaration, and begin with `typearg`. There MUST NOT be any other text between the declaration and the arguments.

The following syntax is invalid:
```
400x200 textbox 'input'
	100%x100% rect style='input_bg'		# Child presence implies the type arguments are empty.
	typearg empty-text='Add some words'	# This produces a syntax error, because it is not the first line.
```

The following element types and their arguments are as follows:

- `window`: A panel that contains various contents. Can be a child restricted to the bounds of its parent. When it or its children are interacted with, the window SHOULD be brought to the front of all other elements on its z-index.
	- `header`: Text that appears on top, as a string. This is REQUIRED to the designer, unless `headerless` is set to `on`, where it MUST default to an empty string.
	- `minimizable`: Boolean for whether or not the window can be minimized (made invisible). This is OPTIONAL to the designer, implementation, and program; and defaults to `off`.
	- `maximizable`: Boolean for whether or not the window can be maximized (expanded to `100%x100%`). This is OPTIONAL to the designer, implementation, and program; and defaults to `off`.
	- `closable`: Boolean for whether or not the window can be closed. This is OPTIONAL to the designer and defaults to `on`.
	- `resizeable`: Boolean for whether or not the window can be resized by the user adjusting its borders. This is OPTIONAL to the designer, implementation, and program; and defaults to `on`.
	- `movable`: Boolean for whether or not the window can be moved by the user. This is OPTIONAL to the designer, implementation, and program; and defaults to `on`.
	- `headerless`: Boolean for if this window has no header. This is OPTIONAL to the designer, and defaults to `off`.
	- `borderless`: Boolean for if this window is borderless. This is OPTIONAL to the designer, implementation, and program; and defaults to `off`.
- `table`: An element that contains items. Scale MAY be omitted by the designer, and it MUST default to `100%xDYNAMIC`.
	- `rows-columns`: Number of `ROWSxCOLUMNS`, integers only. This is REQUIRED to the designer, can use `DYNAMIC` only on one axis to allow for more elements.
	- `borderless`: Boolean for if this table is borderless. This is OPTIONAL to the designer, implementation and program; and defaults to `off`.
- `label`: An element that contains text. Scale MAY be omitted by the designer, and it MUST default to `DYNAMICxDYNAMIC`.
	- `text`: Text of the label, as a string. This is OPTIONAL to the designer and defaults to an empty string.
- `textbox`: An input field containing text. Scale MAY be omitted by the designer, and it MUST default to `100%xDYNAMIC`.
	- `empty-text`: Text that appears when the box is empty, as a string. This is OPTIONAL to the designer and defaults to an empty string.
	- `text`: The initial text that appears in the box, as a string. This is OPTIONAL to the designer and defaults to an empty string.
	- `max-lines`: The maximum number of lines a user may add, only as an integer. This is OPTIONAL to the designer and defaults to `1`. `DYNAMIC` SHOULD allow infinite lines, and MUST present a scroll bar on any axis if the input expands beyond the given scale.
- `button`: An interactable button that fires an event. Scale MAY be omitted by the designer, and it MUST default to `DYNAMICxDYNAMIC`.
	- `event`: The event to fire when interacted with, as a string. This is OPTIONAL to the designer and defaults to an empty string. See [event bus](#Event_bus) for more information.
- `image`: An image to be presented to the user. Image acquisition is OPTIONAL for an implementation, but this SHOULD be documented by the implementation. If an error is encountered when acquiring an image, the program or implementation MUST provide an error to the designer.
	- `path`: The location of the image, which may either be on the disk, or an HTTP(S) URL. This is REQUIRED to the designer, but if a URL is present and offline content is enforced, then the implementation or program MUST use `offline-path` or raise an error to the designer.
	- `offline-path`: The location of the image, only on the disk. This is OPTIONAL and defaults to an empty string, used as fallback for offline content.
- `scroll-rect`: A region of a given size.
	- `inner-scale`: The actual scale of the contents, as `WIDTHxHEIGHT`. This is OPTIONAL, and defaults to `DYNAMICxDYNAMIC` (the inner scale will fit around the size of its contents). Scroll bars MUST be placed accordingly if the inner scale exceeds the scale of this element; if the inner scale is smaller, the scroll bars MUST be disabled.

### Element parenting

An element MAY be declared as a child of another element, which MUST be placed below another declaration, and given one additional indentation.

```
1024x768 window 'main_window'		# Parent
	typearg header='Window'			# See "element type arguments" for more information
	450x150 table 'main_content'	# Child
```

### Event bus

The event bus uses strings to define specific events, and MAY be called and defined by the program. The implementation MAY raise a warning if an event is called but has no listeners.

An event MAY contain arguments through the following syntax:

```
program_event(arg1, arg2)
```

Whitespace between any name or syntax is OPTIONAL. If there are no arguments, the parentheses are OPTIONAL.

References are applicable as arguments. See [inheritance and references](#Inheritance_and_references) for more information.

### Inheritance and references

When using `INHERIT`, a reference MAY be given. This is identified by the special character `$`. Inheritance uses the following syntax:

```
INHERIT:$reference
```

The colon MUST be omitted if there is no reference defined. The reference MUST default to the element's parent.

A variable MAY be referenced, and it MUST be with the following syntax:

```
@variable-name
@typearg.variable-name
$reference:@variable-name
```

`INHERIT` MUST NOT be used to prefix any variable references.

An implementation MUST resolve types for references. If an invalid type is given, an error MUST be given to the designer.

### Guidance

All parser errors are intended to be visible to the designer. Therefore, a program SHOULD forward errors and warnings from the implementation if they pertain to the designer.

For any SHOULD NOT, a warning SHOULD be given to its intended recipient.