namespace RegenApp {
    public class AdvancedExamples {

        public AdvancedExamples(params int[] p) { }

#if _REGEN
        %pre = "dims.Item"
        %foreach range(2,14)%
        public static implicit operator AdvancedExamples(#(repeat("int", #1 ,  ", "  ,  "("  ,  ""  ,  ""  ,  ")"  )) dims) => new AdvancedExamples(#(repeat("^pre+(n+1)", i ,  ", " )));
        %
#else
        public static implicit operator AdvancedExamples((int, int) dims) => new AdvancedExamples();
        public static implicit operator AdvancedExamples((int, int, int) dims) => new AdvancedExamples(dims.Item1);
        public static implicit operator AdvancedExamples((int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2);
        public static implicit operator AdvancedExamples((int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3);
        public static implicit operator AdvancedExamples((int, int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3, dims.Item4);
        public static implicit operator AdvancedExamples((int, int, int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5);
        public static implicit operator AdvancedExamples((int, int, int, int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6);
        public static implicit operator AdvancedExamples((int, int, int, int, int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7);
        public static implicit operator AdvancedExamples((int, int, int, int, int, int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8);
        public static implicit operator AdvancedExamples((int, int, int, int, int, int, int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9);
        public static implicit operator AdvancedExamples((int, int, int, int, int, int, int, int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9, dims.Item10);
        public static implicit operator AdvancedExamples((int, int, int, int, int, int, int, int, int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9, dims.Item10, dims.Item11);
        public static implicit operator AdvancedExamples((int, int, int, int, int, int, int, int, int, int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9, dims.Item10, dims.Item11, dims.Item12);
        public static implicit operator AdvancedExamples((int, int, int, int, int, int, int, int, int, int, int, int, int, int, int) dims) => new AdvancedExamples(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7, dims.Item8, dims.Item9, dims.Item10, dims.Item11, dims.Item12, dims.Item13);
#endif



        public void method(object input)
        {

#if _REGEN
        
        %foreach ["int", "float"]%
            switch(input) {
                %foreach ["int", "float"]%
                case #101 _#101:
                    switch((object)_#101) {
                        %foreach ["int", "float"]%
                        case #201 __#201:
                            switch((object)__#201) {
                                %foreach ["int", "float"]%
                                case #301 ___#301:
                                    break;
                                %
                            }
                            break;
                        %
                    }
                    break;
                %
            }

        %
#else

            switch(input) {
                case int _int:
                    switch((object)_int) {
                        case int __int:
                            switch((object)__int) {
                                case int ___int:
                                    break;
                                case float ___float:
                                    break;
                            }
                            break;
                        case float __float:
                            switch((object)__float) {
                                case int ___int:
                                    break;
                                case float ___float:
                                    break;
                            }
                            break;
                    }
                    break;
                case float _float:
                    switch((object)_float) {
                        case int __int:
                            switch((object)__int) {
                                case int ___int:
                                    break;
                                case float ___float:
                                    break;
                            }
                            break;
                        case float __float:
                            switch((object)__float) {
                                case int ___int:
                                    break;
                                case float ___float:
                                    break;
                            }
                            break;
                    }
                    break;
            }

            switch(input) {
                case int _int:
                    switch((object)_int) {
                        case int __int:
                            switch((object)__int) {
                                case int ___int:
                                    break;
                                case float ___float:
                                    break;
                            }
                            break;
                        case float __float:
                            switch((object)__float) {
                                case int ___int:
                                    break;
                                case float ___float:
                                    break;
                            }
                            break;
                    }
                    break;
                case float _float:
                    switch((object)_float) {
                        case int __int:
                            switch((object)__int) {
                                case int ___int:
                                    break;
                                case float ___float:
                                    break;
                            }
                            break;
                        case float __float:
                            switch((object)__float) {
                                case int ___int:
                                    break;
                                case float ___float:
                                    break;
                            }
                            break;
                    }
                    break;
            }
#endif
        }
    }
}