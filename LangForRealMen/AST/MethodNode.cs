using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class MethodNode : INode
    {
        public INode[] Children { get; set; }

        public IVarType Evaluate()
        {
            return null;
        }

        public override string ToString()
        {
            return Children[0] + " " + Children[1] + Children[2] + Children[3];
        }
    }
}
