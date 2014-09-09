using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class StringNode : INode
    {
        public string Value { get; set; }

        public IVarType Evaluate()
        {
            return new StringVar {Value = Value};
        }
    }
}
