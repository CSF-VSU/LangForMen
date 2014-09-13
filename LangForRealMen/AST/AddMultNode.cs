using System;
using System.Collections.Generic;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class AddMultNode : INode
    {
        protected static Dictionary<string, Func<double, double, double>> DoubleOperations
            = new Dictionary<string, Func<double, double, double>>
            {
                {"+", (x, y) => x + y},
                {"-", (x, y) => x - y},
                {"*", (x, y) => x*y},
                {"/", (x, y) => x/y}
            };

        protected static Dictionary<string, Func<int, int, int>> IntOperations
            = new Dictionary<string, Func<int, int, int>>
            {
                {"+", (x, y) => x + y},
                {"-", (x, y) => x - y},
                {"*", (x, y) => x*y},
                {"/", (x, y) => x/y},
                {"%", (x, y) => x%y}
            };

        public string Value { get; set; }
        public INode[] Nodes { get; set; }

        public IVarType Evaluate()
        {
            if (!DoubleOperations.ContainsKey(Value) && !IntOperations.ContainsKey(Value))
                throw new ASTException(string.Format("Operation {0} is invalid", Value));

            var left = Nodes[0].Evaluate();
            var right = Nodes[1].Evaluate();

            if (TypeInferer.IsNumeric(left, right))
            {
                if (TypeInferer.IsInt(left, right))
                    return new IntVar
                    {
                        Value = IntOperations[Value]((left as IntVar).Value, (right as IntVar).Value)
                    };

                if (TypeInferer.IsDouble(left, right))
                    return new DoubleVar
                    {
                        Value = DoubleOperations[Value]((left as DoubleVar).Value, (right as DoubleVar).Value)
                    };

                if (TypeInferer.IsInt(left) && TypeInferer.IsDouble(right))
                    return new DoubleVar
                    {
                        Value = DoubleOperations[Value]((left as IntVar).Value, (right as DoubleVar).Value)
                    };

                return new DoubleVar
                {
                    Value = DoubleOperations[Value]((left as DoubleVar).Value, (right as IntVar).Value)
                };
            }

            if (TypeInferer.IsString(left) && Value == "+")
                return new StringVar {Value = (left as StringVar).Value + right.ToString()};
            if (TypeInferer.IsString(right) && Value == "+")
                return new StringVar {Value = left.ToString() + (right as StringVar).Value};
            

            throw new ASTException("Невозможно применить операцию к данным операндам.");
        }

        public override string ToString()
        {
            return Nodes[0] + " " + Value + " " + Nodes[1];
        }
    }
}
