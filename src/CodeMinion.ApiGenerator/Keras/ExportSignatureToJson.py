import inspect
from inspect import signature
import keras
import json

library = []

def gen_func(obj):
    params = inspect.getfullargspec(obj)
    print(params)

def gen_class_def(obj):
    mlist = inspect.getmembers(obj, predicate=inspect.ismethod)
    funclist = []
    for name, mobj in mlist:
        if inspect.isbuiltin(mobj):
            continue
        moduleFunc = {}
        moduleFunc["Name"] = name
        spec = inspect.getfullargspec(mobj)
        moduleFunc["Args"] = spec.args
        if not spec.defaults is None:
            def_list = str(spec.defaults).replace('(', '').replace(')', '').split(',')
            moduleFunc["Defaults"] = [x for x in def_list if x]
        moduleFunc["DocStr"] = inspect.getdoc(mobj)
        funclist.append(moduleFunc)
    
    return funclist

def expand_module(name, obj):
    module = {"Name": name, "Type": "Module", "Classes": [], "Functions": []}
    clist = inspect.getmembers(obj, predicate=inspect.isclass)
    
    for name, cobj in clist:
        if inspect.isbuiltin(cobj):
            continue

        moduleCls = {}
        moduleCls["Name"] = name
        moduleCls["Type"] = "Class"
        moduleCls["Abstract"] = inspect.isabstract(cobj)
        try:
            spec = inspect.getfullargspec(cobj)
            moduleCls["Args"] = spec.args
            if not spec.defaults is None:
                def_list = str(spec.defaults).replace('(', '').replace(')', '').split(',')
                moduleCls["Defaults"] = [x for x in def_list if x]
        except:
            pass
        
        moduleCls["DocStr"] = inspect.getdoc(cobj)
        moduleCls["Functions"] = gen_class_def(cobj)
        module["Classes"].append(moduleCls)

    mlist = inspect.getmembers(obj, predicate=inspect.isfunction)
    for name, mobj in mlist:
        if inspect.isbuiltin(mobj):
            continue
        moduleFunc = {}
        moduleFunc["Name"] = name
        spec = inspect.getfullargspec(mobj)
        moduleFunc["Args"] = spec.args
        if not spec.defaults is None:
            def_list = str(spec.defaults).replace('(', '').replace(')', '').split(',')
            moduleFunc["Defaults"] = [x for x in def_list if x]
        moduleFunc["DocStr"] = inspect.getdoc(mobj)
        module["Functions"].append(moduleFunc)

    library.append(module)

def generate():
    modules = inspect.getmembers(keras, predicate=inspect.ismodule)
    for name, m in modules:
        expand_module(m.__name__, m)
	
    #with open('sig.json', 'w') as f:  # writing JSON object
    #    json.dump(library, f, indent=4, sort_keys=True)
        
    return json.dumps(library)
        

if __name__ == "__main__":
    generate()


    
