using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    class NegNode : INode
    {
        public INode Child { get; set; }
        public string Value { get; set; }

        public IVarType Evaluate()
        {
            var result = Child.Evaluate();

            if (TypeInferer.IsInt(result) && Value == "-")
            {
                (result as IntVar).Value *= -1;
                return result;
            }
            if (TypeInferer.IsDouble(result) && Value == "-")
            {
                (result as DoubleVar).Value *= -1;
                return result;
            }
            if (TypeInferer.IsBool(result) && Value == "!")
            {
                (result as BoolVar).Value = !(result as BoolVar).Value;
                return result;
            }

            throw new ASTException("Смена знака (отрицание) применимо только к числовому и логическому типу.");
        }

        public override string ToString()
        {
            return Value + Child;
        }
    }
}
