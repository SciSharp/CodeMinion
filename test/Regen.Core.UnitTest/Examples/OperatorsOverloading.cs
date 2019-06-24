namespace Regen.Core.Tests.Examples {
#if _REGEN_GLOBAL
%supportedTypes = ["Boolean","Byte","Int16","UInt16","Int32","UInt32","Int64","UInt64","Char","Double","Single","Decimal","String","Object"]
#endif

    public class OperatorsOverloading {
        public dynamic Value; //a value that can perform overrides

        public OperatorsOverloading(dynamic value) {
            Value = value;
        }

#if _REGEN
        //todo support multiple loops
        %foreach range(3,3)
        //calls range(start, count) from Builtins/CommonExpressionFunctions.cs resulting in: #1

        %operators = ["+","-","*","%","/","&"]
        %foreach operators%
        public static dynamic operator #1(OperatorsOverloading lhs, int rhs) {
            dynamic left = lhs.Value;
            dynamic right = rhs;
            return left #1 right;
        }            
        
        public static dynamic operator #1(int lhs, OperatorsOverloading rhs) {
            dynamic left = lhs;
            dynamic right = rhs.Value;
            return left #1 right;
        }        
        %
#else
        //todo support multiple loops
        //calls range(start, count) from Builtins/CommonExpressionFunctions.cs resulting in: 3
        //calls range(start, count) from Builtins/CommonExpressionFunctions.cs resulting in: 4
        //calls range(start, count) from Builtins/CommonExpressionFunctions.cs resulting in: 5

        public static dynamic operator +(OperatorsOverloading lhs, int rhs) {
            dynamic left = lhs.Value;
            dynamic right = rhs;
            return left + right;
        }

        public static dynamic operator +(int lhs, OperatorsOverloading rhs) {
            dynamic left = lhs;
            dynamic right = rhs.Value;
            return left + right;
        }

        public static dynamic operator -(OperatorsOverloading lhs, int rhs) {
            dynamic left = lhs.Value;
            dynamic right = rhs;
            return left - right;
        }

        public static dynamic operator -(int lhs, OperatorsOverloading rhs) {
            dynamic left = lhs;
            dynamic right = rhs.Value;
            return left - right;
        }

        public static dynamic operator *(OperatorsOverloading lhs, int rhs) {
            dynamic left = lhs.Value;
            dynamic right = rhs;
            return left * right;
        }

        public static dynamic operator *(int lhs, OperatorsOverloading rhs) {
            dynamic left = lhs;
            dynamic right = rhs.Value;
            return left * right;
        }

        public static dynamic operator %(OperatorsOverloading lhs, int rhs) {
            dynamic left = lhs.Value;
            dynamic right = rhs;
            return left % right;
        }

        public static dynamic operator %(int lhs, OperatorsOverloading rhs) {
            dynamic left = lhs;
            dynamic right = rhs.Value;
            return left % right;
        }

        public static dynamic operator /(OperatorsOverloading lhs, int rhs) {
            dynamic left = lhs.Value;
            dynamic right = rhs;
            return left / right;
        }

        public static dynamic operator /(int lhs, OperatorsOverloading rhs) {
            dynamic left = lhs;
            dynamic right = rhs.Value;
            return left / right;
        }

        public static dynamic operator &(OperatorsOverloading lhs, int rhs) {
            dynamic left = lhs.Value;
            dynamic right = rhs;
            return left & right;
        }

        public static dynamic operator &(int lhs, OperatorsOverloading rhs) {
            dynamic left = lhs;
            dynamic right = rhs.Value;
            return left & right;
        }
#endif
    }
}