using System;
using System.Collections.Generic;
using Regen.Compiler.Helpers;
using Regen.Flee.PublicTypes;

namespace Regen.Compiler {

    /// <summary>
    ///     A class that is capable of evaluating a string and returning its value in the Regen Data System Format (RDSF).
    /// </summary>
    public interface IEvaluator {
        ExpressionContext Context { get; set; }
        string EvaluateString(string expression, Line line = null);
        Int32 EvaluateInt32(string expression, Line line = null);
        T Evaluate<T>(string expression, Line line = null);
        object EvaluateUnpackedObject(string expression, Line line = null);
        object EvaluateObject(string expression, Line line = null);
    }
}