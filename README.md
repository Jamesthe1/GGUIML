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

The language is case-sensitive except for element names. If the implementation exposes an API, element names are RECOMMENDED to be lowercase. Uppercase file paths SHOULD throw a warning to the designer, as this may result in undesired behavior on case-sensitive file systems.

The topmost element is the container; this is the window of the application itself, or display, or module file depending on the program and its environment. It MAY have no parent, but the implementation or program SHOULD document if it does have one.

If a feature or element is unavailable in a given API, it MUST be discarded and SHOULD give a warning to the designer, but it SHOULD NOT stop the program. Custom elements MUST NOT be present. The strict palette of elements in the language ensures that one GUI file is compatible across multiple implementations and programs.

A numeric is either:

- Integers which contain no suffix, also referred to as "pixels."
- Percentages which are suffixed with `%`, and the designer can include a decimal point.
- Decimals, which include a decimal point.
- `DYNAMIC`, a special keyword used to indicate that this item is flexible.

A scale is two numerics separated by a lowercase `x`, no whitespace. A special keyword, `SQUARE`, may substitute only either the first or second parameter to make the element have equal height and width; it may be suffixed with an asterisk (`*`) and then an integer.

A point is either a numeric of `(x,y)` or `(x,y,z)`. The allowed numerics is dependent on the variable. Whitespace is allowed.

A rect is an array of four numerics, written as `[top, right, down, left]`.

A string is either surrounded by single-quotes (`'`) which refer to un-parsed text, or double-quotes (`"`) which refer to parsed text. The escape character `\` conforms to ISO C escape sequences (except for carriage return, which is excluded), and also escapes special characters in this language only if it is present in double-quotes. Newlines are permitted, indentation is subtracted by element depth. Strings are indexable to the designer by line count via the bracket operator `[n]`, but the string MUST NOT be indexable further. If empty lines are omitted by the implementation, it MUST preserve each line's index.

A boolean MUST either be `on` or `off`, representing `true` and `false` in the context of a UI. The intent is to better present features rather than states.

A single-line comment begins with a single hashtag/pound symbol (`#`), whereas hint text begins with two (`##`). A multi-line comment begins with `#(` and ends with `)#`, and a multi-line hint text begins with `##(` and ends with `)##`. Single-line comments and hints may also be chained together to form a multi-line version, but empty lines without any single-line indicator MUST be counted as a break.

An associative array has a series of entries prefixed by an additional indentation, and a hyphen (`-`). They begin with a key, followed by a colon (`:`), and then a value. The following syntax is valid:

```
items=
	- 'cans': 5
	- 'cups': 12
	- 'bottles': 9
```

