#if _REGEN_TEMPLATE
%template "./relative_dir/tempfilename.#1.cs" for every ["INT", "FLOAT"], ["int", "float"]
#endif

namespace RegenApp
{
    public class templatefile__1__
    {

        public __2__ Add(__2__ left, __2__ right)
        {
            return left + right;
        }

#if _REGEN
        %arr = ["__1__", "__2__"]
        //%(arr[0]) %(arr[1])
#else

#endif

    }
}