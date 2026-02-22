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

A text file for the language starts with `!GGUIML` and a newline afterward; and the file extension MAY be `.ggui` or `.guil`.

The topmost element is the container; this is the window of the application itself, or display, or module file depending on the program and its environment. It MAY have no parent, but the implementation or program SHOULD document if it does have one.

If a feature or element is unavailable in a given API, it MUST be discarded and SHOULD give a warning to the designer, but it SHOULD NOT stop the program. Custom elements MUST NOT be present. The strict palette of elements in the language ensures that one GUI file is compatible across multiple implementations and programs.

A numeric is either:
- Integers which contain no suffix, also referred to as "pixels".
- Percentages which MUST be suffixed with `%`, the number MAY include a decimal point.
- Decimals, which MUST include a decimal point.
- `DYNAMIC`, a special keyword used to indicate that this item is flexible.

A point is either a numeric of `(x,y)` or `(x,y,z)`. The allowed numerics is dependent on the variable.

A string is either surrounded by single-quotes (`'`) which refer to un-parsed text, or double-quotes (`"`) which refer to parsed text. The escape character `\` conforms to ISO C escape sequences, and escapes special characters in this language; it MUST be interpreted as such, only if it is present in double-quotes. Strings can be accessed as an associative array with line index as the key, and all text on each line as the values (which MUST be a non-indexable string for the designer). Empty lines MAY be omitted.

A boolean MUST either be `on` or `off`, representing `true` and `false` in the context of a UI. The intent is to better present features rather than states.

A single-line comment begins with a single hashtag/pound symbol (`#`), whereas tooltip text begins with two (`##`). A multi-line comment begins with `#(` and ends with `)#`, and a multi-line tooltip text begins with `##(` and ends with `)##`.

An associative array has a series of entries prefixed by an additional indentation, and a hyphen (`-`). They begin with a key, followed by a colon (`:`), and then a value. The following syntax is valid:

```
items=
	- 'cans': 5
	- 'cups': 12
	- 'bottles': 9
```

