# Getting Started
Regen at its core is a templating engine that uses C#-Python like syntax and is entirely written in C#.<br>
Its purpose is to replace T4 Templating with intuitive and fast in-code (not in a seperate file) scripting/templating.<br>
Having your template along the generated code makes it much more readable and easy to modify leading to a major increase in productivity and maintainability.

---

If you wish to use Regen via code, feel free to explore the unit-tests at `Regen.Core.UnitTests`.<br>
This tutorial will be about how to use Regen's templating language (aka `regen-lang`) along visual studio but will also teach the `regen-lang` in-depth.<br>

It's also worth mentioning that there are no differences between using `Regen.Core` and  the `VS extension` but for the `#if #else #endif` that helps with incapsulating the code inside VS editor.

If you rather learn from examples, please refer to UnitTests/Examples/ (STILL WIP).<br>

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
The `_REGEN` conditional compliation symbol is not defined at any moment - making the template itself to be ignored by the compiler but on
the other hand the `#else` block does compile and is where the compiled template is pasted by the plugin/extension.

<strong>Hello World</strong> <br>
The precentage character (`%`) is used to tell the compiler that the following expression parentheses belongs to `regen-lang` and contain an expression.<br>
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

What can I write inside an expression? Anything that works with Flee which is covers mostly what works in C#.<br>
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
<u>Not supported features</u>: new, throw, explicit casting, as, is, ??, :? (diamond operator), sizeof, await

#### Variables and Type System
Regen has it's own Type system ([See More](./DataTypes)) with an abstract base class [Data](./DataTypes/Data.cs).<br>
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
%dictionary = [key: 1, a_key2: 2, "kk": 3] //is an array but the values are a KeyValue
```

#### Foreach Loops
Foreach loops in `regen-lang` allows to iterate a single list/array or multiple lists/arrays at the same time. <br>
An important difference is the mark character is a hashtag (`#`) instead of precentage (%).<br>

The synax specification: <br>
    
    multiline:
    %foreach expr, expr ..., exprn%
        template
    %    

    singleline:
    %foreach expr1, expr2 ..., exprn
        template

    expr - an expression that returns an object that must implement IList

As you can see, unlike traditional foreach/for - we can pass multiple expressions.<br>
Every expression that returns an IList will be iterated together to the smallest length of them all.
We access the list values inside the foreach template similarly to Regex's sytanx.<br>
Regex uses `$1, $2 .. $n` for their match.<br>
`regen-lang` uses `#1`, `#2` where `#1` accesses the first expression's current value and `#2` accesses second expression's current value.

Heres an example of how a multiple-expressions foreach loop can be translated to plain `C#` code:
```C#
//assume expr1 and expr2 are an IList
foreach (var tuple in System.Linq.Enumerable.Zip(expr1, expr2, (val1, val2) => (val1, val2))) {
    Console.WriteLine($"! expr1: {tuple.val1}, expr2: {tuple.val2}");
}
```
Heres how we would implement the same in `regen-lang`:
```C#
%foreach expr1, expr2% 
    ! expr1: #1, expr2: #2
% 
```
Note: The engine knows how to copy indentions and keeps them during output.

Let's try something less basic, lets iterate an array we defined.
```C#
#if _REGEN 
    %numbers = [1,2,3,4] //or range(1,4)
    %foreach numbers% 
    Console.WriteLine("#1");
    % 

    some text that has nothing to do with the foreach
#else 
    Console.WriteLine("1");
    Console.WriteLine("2");
    Console.WriteLine("3");
    Console.WriteLine("4");

    some text that has nothing to do with the foreach
#endif
```
Note: Incase you want to actually write `#` in your template (or `%`), add a reversed slash `\` before the # and it will not be compiled! (//TODO STILL WIP)

What if we want to know our current index (usually `i`) like in a for-loop?<br>
For this we have a reserved variable named `i`. When-ever you use it, it'll hold the value of the current iteration index (0 based).<br>
The usage of `i` must be inside an expression-block like this: `#(i)`<br>
Note: If you'll try to declare a variable named `i`, it'll throw a compilation error.
```C#
#if _REGEN 
    %foreach [1,2,3,4]
        #(i) <> #1
#else   
        0 <> 1
        1 <> 2
        2 <> 3
        3 <> 4
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
Importing static functions is faily easy,<br>
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
  * `isnumber(obj)` - Checks if 
---

#### Internal Variables

- `__context__`&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| &nbsp;&nbsp;`Flee.PublicTypes.ExpressionContext`<br>
  Returns Flee's expression context.
- `__vars__`&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| &nbsp;&nbsp;`Flee.PublicTypes.VariableCollection`<br>
  Returns to Flee's expression context variables storage wrapped in `VariableCollectionWrapper`.
- `__compiler__`   &nbsp;&nbsp;| &nbsp;&nbsp;`Regen.Compiler.RegenCompiler`<br>
  Returns the interpreter that the expression is currently running in.


