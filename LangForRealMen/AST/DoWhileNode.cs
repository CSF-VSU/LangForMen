using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class DoWhileNode : WhileNode
    {
        public new IVarType Evaluate()
        {
            // do 1 more iteration 
            return base.Evaluate();
        }

        public override string ToString()
        {
            return "do " + Nodes[1] + " while (" + Nodes[0] + ")\n";
        }
    }
}
