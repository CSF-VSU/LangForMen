using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class BoolNode : INode
    {
        public BoolVar Value { get; set; }

        public IVarType Evaluate()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
