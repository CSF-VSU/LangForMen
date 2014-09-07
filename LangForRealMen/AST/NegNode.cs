using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    class NegNode : INode
    {
        public INode Child { get; set; }

        public IVarType Evaluate()
        {
            var result = Child.Evaluate();

            if (TypeInferer.IsInt(result))
            {
                (result as IntVar).Value *= -1;
                return result;
            }
            if (TypeInferer.IsDouble(result))
            {
                (result as DoubleVar).Value *= -1;
                return result;
            }
            if (TypeInferer.IsBool(result))
            {
                (result as BoolVar).Value = !(result as BoolVar).Value;
                return result;
            }

            throw new ASTException("Смена знака (отрицание) применимо только к числовому и логическому типу.");
        }
    }
}
