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
- [ ] Support multiple loops. 
- [ ] Extending builtin functions by adding types. syntax: `%import namespace.type`
- [ ] Escaped `\%` should be unescaped on output.
- [ ] Support comments, syntax: `#// `
- [ ] 200 Unit tests _(progress: 120)_
- [ ] GUI version for usage without vsix installed.
- [X] Support names that end with number
- [ ] Support booleans
- [ ] Support conditional emit


##### Beta
- [ ] Intellisense for _REGEN blocks.
- [ ] Overlay near _REGEN blocks for recompile by clicking.
- [ ] Support nested if expressions