- Indexer inside foreach loops: `#[i-1]`
- Support for `%import global namespace.type` which parses _REGEN_GLOBAL blocks.
- Support for %foreach longest expr% which iterates to the longest index, filling rest with default value.
- GUI version for usage without vsix installed. (also to serve as playground)
- Support comments, syntax: `#// ` and `%// ` 
- Support conditional expressions
- Complete support for dictionaries.
- Add support for tuples.
- Support `[1,2][0]` or `[1,2]()` expression
- First parse all expressions %(expr) in a foreach block and then proceed with the post-replacement code.
- A ParsedCode should be usable for multiple compiler.compile calls.
- Support nested foreach.
    - [X] Create a temporary workaround, add a function `forevery(array, array, bool)` which returns a single array
    that is populated as if they were nested foreaches.
- Add a solution-wide Clear and Compile with promt asking if you are sure - please review first.
- Add support for macroing https://www.codeproject.com/KB/recipes/prepro/prepro_src.zip

##### Beta
- [ ] Intellisense for _REGEN blocks.
- [ ] Overlay near _REGEN blocks for recompile by clicking.
- [ ] Support nested if expressions