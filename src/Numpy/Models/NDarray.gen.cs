using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Python.Runtime;
using Python.Included;
using NumSharp;

namespace Numpy
{
    public partial class NDarray
    {
        
        /// <summary>
        /// Copy an element of an array to a standard Python scalar and return it.
        /// 
        /// Notes
        /// 
        /// When the data type of a is longdouble or clongdouble, item() returns
        /// a scalar array object because there is no available Python scalar that
        /// would not lose information. Void arrays return a buffer object for item(),
        /// unless fields are defined, in which case a tuple is returned.
        /// 
        /// item is very similar to a[args], except, instead of an array scalar,
        /// a standard Python scalar is returned. This can be useful for speeding up
        /// access to elements of the array and doing arithmetic on elements of the
        /// array using Python’s optimized math.
        /// </summary>
        /// <returns>
        /// A copy of the specified element of the array as a suitable
        /// Python scalar
        /// </returns>
        public T item<T>(params int[] args)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
                args,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("item", pyargs, kwargs);
            return ToCsharp<T>(py);
        }
        
        /// <summary>
        /// Return the array as a (possibly nested) list.
        /// 
        /// Return a copy of the array data as a (nested) Python list.
        /// Data items are converted to the nearest compatible Python type.
        /// 
        /// Notes
        /// 
        /// The array may be recreated, a = np.array(a.tolist()).
        /// </summary>
        /// <returns>
        /// The possibly nested list of array elements.
        /// </returns>
        public List<T> tolist<T>()
        {
            //auto-generated code, do not change
            dynamic py = self.InvokeMethod("tolist");
            return ToCsharp<List<T>>(py);
        }
        