A special keyword, `INHERIT`, MUST inherit the value of its parent if no reference is defined (see [inheritance and references](#inheritance-and-references) for more information), and is available for all arguments unless otherwise stated. In the event that `INHERIT` is not applicable, the implementation MUST give an error.

A variable reference MUST be an option for all variables. See [inheritance and references](#inheritance-and-references) for more information.

Special characters MUST NOT be used in any names that elements in the language may reference, even when escaped. These are `$` and `@` ([inheritance and references](#inheritance-and-references)), square brackets (`[` and `]`), curly brackets (`{` and `}`), quotes (`'` and `"`), backslash (`\`), punctuation marks (`.`, `?`, and `!`, from [Inheritance and references](#inheritance-and-references); `:` and `;`), asterisk (`*`), and space (` `).

All types are explicitly defined by their variables. If an argument does not match the specified type, the implementation or program MUST give an error to the designer.

The UI has the capacity to be split into modules. See [modular elements](#modular-elements) for more information.

The implementation or program MAY include an option to "enforce offline content," which prevents the usage of URLs to acquire content.

Indentation MAY either be tab characters OR spaces. There MUST NOT be any mixed whitespace for indentation. Empty lines are ignored.

For any argument with a default, it is implied OPTIONAL for the designer, unless otherwise stated. Any argument that is excluded by the implementation, if permitted by this document, SHOULD be documented.

Most text that is displayed supports a very basic form of Markdown: `**` for bold, `*` or `_` for emphasized/italic text, and `__` for underline. All characters presented must surround text to take effect.

### Element syntax

An implementation uses the following syntax for declaring an element:

```
## Hint text
alignment[margin](position) inner-alignment[padding](inner-position) scale order style appearance type name
```

All arguments, except for hint text, margin/padding, and positions, can be preceded with their name to either change the argument's position, or to more clearly demonstrate their intended context; otherwise, it MUST be interpreted as the first argument by context. Double assignment of an argument MUST present a warning.

Hint text MAY either appear as a tooltip OR at the bottom of the window. Indentation MUST match the succeeding element. The inner text is parsed as markdown; the depth of the supported markdown is up to the implementation, and SHOULD be documented. The program may choose not to support this, and this is not required to the designer. Comments MUST NOT appear within hint text. `INHERIT` is invalid.

The following alignments are available:

- Vertical:
	- top
	- center
	- bottom
- Horizontal:
	- left
	- center
	- right

An alignment is typed as `vertical-horizontal`. The alignment defaults to `INHERIT` (or `center-center` if the topmost element) and can be omitted when no position or margin/padding is defined. Additionally, if one part of the alignment is defined, the other side of the alignment can be omitted (e.g. `left` translates to `center-left`, and `top` translates to `top-center`).

The margin describes the spacing away from surrounding elements, and is represented as a rect. All numerics except `DYNAMIC` are valid, and the default is `[0,0,0,0]`.

The offset position can be defined, written as a point. Any numeric is accepted, except on `z` which MUST be an integer. Percentages are interpreted as a percentage of the given inner boundary. The position defaults to `(0,0,DYNAMIC)`. The Z-order must sort elements on similar layers, from first-to-last as back-to-front respectively. `INHERIT` is invalid. If excluded, the designer SHOULD NOT leave empty parentheses (`()`).

The inner alignment regards the alignment that will be inherited, the padding describes the inner spacing away from the boundaries and is typed similarly to margin, and the position describes the base offset that other elements will start from. These follow the same definition rules as above. When inferring this argument's context, the original alignment MUST be defined.

The scale is written as a scale, where any numeric is accepted and defaults to pixels. This is REQUIRED, unless otherwise stated. `INHERIT` is invalid.

The order argument is the sort order of the element's contents, and can be any one of the following, defaulting to `horizontal` unless otherwise specified:

- `vertical`: Organizes its contents vertically first until it encounters an element or endpoint, where it will then return to the first element and extend once horizontally. This will continue up to a horizontal endpoint or reaching an element horizontally.
- `horizontal`: Organizes its contents horizontally first until it encounters an element or endpoint, where it will then return to the first element and extend once vertically. This will continue up to a vertical endpoint or reaching an element vertically.

The style argument is a string that refers to one or more styles provided by the program; they are separated by a comma (within the string), any whitespace is allowed except newlines, and special characters are not allowed for a valid style name even when escaped (see [Interpretation](#interpretation)). This defaults to `'default'` for the entire container, or `INHERIT` if it is a child element.

The appearance argument can be any one of the following, defaulting to `visible`:

- `visible`: Presents its contents/children and allows interaction. This MUST NOT override the appearance of its children. This is the default.
- `locked`: Presents its contents/children but MUST NOT allow any interaction for itself or any child.
- `invisible`: Does not present any of its contents/children nor allow any interaction, and MUST NOT capture any input.

The type of element is REQUIRED and can be any one of the following:

- `window` (OPTIONAL to the program and may be replaced with a `rect` internally; if done so, arguments relating to the window SHOULD be ignored)
- `table`
- `label`
- `textbox`
- `button`
- `image`
- `rect`
- `graph`
- `list`
- `break`
- `progress`

The name is written as a string, MUST NOT contain special characters (see [interpretation](#interpretation)) or uppercase letters, and MUST be unique to other elements in the file (otherwise an error needs to be raised). This MAY default to a random UUIDv4, but is overall optional to the designer. The default naming method SHOULD be disclosed in the implementation, but it SHOULD NOT be used by the designer. `INHERIT` is invalid.

### Element type arguments

Some pre-defined types come with additional variables that could be required. On the next line at least, with one additional indentation, the keyword `TYPEARG` is used to provide type arguments. There MUST NOT be any other text between the declaration and the arguments.

Arguments can be separated with a newline and indented, to indicate they are still a part of `TYPEARG`. `INHERIT` is invalid for all type arguments.

The following syntax is invalid:
```
400x200 button 'play_button'
	100x100 button style='play_icon'		# Child presence implies the type arguments are empty.
	TYPEARG event='start_music'	# This produces a syntax error, because it is not the first line.
```

The following element types and their arguments are as follows:

- `window`: A panel that contains various contents. If a child of another element, it is restricted to its parent's boundaries. When it or its children are interacted with, the window SHOULD be brought to the front of all other elements on its z-index. This element extends from `rect` and includes its type arguments as well.
	- `header`: Text that appears on top, as a string. This is REQUIRED to the designer, unless `headerless` is set to `on`, where it defaults to an empty string. `INHERIT` is invalid.
	- `minimizable`: Boolean for whether or not the window can be minimized (made invisible). This defaults to `off`.
	- `maximizable`: Boolean for whether or not the window can be maximized (expanded to `100%x100%`). This defaults to `off`.
	- `closable`: Boolean for whether or not the window can be closed. This defaults to `on`.
	- `resizeable`: Boolean for whether or not the window can be resized by the user adjusting its borders. This defaults to `on`, and the argument MAY be ignored by the program.
	- `movable`: Boolean for whether or not the window can be moved by the user. This defaults to `on`, and the argument MAY be ignored by the program.
	- `headerless`: Boolean for if this window has no header. This defaults to `off`.
	- `borderless`: Boolean for if this window is borderless. This defaults to `off`, and the argument MAY be ignored by the program.
- `table`: An element that contains items. Scale can be omitted by the designer, and it defaults to `DYNAMICxDYNAMIC`. Cells are filled in by the `order` of this element.
	- `rows-columns`: A scale of `ROWSxCOLUMNS`, integers only. This is REQUIRED to the designer, and `DYNAMIC` can be used on any axis; a `break` element is suggested if both axes are dynamic. A warning SHOULD be given by the implementation to the designer if there are more children than the table's size allows, and additional children MUST be discarded.
	- `borderless`: Boolean for if this table is borderless. This defaults to `off`, and the argument MAY be ignored by the program.
- `label`: An element that contains text. Scale can be omitted by the designer, and it defaults to `DYNAMICxDYNAMIC`.
	- `text`: Text of the label, as a string. This defaults to an empty string.
	- `font-size`: The size of the font. Integers and decimals are treated as font points, and defaults to the style's font size. The actual font MUST be defined in the style.
- `textbox`: An input field containing text. Scale can be omitted by the designer, and it defaults to `100%xDYNAMIC`. This element extends from `label` and includes its type arguments as well.
	- `empty-text`: Text that appears when the box is empty, as a string. This defaults to an empty string.
	- `max-lines`: The maximum number of lines a user may add, only as an integer. This defaults to `1`. `DYNAMIC` SHOULD allow indefinite lines, and MUST present a scroll bar on any axis if the input expands beyond the given scale.
- `button`: An interactable button that fires an event. Scale can be omitted by the designer, and it defaults to `DYNAMICxDYNAMIC`.
	- `event`: The event to fire when interacted with, as a string. This defaults to an empty string. See [event bus](#Event_bus) for more information.
- `image`: An image to be presented to the user. Image acquisition is OPTIONAL for an implementation, but this SHOULD be documented by the implementation. If an error is encountered when acquiring an image, or images are not supported by the program, the program or implementation SHOULD provide an error to the user. A designer SHOULD NOT use this as a full background to any element, as backgrounds should be defined by the program's style.
	- `path`: The location of the image, which may either be on the disk, or an HTTP(S) URL. This is REQUIRED to the designer, but if a URL is present and offline content is enforced, then the implementation or program MUST use `offline-path` if it exists. Newlines are invalid.
	- `offline-path`: The location of the image, only on the disk. This defaults to an empty string, used as fallback for offline content. Newlines are invalid.
	- `alt-text`: Text for screen readers or when the image fails to load. This is REQUIRED for the designer.
- `rect`: A region of a given size. Scale defaults to `DYNAMICxDYNAMIC` (fits around the size of its contents). Overflowing contents MUST be cropped by the program.
	- `inner-scale`: The actual scale of the contents, as `WIDTHxHEIGHT`. Defaults to `DYNAMICxDYNAMIC` (the inner scale will fit around the size of its contents). If `DYNAMIC` is specified on any axis of the rect's scale, `DYNAMIC` must also appear in the same axis on the inner scale; the inner scale's dynamic axis MAY be omitted along with the `x`, and the argument is interpreted as a single numeric, but a blank argument is not allowed.
	- `scrollable`: Boolean whether or not scroll bars are to be placed accordingly by the program, if the inner scale exceeds the scale of this element; if the inner scale is smaller, the scroll bars SHOULD be disabled. Defaults to `off`.
- `graph`: A display with given data points, displayed as determined by the program style.
	- `data`: An associative array of string keys (no special characters allowed), and numeric or point values. Numerics, and numerics of points, can be any number except `DYNAMIC`. The designer MAY create a nested associative array, with names for each plot.
- `list`: A list enumerating its child elements. Can be nested. Sort order defaults to `vertical`, and `horizontal` elements are RECOMMENDED to be aligned akin to a table.
	- `mode`: How the list is presented and interaction is determined. The following options are available and defaults to `INHERIT`:
		- `ordered`: Numbers, letters, roman numerals, any as specified by the program and style.
		- `unordered`: Bullet points, squares, any as specified by the program and style.
		- `radio`: Multiple options, one selection. If a child radio is selected, it will also make its parent selected.
		- `checkbox`: Multiple options, multiple selections. If a child checkbox is selected, it will also make its parent selected.
	- `start-selected`: A boolean of whether or not this option starts selected. Only available for `radio` and `checkbox` lists, and the implementation SHOULD present a warning to the designer if multiple `radio` options have `start-selected` to `on`. A warning SHOULD be presented to the designer if this option is not applicable to the given mode.
- `break`: An intentional break in the ordering of elements. This forces sorted items after this element to move across to the next available row/column. It MAY be presented with a horizontal rule by the program, or an explicit element representing the rule can be added by the designer; each approach is valid depending on one's use case.
- `progress`: A progress bar that fills in a direction given by the program style.
	- `value`: The value, as a percent.

These arguments are intended to be explicit, instead of sequential. An implementation MUST NOT support context inferencing on type arguments.

### Element parenting

An element can be declared as a child of another element, which MUST be placed below another declaration, and given one additional indentation. The following syntax is valid:

```
1024x768 window 'main_window'		# Parent
	TYPEARG header='Window'			# See "element type arguments" for more information
	450x150 table 'main_content'	# Child
```

Depending on the element, some children may be organized differently or moved into individual containers. If multiple elements are intended to occupy one region, a `rect` may be used as a parent containing all of them.

### Event bus

The event bus uses strings to define specific events, which MAY be called and defined by the program. If the string is empty, there SHOULD NOT be an event called in the event bus. The implementation MAY raise a warning if an event is called but has no listeners.

An event can contain arguments through the following syntax:

```
program_event(arg1, arg2)
```

Whitespace, except for newlines, between any name or syntax is OPTIONAL. If there are no arguments, the parentheses MAY be excluded.

References are applicable as arguments. See [inheritance and references](#inheritance-and-references) for more information.

### Inheritance and references

When using `INHERIT`, an element reference can be given by the designer. This is identified by the special character `$`. Inheritance uses the following syntax:

```
INHERIT:$element
```

The colon MUST be omitted if there is no reference defined, and defaults to the element's parent (or next-applicable parent up the hierarchy).

A variable can be referenced with the following syntax:

```
@variable-name				# Relative reference to parent element's arguments (or module arguments, which take priority over relative references)
@TYPEARG.variable-name		# Relative reference to parent element's type argument
$element:@variable-name		# Explicit reference to an element's argument
```

`INHERIT` is invalid to the declaration of any variable references. Variable references, when placed as element arguments, are resolved only as a named argument, and cannot be inferred; if the variable reference's name matches an argument name, this is the only case where prefixing with the name can be excluded, as it will assume the it applies to the argument of the same name.

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

Variables will always resolve to strings in this manner, until the final reference has been acquired. Attempting to resolve a variable or element name with a special character is not allowed. A limit to multi-expansion is RECOMMENDED to be defined by the implementation or program, and SHOULD be documented.

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

Any cyclic references MUST throw an error. A cyclic reference is any reference that eventually loops onto itself, creating an infinite loop of resolution. The following would be an example of a cyclic reference:

```
label 'label-one'
	TYPEARG text=$label-two:@TYPEARG.text
label 'label-two'
	TYPEARG text=$label-one:@TYPEARG.text
```

An implementation MUST resolve types for references. If an invalid type is given, an error MUST be given to the designer.

### Modular elements

A designer can split their UI into multiple reusable elements. This is defined with the special `MODULE` keyword, and can be defined as a child of any element or in its own separate file. The syntax is as follows:

```
MODULE module-name argument-names
```

The module name is a string, MUST be unique to other modules in scope, and MUST NOT contain special characters or uppercase letters. Argument names are space-separated, can be suffixed by `?` to make optional (where it becomes a default value if left blank), and can be accessed through reference syntax. Argument types are inferred by where they are placed, and will find a matching variable name if placed alone. Arguments from modules override relative references. See [inheritance and references](#inheritance-and-references) for more information.

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

Supplied arguments are treated the same way as they would be on an element; arguments are implied to be sequential, variable references that match names will automatically fill that argument, and named arguments are allowed.

To import a module from another file, the `IMPORT` keyword is used with the following syntax (extension is REQUIRED):

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

This behaves similarly to `MODULE` and may be imported the same way. The only difference is that relative variable references and inheritance are resolved after the template has been inserted.

### Guidance

All parser errors are intended to be visible to the designer. Therefore, a program SHOULD forward errors and warnings from the implementation if they pertain to the designer.

Because this is a windowed system, a program MAY provide the user with a bar that contains minimized and present windows. A window SHOULD NOT be a required container for the designer.

For any SHOULD NOT, a warning SHOULD be given to its intended recipient.