A special keyword, `INHERIT`, MUST inherit the value of its parent if no reference is defined (see [inheritance and references](#Inheritance_and_references) for more information), and is available for all arguments unless otherwise stated. In the event that `INHERIT` is not applicable, the implementation MUST give an error.

A variable reference MUST be an option for all variables. See [inheritance and references](#Inheritance_and_references) for more information.

Special characters MUST NOT be used in any names that elements in the language may reference, even when escaped. These are `$` and `@` ([inheritance and references](#Inheritance_and_references)), square brackets (`[` and `]`), curly brackets (`{` and `}`), quotes (`'` and `"`), backslash (`\`), punctuation marks (`.`, `?`, and `!`, from [Inheritance and references](#Inheritance_and_references)), space (` `), and colon (`:`).

All types are explicitly defined by their variables. If an argument does not match the specified type, the implementation or program MUST give an error to the designer.

The UI has the capacity to be split into modules. See [modular elements](#Modular_elements) for more information.

The implementation or program MAY include an option to "enforce offline content," which prevents the usage of URLs to acquire content.

Indentation MAY either be tab characters OR spaces. There MUST NOT be any mixed whitespace for indentation. Empty lines MUST be ignored.

### Element syntax

An implementation uses the following syntax for declaring an element:

```
## Tooltip text
alignment(position) inner-alignment(inner-position)[inner-padding] scale style type name appearance
```

All arguments, except for tooltip text, MAY be preceded with their name to either change the argument's position, or to more clearly demonstrate their intended context; otherwise, it MUST be interpreted as the first argument by context.

Tooltip text MAY either appear as a tooltip OR at the bottom of the window. It is OPTIONAL to the designer and program. `INHERIT` MUST NOT be used.

The following alignments are available:

- Vertical:
	- top
	- center
	- bottom
- Horizontal:
	- left
	- center
	- right

An alignment is typed as `vertical-horizontal`. The alignment defaults to `center-center` and can be omitted when no position is defined. Additionally, if one part of the alignment is defined, the other side of the alignment can be omitted (e.g. `left` translates to `center-left`, and `top` translates to `top-center`).

The offset position can be defined, written as a point. Any numeric is accepted, except on `z` which MUST be an integer. Percentages are interpreted as a percentage of the given inner boundary. The position defaults to `(0,0,DYNAMIC)`. The Z-order must sort elements on similar layers, from first-to-last as back-to-front respectively. `INHERIT` is invalid. If excluded, the designer SHOULD NOT leave empty parentheses (`()`).

The inner alignment and position regard the starting offset of the inner contents. This follows the same alignment and position rules as above. Inner padding is OPTIONAL to the designer, and is represented as `[left, top, right, down]`; all numerics except `DYNAMIC` are valid, and the default is `[0,0,0,0]`. When inferring this argument's context, the original alignment MUST be defined.

The scale is written as `WIDTHxHEIGHT`, where any numeric is accepted and defaults to pixels. This is REQUIRED, unless otherwise stated. `INHERIT` is invalid.

The style argument is a string that refers to a style provided by the program. This is OPTIONAL to the designer and defaults to `'default'` for the entire container, or `INHERIT` if it is a child element.

The type of element is REQUIRED and can be any one of the following:

- `window` (OPTIONAL to the program and may be replaced with a `scroll-rect`)
- `table`
- `label`
- `textbox`
- `button`
- `image`
- `rect`
- `scroll-rect`
- `graph`

The name is written as a string, MUST NOT contain special characters (see [interpretation](#Interpretation)) or uppercase letters, and MUST be unique to other elements in the file (otherwise an error must be raised). This is OPTIONAL to the designer and MAY default to a random UUIDv4. The default naming method SHOULD be disclosed in the implementation, but it SHOULD NOT be used by the designer. `INHERIT` is invalid.

The appearance argument is OPTIONAL to the designer and can be any one of the following:

- `visible`: Presents its contents and allows interaction. This MUST NOT override the appearance of its children. This is the default.
- `locked`: Presents its contents but MUST NOT allow any interaction for itself or any child.
- `invisible`: Does not present any of its contents nor allow any interaction, and MUST NOT capture any input.

### Element type arguments

Some pre-defined types come with additional variables that could be required. On the next line at least, with one additional indentation, the keyword `TYPEARG` is used to provide type arguments. There MUST NOT be any other text between the declaration and the arguments.

Arguments can be separated with a newline and indented, to indicate they are still a part of `TYPEARG`. `INHERIT` is invalid for all type arguments.

The following syntax is invalid:
```
400x200 textbox 'input'
	100%x100% rect style='input_bg'		# Child presence implies the type arguments are empty.
	TYPEARG empty-text='Add some words'	# This produces a syntax error, because it is not the first line.
```

The following element types and their arguments are as follows:

- `window`: A panel that contains various contents. Can be a child restricted to the bounds of its parent. When it or its children are interacted with, the window SHOULD be brought to the front of all other elements on its z-index. The program SHOULD provide scroll bars when the content is beyond the window boundaries.
	- `header`: Text that appears on top, as a string. This is REQUIRED to the designer, unless `headerless` is set to `on`, where it defaults to an empty string.
	- `minimizable`: Boolean for whether or not the window can be minimized (made invisible). This is OPTIONAL to the designer, implementation, and program; and defaults to `off`.
	- `maximizable`: Boolean for whether or not the window can be maximized (expanded to `100%x100%`). This is OPTIONAL to the designer, implementation, and program; and defaults to `off`.
	- `closable`: Boolean for whether or not the window can be closed. This is OPTIONAL to the designer and defaults to `on`.
	- `resizeable`: Boolean for whether or not the window can be resized by the user adjusting its borders. This is OPTIONAL to the designer, implementation, and program; and defaults to `on`.
	- `movable`: Boolean for whether or not the window can be moved by the user. This is OPTIONAL to the designer, implementation, and program; and defaults to `on`.
	- `headerless`: Boolean for if this window has no header. This is OPTIONAL to the designer, and defaults to `off`.
	- `borderless`: Boolean for if this window is borderless. This is OPTIONAL to the designer, implementation, and program; and defaults to `off`.
- `table`: An element that contains items. Scale can be omitted by the designer, and it defaults to `DYNAMICxDYNAMIC`.
	- `rows-columns`: Number of `ROWSxCOLUMNS`, integers only. This is REQUIRED to the designer, can use `DYNAMIC` only on one axis to allow for more elements. A warning MUST be given by the implementation if there are more children than the table's size allows, and additional children SHOULD be discarded.
	- `borderless`: Boolean for if this table is borderless. This is OPTIONAL to the designer, implementation and program; and defaults to `off`.
- `label`: An element that contains text. Scale can be omitted by the designer, and it defaults to `DYNAMICxDYNAMIC`.
	- `text`: Text of the label, as a string. This is OPTIONAL to the designer and defaults to an empty string.
- `textbox`: An input field containing text. Scale can be omitted by the designer, and it defaults to `100%xDYNAMIC`.
	- `empty-text`: Text that appears when the box is empty, as a string. This is OPTIONAL to the designer and defaults to an empty string.
	- `text`: The initial text that appears in the box, as a string. This is OPTIONAL to the designer and defaults to an empty string.
	- `max-lines`: The maximum number of lines a user may add, only as an integer. This is OPTIONAL to the designer and defaults to `1`. `DYNAMIC` SHOULD allow indefinite lines, and MUST present a scroll bar on any axis if the input expands beyond the given scale.
- `button`: An interactable button that fires an event. Scale can be omitted by the designer, and it defaults to `DYNAMICxDYNAMIC`.
	- `text`: Text to be emplaced with the button.
	- `event`: The event to fire when interacted with, as a string. This is OPTIONAL to the designer and defaults to an empty string. See [event bus](#Event_bus) for more information.
- `image`: An image to be presented to the user. Image acquisition is OPTIONAL for an implementation, but this SHOULD be documented by the implementation. If an error is encountered when acquiring an image, the program or implementation MUST provide an error to the user.
	- `path`: The location of the image, which may either be on the disk, or an HTTP(S) URL. This is REQUIRED to the designer, but if a URL is present and offline content is enforced, then the implementation or program MUST use `offline-path` if it exists.
	- `offline-path`: The location of the image, only on the disk. This is OPTIONAL and defaults to an empty string, used as fallback for offline content.
- `scroll-rect`: A region of a given size.
	- `inner-scale`: The actual scale of the contents, as `WIDTHxHEIGHT`. This is OPTIONAL, and defaults to `DYNAMICxDYNAMIC` (the inner scale will fit around the size of its contents). Scroll bars SHOULD be placed accordingly by the program if the inner scale exceeds the scale of this element; if the inner scale is smaller, the scroll bars SHOULD be disabled.
- `graph`: A display with given data points.
	- `data`: An associative array of string keys, and numeric or point values. Numerics, and numerics of points, can be any number except `DYNAMIC`. The designer MAY create a nested associative array, with names for each plot.

These arguments are intended to be explicit, instead of sequential. An implementation SHOULD NOT support context inferencing on type arguments.

### Element parenting

An element can be declared as a child of another element, which MUST be placed below another declaration, and given one additional indentation. The following syntax is valid:

```
1024x768 window 'main_window'		# Parent
	TYPEARG header='Window'			# See "element type arguments" for more information
	450x150 table 'main_content'	# Child
```

### Event bus

The event bus uses strings to define specific events, which MAY be called and defined by the program. If the string is empty, there SHOULD NOT be an event called in the event bus. The implementation MAY raise a warning if an event is called but has no listeners.

An event can contain arguments through the following syntax:

```
program_event(arg1, arg2)
```

Whitespace between any name or syntax is OPTIONAL. If there are no arguments, the parentheses are OPTIONAL.

References are applicable as arguments. See [inheritance and references](#Inheritance_and_references) for more information.

### Inheritance and references

When using `INHERIT`, an element reference can be given by the designer. This is identified by the special character `$`. Inheritance uses the following syntax:

```
INHERIT:$element
```

The colon MUST be omitted if there is no reference defined, and defaults to the element's parent.

A variable can be referenced with the following syntax:

```
@variable-name
@TYPEARG.variable-name
$element:@variable-name
```

`INHERIT` is invalid to the declaration of any variable references.

When referencing an item in an associative array, it can be retrieved with any of the following syntax:

```
@TYPEARG.array.key-name
@TYPEARG.array[key-name]
```

All of these entries will resolve to the variable `TYPEARG`, its sub-variable `array`, and an entry with the key `key-name`, to reference the value at this location.

A variable or element reference can be expanded multiple times with curly brackets (`{` and `}`). The following example will first resolve `@variable`, then the outer reference:

```
@TYPEARG.{@variable}
```

Variables will always resolve to strings in this manner, until the final reference has been acquired. A limit to multi-expansion is RECOMMENDED to be defined by the implementation or program, and SHOULD be documented.

All variable references are possible to resolve in parsed strings. An element reference will resolve into its name in a parsed string. A variable may be further encased in curly brackets to better indicate inline parsing:

```
# var-name is "cat"
"@variable.{@var-name}.name"	# Resolves var-name, then variable.cat.name
"{@variable.{@var-name}}.name"	# Resolves var-name, then variable.cat, and does not resolve further
```

If the variable has the possibility of resolving to an empty string, the following conditions are available:

```
@TYPEARG.text?{.}{@line}
@TYPEARG.!{text}{@variable}
```

The `?{}` provides that anything inside the braces is only present if the next variable can be resolved. `!{}` provides that anything inside the braces is only present if the next variable cannot be resolved. If the provided conditional text contains a variable, it MUST also be expanded.

If an invalid reference is used by the designer, a warning SHOULD be given.

An implementation MUST resolve types for references. If an invalid type is given, an error MUST be given to the designer.

### Modular elements

A designer can split their UI into multiple reusable elements. This is defined with the special `MODULE` keyword, and can be defined as a child of any element or in its own separate file. The syntax is as follows:

```
MODULE module-name argument-names
```

The module name is a string, MUST be unique to other modules in scope, and MUST NOT contain special characters or uppercase letters. Argument names are space-separated, MAY be suffixed by `?` to make optional (where it becomes a default value if left blank), and can be accessed through reference syntax. Argument types are inferred by where they are placed, and will find a matching variable name if placed alone; otherwise, it is assumed to be the first-available argument when under inferred context. See [inheritance and references](#Inheritance_and_references) for more information.

Modules can be placed as children to a `NAMESPACE` keyword with the following syntax:

```
NAMESPACE namespace-name
```

The namespace name is a string, MUST be unique to other modules in scope, and MUST NOT contain special characters or uppercase letters.

Namespaces can be children of other namespaces, but MUST NOT be children of modules.

A module MUST NOT deploy any elements without an `IMPORT` declaration. To access a module in the same file, the `IMPORT` keyword is used with the following syntax:

```
IMPORT $module-name
IMPORT $namespace:$module-name
IMPORT $nested.namespace:$module-name
```

To import a module from another file, the `IMPORT` keyword is used with the following syntax:

```
IMPORT $disk/path/to/file.ggui:$module-name	# Explicit inclusion
IMPORT $disk/path/to/file.ggui				# Implicit inclusion; modules are treated as part of this scope
```

This is only valid if the module is at the root of the file. Module definitions, namespaces, and imports are scope-bound to their parent element. This means that this would be invalid:

```
200x100 textbox 'input'
	MODULE 'custom-background' style
		100%x100% @style rect
IMPORT $custom-background		# Error caused by module not in scope
```

However, this is valid, because modules and namespaces are not elements:

```
1024x768 window 'main_window'
	NAMESPACE 'buttons'
		MODULE 'large-button' name text event
			400x200 button @name style='large_button'
				TYPEARG @text @event
	IMPORT $buttons:$large-button 'play_button' 'PLAY' 'launch_game'
```

When importing, any module arguments are to be placed after the module reference. Module references are special, in that they cannot be referenced by other elements or variables; only imports can refer to them. Likewise, references to variables and named elements cannot be accessed by `IMPORT`.

References and inheritance in modules MUST be resolved before they are implemented. If post-resolution is desired, the `TEMPLATE` keyword is available with the following syntax:

```
TEMPLATE template-name argument-names
```

This behaves similarly to `MODULE` and may be imported the same way. The only difference is that references and inheritance are resolved after the template has been inserted.

### Guidance

All parser errors are intended to be visible to the designer. Therefore, a program SHOULD forward errors and warnings from the implementation if they pertain to the designer.

Because this is a windowed system, a program MAY provide the user with a bar that contains minimized and present windows. A window SHOULD NOT be a required container for the designer.

For any SHOULD NOT, a warning SHOULD be given to its intended recipient.