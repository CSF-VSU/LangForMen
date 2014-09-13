using System;
using System.Collections.Generic;
using System.Linq;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public delegate void FuncDelegate(INode node);

    public class FuncNode : INode
    {
        protected static Dictionary<string, Func<double, double>> FuncOperations1
            = new Dictionary<string, Func<double, double>>
            {
                {"sin", Math.Sin},
                {"asin", Math.Asin},
                {"arcsin", Math.Asin},
                {"cos", Math.Cos},
                {"acos", Math.Acos},
                {"arccos", Math.Acos},
                {"tg", Math.Tan},
                {"tan", Math.Tan},
                {"atg", Math.Atan},
                {"atan", Math.Atan},
                {"arctg", Math.Atan},
                {"arctan", Math.Atan},
                {"ctg", x => 1.0d/Math.Tan(x)},
                {"cotan", d => 1.0d/Math.Tan(d)},
                {"actg", x => 1.0d/Math.Tan(x)},
                {"acotan", x => 1.0d/Math.Tan(x)},
                {"arcctg", x => 1.0d/Math.Tan(x)},
                {"arccotan", x => 1.0d/Math.Tan(x)},
                {"ceiling", Math.Ceiling},
                {"floor", Math.Floor},
                {"abs", Math.Abs},
                {"exp", Math.Exp},
                {"ln", Math.Log},
                {"lg", Math.Log10},
                {"sqrt", Math.Sqrt},
                {"trunc", Math.Truncate}
            };

        protected static Dictionary<string, Func<double, double, double>> FuncOperations2
            = new Dictionary<string, Func<double, double, double>>
            {
                {"pow", Math.Pow},
                {"log", Math.Log},
                {"arctg", Math.Atan2}
            };

        protected static Dictionary<string, FuncDelegate> FuncUtils
            = new Dictionary<string, FuncDelegate>
            {
                {"print", node => Console.WriteLine(node.Evaluate().ToString())}
            };


        public static string[] GetNames()
        {
            var first = FuncOperations1.Keys.ToArray();
            var second = FuncOperations2.Keys.ToArray();
            var third = FuncUtils.Keys.ToArray();
            return first.Concat(second).ToArray().Concat(third).ToArray();
        }


        public static bool IsFunctionName(string name)
        {
            return (FuncOperations1.ContainsKey(name) || FuncOperations2.ContainsKey(name) ||
                    FuncUtils.ContainsKey(name));
        }   
         

        public string Value { get; set; }
        public INode[] Nodes { get; set; }

        public IVarType Evaluate()
        {
            if (!FuncOperations1.ContainsKey(Value) && !FuncOperations2.ContainsKey(Value) &&
                !FuncUtils.ContainsKey(Value))
                throw new ASTException(string.Format("Не существует функции {0}", Value));

            bool isInt;

            if (FuncUtils.ContainsKey(Value))
            {
                if (Nodes.GetLength(0) != 1)
                    throw new ASTException(String.Format("Функция {0} требует 1 входной параметр!", Value));

                FuncUtils[Value](Nodes[0]);
                return null;
            }

            if (FuncOperations1.ContainsKey(Value))
            {
                if (Nodes.GetLength(0) != 1)
                    throw new ASTException(String.Format("Функция {0} требует 1 входной параметр!", Value));
                
                var p = TypeInferer.GetNumericValue(Nodes[0].Evaluate(), out isInt);
                return new DoubleVar { Value = FuncOperations1[Value](p) };
            }

            if (Nodes.GetLength(0) != 2)
                throw new ASTException(String.Format("Функция {0} требует 2 входных параметра!", Value));

            var p1 = TypeInferer.GetNumericValue(Nodes[0].Evaluate(), out isInt);
            var p2 = TypeInferer.GetNumericValue(Nodes[0].Evaluate(), out isInt);
            return new DoubleVar {Value = FuncOperations2[Value](p1, p2)};
        }

        public override string ToString()
        {
            var part = Value + "(";
            part = Nodes.Aggregate(part, (current, node) => current + (node + ", "));
            part = part.Substring(0, part.Length - 2);
            part += ")";
            return part;
        }
    }
}
