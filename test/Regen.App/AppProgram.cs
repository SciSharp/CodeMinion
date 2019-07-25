using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

#if _REGEN_GLOBAL
%globy = "im from global"
#endif

namespace RegenApp {
    class AppProgram {
        static void Main(string[] args) {
            //  This is a test file for Regen.Package
            //  To test, open 'Regen' menu and press 'Compile File'
            //  And then run this Main file.


            //test basic expression
#if _REGEN
           var a = %(1);
#else

           var a = 1;
#endif
            a.Should().Be(1);

            //test indexing of a variable
#if _REGEN
            %vari = "123"
            a = %(vari[1]);
#else

            a = 2;
#endif
            a.Should().Be(2);

            //test _REGEN_GLOBAL block parsing
#if _REGEN
            var str = 
            @"
            %(globy)
            ";
#else
            var str = 
            @"
            im from global
            ";
#endif
            str.Should().Contain("im from global");

            //test solution-wide global block parsing
#if _REGEN
            a = %(global_variable[2]);
#else

            a = 3;
#endif
            a.Should().Be(3); 
            
            //test foreach
#if _REGEN
            a = 0;
            %foreach range(1,3)%
            a += #1;
            %
#else

            a = 0;
            a += 1;
            a += 2;
            a += 3;
#endif
            a.Should().Be(6);

            //test nested foreach
#if _REGEN
            %a = 0
            %foreach range(1,3)%
                %foreach range(1,3)%
                    |#a = a+#1+#101;
                %
            %
            a.Should().Be(%(a));
#else
            a.Should().Be(36);
#endif

            Console.WriteLine("Test has passed successfully.");
        }
    }
}