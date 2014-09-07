using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class NumberNode : INode
    {
        public IVarType Value { get; set; }

        public IVarType Evaluate()
        {
            return Value;
        }
    }
}
