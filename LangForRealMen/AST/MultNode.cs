using System;
using System.Collections.Generic;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class MultNode : INode
    {
        protected static Dictionary<string, Func<double, double, double>> DoubleOperations
            = new Dictionary<string, Func<double, double, double>>
            {
                {"*", (x, y) => x*y},
                {"/", (x, y) => x/y}
            };

        protected static Dictionary<string, Func<int, int, int>> IntOperations
            = new Dictionary<string, Func<int, int, int>>
            {
                {"*", (x, y) => x*y},
                {"/", (x, y) => x/y},
                {"%", (x, y) => x%y}
            };

        public string Value { get; set; }
        public INode[] Nodes { get; set; }

        public IVarType Evaluate()
        {
            if (!IntOperations.ContainsKey(Value) && !DoubleOperations.ContainsKey(Value))
                throw new ASTException(string.Format("Operation {0} is invalid", Value));

            var left = Nodes[0].Evaluate();
            var right = Nodes[1].Evaluate();

            if (TypeInferer.IsNumeric(left, right))
            {
                if (TypeInferer.IsInt(left, right))
                    return new IntVar
                    {
                        IsDefined = true,
                        Value = IntOperations[Value]((left as IntVar).Value, (right as IntVar).Value)
                    };

                if (TypeInferer.IsDouble(left, right))
                    return new DoubleVar
                    {
                        IsDefined = true,
                        Value = DoubleOperations[Value]((left as DoubleVar).Value, (right as DoubleVar).Value)
                    };

                if (TypeInferer.IsInt(left) && TypeInferer.IsDouble(right))
                    return new DoubleVar
                    {
                        IsDefined = true,
                        Value = DoubleOperations[Value]((left as IntVar).Value, (right as DoubleVar).Value)
                    };

                return new DoubleVar
                {
                    IsDefined = true,
                    Value = DoubleOperations[Value]((left as DoubleVar).Value, (right as IntVar).Value)
                };
            }
            else
            {
                //strings, bools
            }
            throw new ASTException("Невозможно применить операцию к данным операндам.");
        }
    }
}
