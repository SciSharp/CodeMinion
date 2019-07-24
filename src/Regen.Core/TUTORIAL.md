# Getting Started
Regen at its core is a templating engine that uses a mix of C# and Python-like syntax and is entirely written in C#.<br>
Its purpose is to replace T4 Templating with intuitive and fast in-code (not in a seperate file) scripting/templating.<br>
Having your template alongside the generated code makes it much more readable and easy to modify leading to a major increase in productivity and maintainability.

---

This tutorial is about how to use Regen's templating language (aka `regen-lang`) in Visual Studio but will also teach the `regen-lang` in-depth.<br>

It's also worth mentioning that there are no differences between using `Regen.Core` and  the `VS extension` but for the `#if #else #endif` that allows to place regen templates in-line in C# source code.

If you rather learn from examples, please refer to UnitTests/Examples/ (STILL WIP) or feel free to explore the unit-tests at `Regen.Core.UnitTests`.<br>

#### Installation
//todo nuget package for Regen.Core<br>
Official vs-extension releases can be downloaded [here](https://github.com/SciSharp/CodeMinion/tree/master/src/Regen.Package/releases)

##### Visual Studio Extension 
A code frame / template block inside a C# code looks like this:
```C#
#if _REGEN 
	//this is an input block where the template is placed.
#else   
	//this is where the template's output will be placed after compilation
#endif
```
The `_REGEN` conditional compliation symbol must not be defined at any moment - making the template itself to be ignored by the C#-compiler. The `#else` block is where the generated code of the compiled template is pasted by the plugin/extension.

<strong>Hello World</strong> <br>
The percentage character (`%`) signifies that the content of the following parentheses is a `regen-lang` expression.<br>
Inside foreach-loops we use hashtags (`#`) (we will get to that later).

```C#
#if _REGEN 
	%varname = "Hello World!"
	Console.WriteLine("%(varname)");
#else   
	Console.WriteLine("Hello World!");
#endif
```
<strong>Note:</strong> The following <u>examples</u> as of this moment will run only without any comments (`//` at the end of the line).

#### Expressions
Expressions are evaluated using [Flee](https://github.com/mparlak/Flee) and are C# compliant.<br>
Syntax: &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`%(expr)`<br>
Syntax in-foreach: `#(expr)`<br>

What can I write inside an expression? Anything that works with Flee which covers mostly what works in C#.<br>
```C#
%(1 + 1) //returns 2 (int)
%(1 + 1.1f) //returns 2.1 (upcasted float)
%(1 + 1.1) //returns 2.1 (upcasted double)
```

```C#
%arr = [1,2,]
%(arr[0] + arr[1]) //returns 3
%(arr[0] + arr[arr.Length-1]) //returns 3, arrays impl IList, remember?
```

```C#
%str = "there"
%("hi" + " " + str) //returns "hi there"
%("hi" + " " + str + str[2]) //returns "hi theree"
```
<u>Not supported features</u>: `new`, `throw`, explicit casting, `as`, `is`, `??`, ternery expressions, generics, `sizeof`, `await`

#### Variables and Type System
Regen has it's own type system ([See More](./DataTypes)) with an abstract base class [Data](./DataTypes/Data.cs).<br>
To the user it is mostly transparent.<br>


##### Variable Declaration<br>
Syntax: `%name = expr`<br>
Note: The template is compiled from top to bottom therefore you must first declare variables and then use them.<br>
```C#
//primitives
%an_int = 1 
%a_long = 100000L
%a_float = 1f //or 1.0f
%a_double = 1d //or 1.0d
%a_decimal = 5.321m

%name = "string literal"
%a_char = "c" 

%array = [1,2,3,4, "woah, a string"] //arrays are non-generic.
%dictionary = [key: 1, a_key2: 2, "kk": 3] //dictionariy is declared as an array of key-value pairs
```

#### Foreach Loops
Foreach loops in `regen-lang` allows to iterate a single list/array or multiple lists/arrays at the same time. <br>
An important difference is that inside the body of a `foreach` expression you must use of the hashtag (`#`) instead of the percentage (`%`) to signify expressions.<br>

The synax specification: <br>
    
    multiline:
    %foreach expr, expr ..., exprn%
        template
    %    

    singleline:
    %foreach expr1, expr2 ..., exprn
        template

    expr     - an expression that returns an object that must implement IList
    template - a regen template (using # instead of % for expressions)

As you can see, unlike traditional foreach/for - we can pass multiple expressions.<br>
Every expression that returns an IList will be iterated together to the smallest length of them all.
We access the list values inside the foreach template in `regen-lang` using the loop variables `#1`, `#2` where `#1` accesses the first expression's current value and `#2` accesses second expression's current value and so forth.

The following example shows how a foreach loop iterating over multiple lists plain `C#` code and how it can be rewritten in `regen`:
```C#
//assume expr1 and expr2 are an IList
foreach (var tuple in System.Linq.Enumerable.Zip(expr1, expr2, (val1, val2) => (val1, val2))) {
    Console.WriteLine($"! expr1: {tuple.val1}, expr2: {tuple.val2}");
}
```
Heres how we would implement the same in `regen-lang` with a multi-variable foreach-expression:
```C#
%foreach expr1, expr2% 
    Console.WriteLine($"! expr1: #1, expr2: #2");
% 
```
Note: The engine respects indentation so the template's indentation will be kept in the generated code.

Let's try something less basic, lets iterate an array we defined.
```C#
#if _REGEN 
    %numbers = [1,2,3] //or range(1,3)
    %foreach numbers% 
    Console.WriteLine("#1");
    % 

    some text that has nothing to do with the foreach
#else 
    Console.WriteLine("1");
    Console.WriteLine("2");
    Console.WriteLine("3");

    some text that has nothing to do with the foreach
#endif
```
Note: In case you want to actually write `#` in your template (or `%`), use the backslash `\` to escape them: (//TODO STILL WIP)

``` C#
#if _REGEN 
    %foreach range(1,3)
        Console.Writeline("\#1 = #1");
#else   
        Console.Writeline("#1 = 1");
        Console.Writeline("#1 = 2");
        Console.Writeline("#1 = 3");
#endif
```

What if we want to know our current index (usually `i`) like in a for-loop?<br>
For this we have a reserved variable named `i`. When-ever you use it, it'll hold the value of the current iteration index (0 based).<br>
The usage of `i` must be inside an expression-block like this: `#(i)`<br>
Note: If you'll try to declare a variable named `i`, it'll throw a compilation error.

```C#
#if _REGEN 
    %foreach range(1,3)
        Console.Writeline("\#(i) = #(i), \#1 = #1");
#else   
        Console.Writeline("#(i) = 0, #1 = 1");
        Console.Writeline("#(i) = 1, #1 = 2");
        Console.Writeline("#(i) = 2, #1 = 3");
#endif
```

#### Advanced Examples
```C#
#if _REGEN 
    %foreach [1,2,3,4]%
        var name#1 = #(i * #1);
    %
#else   
        var name1 = 0;
        var name2 = 2;
        var name3 = 6;
        var name4 = 12;
#endif
```
```C#
#if _REGEN 
    using System;
    %types = ["short","int","long"]
    %foreach types%
    public #1 Multiply#(#1.ToUpper())(#1 left, #1 right) {
        return (#1) Convert.ChangeType(left * right, typeof(#1));
    }
    %
#else   
    using System;
    public short MultiplySHORT(short left, short right) {
        return (short) Convert.ChangeType(left * right, typeof(short));
    }
    public int MultiplyINT(int left, int right) {
        return (int) Convert.ChangeType(left * right, typeof(int));
    }
    public long MultiplyLONG(long left, long right) {
        return (long) Convert.ChangeType(left * right, typeof(long));
    }
#endif
```
#### Import
Importing allows the user to use external functions available in expression evaluation.<br>
__Syntax 1:__ &nbsp;&nbsp;&nbsp;&nbsp; %import _namespace.type_<br>
__Syntax 2:__ &nbsp;&nbsp;&nbsp;&nbsp; %import _namespace.type_ as _aliasname_<br>
__Syntax 3 (WIP):__ &nbsp;&nbsp;&nbsp;&nbsp; %import global "./directory/file.cs"<br>
Importing static functions is fairly easy.<br>
Any static function that is imported becomes available in expressions as a lowercase.
Therefore `Math.Sin(double)` turns usable `%(sin(double))`.<br>
If the developer uses syntax 2, aliasname is used as a prefix and should look like the following:
```C#
%import System.Math as math
%a = math.cos(1)
```

###### Default Imports
By default there are couple of namespaces imported,<br>
One of them is `System.Math` so using `Math.Cos(...)` will be in `regen-lang`: `cos(1)`
* `System.Math`
* `Regen.Builtins.CommonRandom as random`
* `Regen.Builtins.CommonRegex` (WIP)
* `Regen.Builtins.CommonLinq` (WIP)
  * `except(list, params objs)` - returns `list` except for items passed via `objs`
  * `concat(list, params objs)` - Concatenate all `objs` with `list` and returns a new array
* `Regen.Builtins.CommonExpressionFunctions`
  *  `forevery(IList, IList, bool exclude)` - Combine the two lists to mimic a nested for loop
     *  Example: forevery([1,2,3], [3,4], false) - will return: [1,1,2,2,3,3], [3,4,3,4,3,4].
     *  exclude will compare the items of current iteration and if they match, it'll skip it.
        *  Example: forevery([1,2,3], [3,4], true) - will return: [1,1,2,2,3], [3,4,3,4,4]. (notice the missing element)
  * `len(ICollection)` - Returns the lenght of given collection (`IList` implements `ICollection`)
  * `range(length)` -  - Returns an array of integer numbers (similar to Enumerable.Range)
  * `range(startFrom, length)` - Returns an array of integer numbers (similar to Enumerable.Range)
  * `str(obj)` - Performs `obj?.ToString() ?? ""`
  * `str(params objs)` - Converts all `objs` to string and then concatenates them.
  * `asarray(params obj)` - Wraps all parameters passed to an Array.
  * `isnull(obj)` - Checks if `obj` is C# `null` or `regen-lang` null.
  * `isarray(obj)` - Checks if `obj` implements `IList`
  * `isnumber(obj)` - Checks if `obj`is a numeric type.
  * `repeat("expr", int repeats, string seperator, ...)` - Repeat expr repeats times inlined.
---
#### Useful Functions
- __Inline Repeat__<br>
`repeat(string expr, int repeats, string seperator, string beforeFirst, string afterFirst,`<br>
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`string beforeLast, string afterLast)`<br>
    Repeat is especially useful when you need to output the same repeating word in the same line.
    `expr` and `seperator` can be an expression if the string starts with `^`. (Escaped version: `\^`)<br>
    Internally occurs a for-loop. its index is available as a variable `n` (0 based).

    __Example:__
```C#
   #if _REGEN
        %pre = "dims.Item"
        %foreach range(2,14)%
        public static implicit operator Shape(#(repeat("int", #1 ,  ", "  ,  "("  ,  ""  ,  ""  ,  ")"  )) dims) => new Shape(#(repeat("^pre+(n+1)", #1 ,  ", " )));
        %
   #else
        public static implicit operator Shape((int, int) dims) => new Shape(dims.Item1, dims.Item2);
        public static implicit operator Shape((int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3);
        public static implicit operator Shape((int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4);
        public static implicit operator Shape((int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5);
        public static implicit operator Shape((int, int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6);
        public static implicit operator Shape((int, int, int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7);
        public static implicit operator Shape((int, int, int, int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8);
        public static implicit operator Shape((int, int, int, int, int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9);
        public static implicit operator Shape((int, int, int, int, int, int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9, dims.Item10);
        public static implicit operator Shape((int, int, int, int, int, int, int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9, dims.Item10, dims.Item11);
        public static implicit operator Shape((int, int, int, int, int, int, int, int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9, dims.Item10, dims.Item11, dims.Item12);
        public static implicit operator Shape((int, int, int, int, int, int, int, int, int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9, dims.Item10, dims.Item11, dims.Item12, dims.Item13);
        public static implicit operator Shape((int, int, int, int, int, int, int, int, int, int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9, dims.Item10, dims.Item11, dims.Item12, dims.Item13, dims.Item14);
        public static implicit operator Shape((int, int, int, int, int, int, int, int, int, int, int, int, int, int, int) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9, dims.Item10, dims.Item11, dims.Item12, dims.Item13, dims.Item14, dims.Item15);
   #endif
```
#### Internal Variables

- `__context__`&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| &nbsp;&nbsp;`Flee.PublicTypes.ExpressionContext`<br>
  Returns Flee's expression context.
- `__vars__`&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| &nbsp;&nbsp;`Flee.PublicTypes.VariableCollection`<br>
  Returns to Flee's expression context variables storage wrapped in `VariableCollectionWrapper`.
- `__compiler__`   &nbsp;&nbsp;| &nbsp;&nbsp;`Regen.Compiler.RegenCompiler`<br>
  Returns the interpreter that the expression is currently running in.


### Global Regen
In some cases there is a need for a shared varaible across an entire solution, project or single file.<br>
`_REGEN_GLOBAL` and `*.regen` filetype were created.

#### _REGEN_GLOBAL
Is precompiled before any `_REGEN` block in that specific file, making `%arr` available <br>in the other `_REGEN` blocks/frames.
```C#
#if _REGEN_GLOBAL
    %arr = [1,2,3]
#endif

#if _REGEN
    %(arr[2])
#else
    3
#endif

#if _REGEN
    %(arr[1])
#else
    2
#endif
```

#### *.regen filetype
`*.regen` files are used to provide variables solution-wide and has to be reloaded from the Regen menu (by `Reload Globals` button) after changes.<br>
The contents of the entire `.regen` file are considered `regen-lang` therefore there is no need for #if blocks.

Example file: `/sharedtypes.regen`
```c#
%numericalTypes = ["int", "short"]
%complexTypes = ["object", "string"]
%allTypes = concat(numericalTypes,complexTypes)
```

In a different file:
```C#
#if _REGEN
    %(allTypes[0])
#else
    int
#endif
```
Once you'll reload globals, these variables will be available accross and `_REGEN` and `_REGEN_TEMPLATE` blocks in this solution.

### Regen File Template
Regen file templating gives the ability to generate multiple files.<br>
##### Syntax: 
```C#
#if _REGEN_TEMPLATE
%template "relative path" for every expr1, expr2 ... , expr-n
#endif

... file contents ...

relative path - may contain #n to access current data relative to the template file.
file contents - regular C# code, any `__n__` will be handled like `#n` inside a foreach.
expr          - an expression that returns an object that must implement IList

```
##### Example:
```C#
#if _REGEN_TEMPLATE
%template "./#2/filename.#1.cs" for every ["INT", "FLOAT"], ["int", "float"]
#endif

public class Convert__1__ { }
```
First file (out of 2) will output as `"./int/filename.INT.cs"` relative to the template file path.<br>
`__n__` are similar to `#n` inside a foreach loop resulting the first file as:<br>
`public class ConvertINT { }`<br><br>
Example file: [test/Regen.Core.UnitTest/Package/tempfilename.template.cs](../../test/Regen.Core.UnitTest/Package/tempfilename.template.cs)

The logic-flow is as follows:
1. The template file is compiled and all `__n__` literals are replaced with their corresponding value.
2. Every template file outputted goes through `Compile File` command triggering compilation at all `_REGEN` blocks
3. The files are saved path mentioned in the `%template` expression relatively to the template file itself.