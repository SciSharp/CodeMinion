using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Regen.Builtins;
using Regen.DataTypes;
using Array = Regen.DataTypes.Array;

namespace Regen.Compiler {
    public class ForeachConfig {
        public ForeachInstance.StackLength Length { get; set; }
    }

    /// <summary>
    ///     A foreach instance that is used internally in <see cref="Interperter"/>.
    /// </summary>
    public class ForeachInstance {
        protected readonly List<Array> usedVariables;
        protected readonly List<object> parsedVariables;

        /// <summary>
        ///     The stack length that is being used.
        /// </summary>
        public StackLength Length { get; set; }

        /// <summary>
        ///     The pregenerated values for each expected iteration.
        /// </summary>
        public Dictionary<int, StackDictionary> Stacks { get; set; }

        public ForeachInstance(IEnumerable<object> evaluatedVariables, StackLength length) {
            Length = length;
            parsedVariables = evaluatedVariables as List<object> ?? evaluatedVariables.ToList();
            usedVariables = new List<Array>(parsedVariables.Count);

            var cfgObj = parsedVariables.SingleOrDefault(o => o is ForeachConfig);
            if (cfgObj != null) {
                parsedVariables.Remove(cfgObj);
                var config = (ForeachConfig) cfgObj;
                Length = config.Length;
            }

            //expand and evaluate variables to Array
            for (var i = 0; i < parsedVariables.Count; i++) {
                var var = parsedVariables[i];
                switch (var) {
                    case Array _:
                        break;
                    case IList list:
                        parsedVariables[i] = new Array(list.Cast<Data>().ToList());
                        break;
                    case StringScalar ss: {
                        parsedVariables[i] = ss.ToArray();
                        break;
                    }

                    case NullScalar _:
                        parsedVariables[i] = new Array(new List<Data>());
                        break;

                    case NumberScalar num: {
                        if (num.Value is float || num.Value is double) {
                            throw new NotSupportedException($"Unable to iterate in a foreach block a value of type: {var.GetType().Name}");
                        }

                        var @till = num.Cast<int>();
                        var arr = CommonExpressionFunctions.range(@till);
                        parsedVariables[i] = arr;
                        break;
                    }

                    default:
                        throw new NotSupportedException($"Unable to iterate in a foreach block a value of type: {var.GetType().Name}");
                }
            }

            //generate stacks
            //we generate data for largest zip.
            usedVariables.AddRange(parsedVariables.Cast<Array>());
            var maxlen = usedVariables.Max(arr => arr.Values.Count);

            //generate stacks data
            var stacks = new Dictionary<int, StackDictionary>();
            for (int i = 0; i < maxlen; i++) {
                //we use maxlen incase someone offsets forward.
                var stack = new StackDictionary();
                for (var j = 0; j < usedVariables.Count; j++) {
                    try {
                        stack[j + 1] = usedVariables[j].Values[i];
                    } catch (ArgumentOutOfRangeException) {
                        stack[j + 1] = new NullScalar();
                    }
                }

                stacks[i] = stack;
            }

            Stacks = stacks;
        }

        public Interperter.ForLoop StartLoop() {
            var len = Length == StackLength.SmallestIndex ? usedVariables.Min(arr => arr.Values.Count) : Length == StackLength.LargestIndex ? usedVariables.Max(arr => arr.Values.Count) : throw new NotImplementedException();
            return new Interperter.ForLoop() {From = 0, Index = 0, To = len};
        }

        public void Merge(ForeachInstance otherInstance) {
            //todo
        }

        public enum StackLength {
            SmallestIndex,
            LargestIndex
        }
    }
}