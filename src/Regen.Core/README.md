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
- [ ] %(expr) should be parsed first before foreach, allowing to insert data into the foreach loops's template.
- [ ] Real world application examples.
- [ ] Foreach Loops
  - [ ] ForLoops unit-tests.
- [ ] Evaluation of arrays.
- [ ] Support for %import global namespace.type which parses _REGEN_GLOBAL blocks.
- [ ] GUI version for usage without vsix installed. (also to serve as playground)
- [ ] Support comments, syntax: `#// `
- [ ] Support conditional expressions
- [ ] Add support for dictionaries.
- [ ] Add support for tuples.

##### Beta
- [ ] Intellisense for _REGEN blocks.
- [ ] Overlay near _REGEN blocks for recompile by clicking.
- [ ] Support nested if expressions