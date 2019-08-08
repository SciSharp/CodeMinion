#### 0.27a
- Added ternary operator syntaxed: cond|true|false
- Added support for ==, !=, ~=, &&, ||, !, ~
- Added BoolScalar implicit casts.
- Added BoolScalar & and | operators.
- Fixed complex expression resolving for cases like: `[1,2,3][1]+[3,3][0]` (array, index 1, add with, array, index 0)
- Flee: Added support full for overloaded operators from https://github.com/mparlak/Flee/pull/52/

- All unit-tests pass except for not implemented features.

#### 0.26a
- Performance optimization to reduce %foreach computation.
- Fixed multiple templates (%tempalte) in the same file.
- Added repeatelement(obj object, int count).

#### 0.25a
- Added support for nested foreach incl. expressions and assignment.
- Added support for `#` expr at the beggining of a line (without breaking VS intellisense) by using `|#` which during compilation replaced with `#`.
- Added support for commented # expressions: `\#...`.
- Added number(obj) methdo that attempts to convert an unknown value to NumberScalar.
- Fixed couple of unit tests.
- Fixed %foreach line is not marked for deletion when the loop did not emit anything.
- Fixed math op between NumberScalar.
- Fixed NumberScalar unpacking when it is constructed with an other NumberScalar.
- Fixed variables were removed even if they were declared outside of the foreach loop.
- Changed 'i', 'i1' .. 'in' are wrapped inside NumberScalar.

#### 0.23a-0.24a
- added file templating support (see TUTORIAL.MD).
- fixed boolean emitting as Camel case instead of camel case (False vs false).
- fixed except(params).
- fixed flatten(params).
- fixed concat(params).
- fixed Array.Create to return itself is it is already an array.

#### 0.22a
- added `forevery(IList,IList, bool)`.
- added `forevery(IList,IList, IList, bool)`.
- fixed extra newline at the beggining of an output after every compile.

#### 0.21a stable
First stable release.