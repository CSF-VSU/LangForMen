using System;
using System.Collections.Generic;
using System.Linq;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
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

        public static string[] GetNames()
        {
            return FuncOperations1.Keys.ToArray();
        }

        public static bool IsFunctionName(string name)
        {
            return (FuncOperations1.ContainsKey(name));
        }            

        public string Value { get; set; }
        public INode[] Nodes { get; set; }

        public IVarType Evaluate()
        {
            if (!FuncOperations1.ContainsKey(Value))
                throw new ASTException(string.Format("Не существует функции {0}", Value));

            bool isInt;
            var p = TypeInferer.GetNumericValue(Nodes[0].Evaluate(), out isInt);
            return new DoubleVar {Value = FuncOperations1[Value](p)};
        }
    }
}
