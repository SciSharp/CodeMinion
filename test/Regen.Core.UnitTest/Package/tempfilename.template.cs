#if _REGEN_TEMPLATE
%arr = ["int", "float"]
%template "./#1/tempfilename.#1.cs" for every [arr[0].ToUpper(), arr[1].ToUpper()], ["int", "float"]
#endif
//

namespace RegenApp {
    public class templatefile__1__ {

        public __2__ Add(__2__ left, __2__ right) {
            return left + right;
        }

    }
}

