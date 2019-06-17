import inspect
import keras
import json

classTree = []

def gen_func(obj):
    params = inspect.getfullargspec(obj)
    print(params)

def gen_class_def(obj):
    mlist = inspect.getmembers(obj, predicate=inspect.ismethod)
    for name, mobj in mlist:
        print("Class Method: " + name)
        print(mobj)
        
        gen_func(mobj)

    mlist = inspect.getmembers(obj, predicate=inspect.isdatadescriptor)
    for name, mobj in mlist:
        if name == "__weakref__":
            continue
        
        print("Class Prop:" + name)
        print(mobj)

def expand_module(obj):
    clist = inspect.getmembers(obj, predicate=inspect.isclass)
    for name, cobj in clist:
        print("Class:" + name)
        print(inspect.getdoc(cobj))
        gen_class_def(cobj)

def generate():
    members = inspect.getmembers(keras.layers)
    for name, m in members:
        if inspect.ismodule(m):
            expand_module(m)
        
        if inspect.isclass(m):
            gen_class_def(m)

        if inspect.isfunction(m):
            gen_func(m)

if __name__ == "__main__":
    generate()

    
