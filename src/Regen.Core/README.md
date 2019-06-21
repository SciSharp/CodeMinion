# Regen
Regen a templating engine  an external tool for readable and highly-productive template generating.

Current version: 0.01 pre-alpha<br>

### Installation
Regen is shipped as a visual studio extension (VSIX) and currently support vs2017 and vs2019.<br>
Official releases can be downloaded [here](https://github.com/SciSharp/CodeMinion/tree/master/src/Regen.Package/releases).<br>

Make sure to read our [getting started](TUTORIAL.md) page!

---
### TODO
##### Alpha

- [ ] Getting started page.
- [ ] Real world application examples.
- [ ] ForLoops unit-tests.
- [X] Extending builtin functions by adding types. syntax: `%import namespace.type`
  - [X] Unit tests.
  - [ ] Support for %import global namespace.type which parses _REGEN_GLOBAL blocks.
- [X] Escaped `\%` should be unescaped on output.
- [X] Support comments, syntax: `#// `
- [X] 200 Unit tests. _(progress: 200+)_
- [ ] GUI version for usage without vsix installed. (also to serve as playground)
- [X] Support names that end with number.
- [X] Add Context as a builtin variable.
- [X] Add a variable that contains all variables.

- [X] Support booleans
- [ ] Support conditional emit
- [ ] Add support for dictionaries.
- [ ] Add support for tuples.

##### Beta
- [ ] Intellisense for _REGEN blocks.
- [ ] Overlay near _REGEN blocks for recompile by clicking.
- [ ] Support nested if expressions