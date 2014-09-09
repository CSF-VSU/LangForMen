using System;
using System.Collections.Generic;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class RelationNode : INode
    {
        protected static Dictionary<string, Func<double, double, bool>> RelationOperations
            = new Dictionary<string, Func<double, double, bool>>
            {
                {">", (x, y) => x > y},
                {">=", (x, y) => x >= y},
                {"<", (x, y) => x < y},
                {"<=", (x, y) => x <= y},
                {"==", (x, y) => Math.Abs(x - y) < 1e-7},
                {"!=", (x, y) => Math.Abs(x - y) > 1e-7}
            };

        protected static Dictionary<string, Func<bool, bool, bool>> BoolOperations
            = new Dictionary<string, Func<bool, bool, bool>>
            {
                {"&", (b, b1) => b & b1},
                {"|", (b, b1) => b | b1}
            };

        public string Value { get; set; }
        public INode[] Nodes { get; set; }

        public IVarType Evaluate()
        {
            var left = Nodes[0].Evaluate();
            var right = Nodes[1].Evaluate();

            if (TypeInferer.IsNumeric(left, right))
            {
                bool isInt;
                return new BoolVar
                {
                    Value = RelationOperations[Value](
                        TypeInferer.GetNumericValue(left, out isInt), 
                        TypeInferer.GetNumericValue(right, out isInt))
                };
            }

            if (TypeInferer.IsBool(left, right))
            {
                return new BoolVar
                {
                    Value = BoolOperations[Value]((left as BoolVar).Value, (right as BoolVar).Value)
                };
            }

            throw new ASTException("Нельзя применить операции сравнения к нечисловым типам.");
        }
    }
}
