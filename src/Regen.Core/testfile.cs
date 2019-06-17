#if _REGEN_GLOBALS
%c=["hithere"|"twice"]
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regen {


#if _REGEN
        %foreach operators%
        public static dynamic operator #1 (OperatorsOverloading lhs, int rhs) {
            dynamic left = lhs.Value;
            dynamic right = rhs;
            return left #1 right;
        }        
        %
#else



#endif
#if _REGEN
        %a = ["NDArray"|"Double"|"Single"|"Decimal"|"Int32"|"Byte"|"Int16"|"UInt16"|"UInt32"|"Int64"|"UInt64"|"Char"|"Complex"|"String"|"Boolean"|"Object"]
        %indexes = ["i"|"j"|"k"|"m"|"n"|"g"|"h"|"e"|"f"]
        switch (DType.Name) //%(c[0])  //%(c[2-1])
        {
        %foreach a%
            case "#1":
                return _array#1;
        %
            default: 
                throw new NotImplementedException($"GetData {DType.Name}");
        }
#else

    //heyyyy


#endif
#if _REGEEN
        %a = [1|2|3|4]
        %b = ["null"|"gig"|"yo"|"hi"|]
		%c = 123f
		%d = "nigga."
        %foreach zipmax(a,b)%
        //lol#alol#1[#1-1]
        %
#else

#endif

#if _REGEEN
        %a = [1|2|3|4]
        %b = [null|"gig"|"yo"|"hi"|]
        %c = 123 - 412
        
        %d1 = 123.0f + 123.0d
        %d2 = 123f
        %d4 = 123
        %d5 = 123.0M
        
        %e1 = "hey there pretty!%@%$%#\"@%Ifdscvxcv #!@#!"
        %e2 = len(a) + 1
        %e3 = a[len(a) - 2]
        %e3 = a[len(a) - 2]+3
        
        %f = [e3]
        
        %(f[0]/2)
        
        %foreach a%
        //lol#1
        //lol#1#1
        %
        
        %foreach a|b
        //lol#alol#1[i-#1+#1-1]#(#1-1)#2
        
        %foreach a|b%
        lol#alol#2[#1-1]
        Console.WriteLine("Error! #2");
        %
        
        %foreach range(3,3)%
        lol#alol#1[#1-1]
        Console.WriteLine("Error! #1");
        %
        
        %foreach a%
        //lol#alol#1[#1-1]
        %

#else

#endif
}