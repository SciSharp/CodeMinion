- Fix unit test Builtins.len
- Getting started page. (side by side comparison: https://stackoverflow.com/questions/30514408/two-columns-code-in-markdown)
- Indexer inside foreach loops: #[i-1]
- Real world application examples.
- Support global parsing
	- Support for %import global namespace.type which parses _REGEN_GLOBAL blocks.
- Support for %foreach longest expr% which iterates to the longest index, filling rest with default value.
- GUI version for usage without vsix installed. (also to serve as playground)
- Support comments, syntax: `#// ` and `%// ` 
- Support conditional expressions
- Complete support for dictionaries.
- Add support for tuples.

##### Beta
- [ ] Intellisense for _REGEN blocks.
- [ ] Overlay near _REGEN blocks for recompile by clicking.
- [ ] Support nested if expressions