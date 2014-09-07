using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public interface INode
    {
        IVarType Evaluate();
    }
}
