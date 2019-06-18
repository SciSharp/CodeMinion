# Getting Started
Regen is an external tool for readable and highly-productive template generating.


Before we get into details, consider the following basic example: <br>

```C#
#if _REGEN 
	//this is an input block where the template is placed.
#else   
	//this is where the compiled template will be pasted.
#endif
```
Note: The following example as of this moment will run only without any comments.
```C#
#if _REGEN 
	%variable = [1|2|3|4] //assigns an array ranging from 1 to 4 to 'variable'
	%(variable[0]) //expression that'll be replace with the number '1'
	//-----------------
	%foreach variable% 
	//# is used only inside loops and is similar to $1, $2 .. $n used in Regex.
	//Only it supports indexing: '#1[i+3]' expression evaluation '#(expression)'
	#1 + #(i) = #(1+i)
	% //close foreach block
	
	//after compilation, output goes to the else block.
#else   //output: ----------
	1
	var x1 = 1 + 0 == 1
	var x2 = 2 + 1 == 3
	var x3 = 3 + 2 == 5
	var x4 = 4 + 3 == 7
#endif
```

##### Variables
Regen supports arrays, strings and numbers (any primitive number).<br>
The arrays are not type specific, for instance it is possible to mix an array with strings and numbers
```C#
%variable_name  = 123.0f
%variable_name2 = ["str"|1.0|1|null|] //note the last empty index which will compile as null.
```

##### Expression
Expressions are evaluated using Flee library and are C# compliant.<br>
Syntax: &nbsp;&nbsp;&nbsp;&nbsp;%(_expression_)
```C#
%(1+1*arr[2])
```

##### Builtin Keywords
Builtin Keywords are used internally and can not be used to declare variables.
* '`i`' is used inside foreach loops and represents current index.

##### Foreach loops
TODO