        /// <summary>
        /// Write array to a file as text or binary (default).
        /// 
        /// Data is always written in ‘C’ order, independent of the order of a.
        /// The data produced by this method can be recovered using the function
        /// fromfile().
        /// 
        /// Notes
        /// 
        /// This is a convenience function for quick storage of array data.
        /// Information on endianness and precision is lost, so this method is not a
        /// good choice for files intended to archive data or transport data between
        /// machines with different endianness. Some of these problems can be overcome
        /// by outputting the data as text files, at the expense of speed and file
        /// size.
        /// 
        /// When fid is a file object, array contents are directly written to the
        /// file, bypassing the file object’s write method. As a result, tofile
        /// cannot be used with files objects supporting compression (e.g., GzipFile)
        /// or file-like objects that do not support fileno() (e.g., BytesIO).
        /// </summary>
        /// <param name="fid">
        /// An open file object, or a string containing a filename.
        /// </param>
        /// <param name="sep">
        /// Separator between array items for text output.
        /// If “” (empty), a binary file is written, equivalent to
        /// file.write(a.tobytes()).
        /// </param>
        /// <param name="format">
        /// Format string for text file output.
        /// Each entry in the array is formatted to text by first converting
        /// it to the closest Python type, and then using “format” % item.
        /// </param>
        public void tofile(string fid, string sep, string format)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
                fid,
                sep,
                format,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("tofile", pyargs, kwargs);
        }
        
        /// <summary>
        /// Dump a pickle of the array to the specified file.
        /// The array can be read back with pickle.load or numpy.load.
        /// </summary>
        /// <param name="file">
        /// A string naming the dump file.
        /// </param>
        public void dump(string file)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
                file,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("dump", pyargs, kwargs);
        }
        
        /// <summary>
        /// Returns the pickle of the array as a string.
        /// pickle.loads or numpy.loads will convert the string back to an array.
        /// </summary>
        public void dumps()
        {
            //auto-generated code, do not change
            dynamic py = self.InvokeMethod("dumps");
        }
        
        /// <summary>
        /// Copy of the array, cast to a specified type.
        /// 
        /// Notes
        /// 
        /// Starting in NumPy 1.9, astype method now returns an error if the string
        /// dtype to cast to is not long enough in ‘safe’ casting mode to hold the max
        /// value of integer/float array that is being casted. Previously the casting
        /// was allowed even if the result was truncated.
        /// </summary>
        /// <param name="dtype">
        /// Typecode or data-type to which the array is cast.
        /// </param>
        /// <param name="order">
        /// Controls the memory layout order of the result.
        /// ‘C’ means C order, ‘F’ means Fortran order, ‘A’
        /// means ‘F’ order if all the arrays are Fortran contiguous,
        /// ‘C’ order otherwise, and ‘K’ means as close to the
        /// order the array elements appear in memory as possible.
        /// Default is ‘K’.
        /// </param>
        /// <param name="casting">
        /// Controls what kind of data casting may occur. Defaults to ‘unsafe’
        /// for backwards compatibility.
        /// </param>
        /// <param name="subok">
        /// If True, then sub-classes will be passed-through (default), otherwise
        /// the returned array will be forced to be a base-class array.
        /// </param>
        /// <param name="copy">
        /// By default, astype always returns a newly allocated array. If this
        /// is set to false, and the dtype, order, and subok
        /// requirements are satisfied, the input array is returned instead
        /// of a copy.
        /// </param>
        /// <returns>
        /// Unless copy is False and the other conditions for returning the input
        /// array are satisfied (see description for copy input parameter), arr_t
        /// is a new array of the same shape as the input array, with dtype, order
        /// given by dtype, order.
        /// </returns>
        public NDarray astype(Dtype dtype, string order = null, string casting = null, bool? subok = null, bool? copy = null)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
                dtype,
            });
            var kwargs=new PyDict();
            if (order!=null) kwargs["order"]=ToPython(order);
            if (casting!=null) kwargs["casting"]=ToPython(casting);
            if (subok!=null) kwargs["subok"]=ToPython(subok);
            if (copy!=null) kwargs["copy"]=ToPython(copy);
            dynamic py = self.InvokeMethod("astype", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        /// <summary>
        /// Swap the bytes of the array elements
        /// 
        /// Toggle between low-endian and big-endian data representation by
        /// returning a byteswapped array, optionally swapped in-place.
        /// </summary>
        /// <param name="inplace">
        /// If True, swap bytes in-place, default is False.
        /// </param>
        /// <returns>
        /// The byteswapped array. If inplace is True, this is
        /// a view to self.
        /// </returns>
        public NDarray byteswap(bool? inplace = null)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
            });
            var kwargs=new PyDict();
            if (inplace!=null) kwargs["inplace"]=ToPython(inplace);
            dynamic py = self.InvokeMethod("byteswap", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        /// <summary>
        /// Return a copy of the array.
        /// </summary>
        /// <param name="order">
        /// Controls the memory layout of the copy. ‘C’ means C-order,
        /// ‘F’ means F-order, ‘A’ means ‘F’ if a is Fortran contiguous,
        /// ‘C’ otherwise. ‘K’ means match the layout of a as closely
        /// as possible. (Note that this function and numpy.copy are very
        /// similar, but have different default values for their order=
        /// arguments.)
        /// </param>
        public void copy(string order = null)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
            });
            var kwargs=new PyDict();
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("copy", pyargs, kwargs);
        }
        
        /// <summary>
        /// Returns a field of the given array as a certain type.
        /// 
        /// A field is a view of the array data with a given data-type. The values in
        /// the view are determined by the given type and the offset into the current
        /// array in bytes. The offset needs to be such that the view dtype fits in the
        /// array dtype; for example an array of dtype complex128 has 16-byte elements.
        /// If taking a view with a 32-bit integer (4 bytes), the offset needs to be
        /// between 0 and 12 bytes.
        /// </summary>
        /// <param name="dtype">
        /// The data type of the view. The dtype size of the view can not be larger
        /// than that of the array itself.
        /// </param>
        /// <param name="offset">
        /// Number of bytes to skip before beginning the element view.
        /// </param>
        public void getfield(Dtype dtype, int offset)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
                dtype,
                offset,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("getfield", pyargs, kwargs);
        }
        
        /// <summary>
        /// Set array flags WRITEABLE, ALIGNED, (WRITEBACKIFCOPY and UPDATEIFCOPY),
        /// respectively.
        /// 
        /// These Boolean-valued flags affect how numpy interprets the memory
        /// area used by a (see Notes below). The ALIGNED flag can only
        /// be set to True if the data is actually aligned according to the type.
        /// The WRITEBACKIFCOPY and (deprecated) UPDATEIFCOPY flags can never be set
        /// to True. The flag WRITEABLE can only be set to True if the array owns its
        /// own memory, or the ultimate owner of the memory exposes a writeable buffer
        /// interface, or is a string. (The exception for string is made so that
        /// unpickling can be done without copying memory.)
        /// 
        /// Notes
        /// 
        /// Array flags provide information about how the memory area used
        /// for the array is to be interpreted. There are 7 Boolean flags
        /// in use, only four of which can be changed by the user:
        /// WRITEBACKIFCOPY, UPDATEIFCOPY, WRITEABLE, and ALIGNED.
        /// 
        /// WRITEABLE (W) the data area can be written to;
        /// 
        /// ALIGNED (A) the data and strides are aligned appropriately for the hardware
        /// (as determined by the compiler);
        /// 
        /// UPDATEIFCOPY (U) (deprecated), replaced by WRITEBACKIFCOPY;
        /// 
        /// WRITEBACKIFCOPY (X) this array is a copy of some other array (referenced
        /// by .base). When the C-API function PyArray_ResolveWritebackIfCopy is
        /// called, the base array will be updated with the contents of this array.
        /// 
        /// All flags can be accessed using the single (upper case) letter as well
        /// as the full name.
        /// </summary>
        /// <param name="write">
        /// Describes whether or not a can be written to.
        /// </param>
        /// <param name="align">
        /// Describes whether or not a is aligned properly for its type.
        /// </param>
        /// <param name="uic">
        /// Describes whether or not a is a copy of another “base” array.
        /// </param>
        public void setflags(bool? write = null, bool? align = null, bool? uic = null)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
            });
            var kwargs=new PyDict();
            if (write!=null) kwargs["write"]=ToPython(write);
            if (align!=null) kwargs["align"]=ToPython(align);
            if (uic!=null) kwargs["uic"]=ToPython(uic);
            dynamic py = self.InvokeMethod("setflags", pyargs, kwargs);
        }
        
        /// <summary>
        /// Fill the array with a scalar value.
        /// </summary>
        /// <param name="@value">
        /// All elements of a will be assigned this value.
        /// </param>
        public void fill(ValueType @value)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
                @value,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("fill", pyargs, kwargs);
        }
        
        /// <summary>
        /// Returns a view of the array with axes transposed.
        /// 
        /// For a 1-D array, this has no effect. (To change between column and
        /// row vectors, first cast the 1-D array into a matrix object.)
        /// For a 2-D array, this is the usual matrix transpose.
        /// For an n-D array, if axes are given, their order indicates how the
        /// axes are permuted (see Examples). If axes are not provided and
        /// a.shape = (i[0], i[1], ... i[n-2], i[n-1]), then
        /// a.transpose().shape = (i[n-1], i[n-2], ... i[1], i[0]).
        /// </summary>
        /// <returns>
        /// View of a, with axes suitably permuted.
        /// </returns>
        public NDarray transpose(int[] axes = null)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
                axes,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("transpose", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        /// <summary>
        /// Return a copy of the array collapsed into one dimension.
        /// </summary>
        /// <param name="order">
        /// ‘C’ means to flatten in row-major (C-style) order.
        /// ‘F’ means to flatten in column-major (Fortran-
        /// style) order. ‘A’ means to flatten in column-major
        /// order if a is Fortran contiguous in memory,
        /// row-major order otherwise. ‘K’ means to flatten
        /// a in the order the elements occur in memory.
        /// The default is ‘C’.
        /// </param>
        /// <returns>
        /// A copy of the input array, flattened to one dimension.
        /// </returns>
        public NDarray flatten(string order = null)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
            });
            var kwargs=new PyDict();
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("flatten", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        /// <summary>
        /// Sort an array, in-place.
        /// 
        /// Notes
        /// 
        /// See sort for notes on the different sorting algorithms.
        /// </summary>
        /// <param name="axis">
        /// Axis along which to sort. Default is -1, which means sort along the
        /// last axis.
        /// </param>
        /// <param name="kind">
        /// Sorting algorithm. Default is ‘quicksort’.
        /// </param>
        /// <param name="order">
        /// When a is an array with fields defined, this argument specifies
        /// which fields to compare first, second, etc.  A single field can
        /// be specified as a string, and not all fields need be specified,
        /// but unspecified fields will still be used, in the order in which
        /// they come up in the dtype, to break ties.
        /// </param>
        public void sort(int? axis = null, string kind = null, string order = null)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
            });
            var kwargs=new PyDict();
            if (axis!=null) kwargs["axis"]=ToPython(axis);
            if (kind!=null) kwargs["kind"]=ToPython(kind);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("sort", pyargs, kwargs);
        }
        
        /// <summary>
        /// Rearranges the elements in the array in such a way that the value of the
        /// element in kth position is in the position it would be in a sorted array.
        /// All elements smaller than the kth element are moved before this element and
        /// all equal or greater are moved behind it. The ordering of the elements in
        /// the two partitions is undefined.
        /// 
        /// Notes
        /// 
        /// See np.partition for notes on the different algorithms.
        /// </summary>
        /// <param name="kth">
        /// Element index to partition by. The kth element value will be in its
        /// final sorted position and all smaller elements will be moved before it
        /// and all equal or greater elements behind it.
        /// The order of all elements in the partitions is undefined.
        /// If provided with a sequence of kth it will partition all elements
        /// indexed by kth of them into their sorted position at once.
        /// </param>
        /// <param name="axis">
        /// Axis along which to sort. Default is -1, which means sort along the
        /// last axis.
        /// </param>
        /// <param name="kind">
        /// Selection algorithm. Default is ‘introselect’.
        /// </param>
        /// <param name="order">
        /// When a is an array with fields defined, this argument specifies
        /// which fields to compare first, second, etc. A single field can
        /// be specified as a string, and not all fields need to be specified,
        /// but unspecified fields will still be used, in the order in which
        /// they come up in the dtype, to break ties.
        /// </param>
        public void partition(int[] kth, int? axis = null, string kind = null, string order = null)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
                kth,
            });
            var kwargs=new PyDict();
            if (axis!=null) kwargs["axis"]=ToPython(axis);
            if (kind!=null) kwargs["kind"]=ToPython(kind);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("partition", pyargs, kwargs);
        }
        
        /// <summary>
        /// For unpickling.
        /// 
        /// The state argument must be a sequence that contains the following
        /// elements:
        /// </summary>
        /// <param name="version">
        /// optional pickle version. If omitted defaults to 0.
        /// </param>
        /// <param name="rawdata">
        /// a binary string with the data (or a list if ‘a’ is an object array)
        /// </param>
        public void __setstate__(int version, NumSharp.Shape shape, Dtype dtype, bool isFortran, string rawdata)
        {
            //auto-generated code, do not change
            var pyargs=ToTuple(new object[]
            {
                version,
                shape,
                dtype,
                isFortran,
                rawdata,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("__setstate__", pyargs, kwargs);
        }
        
    }
}
