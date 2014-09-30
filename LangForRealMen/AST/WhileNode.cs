using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class WhileNode : INode
    {
        public INode[] Nodes { get; set; }

        public WhileNode()
        {
            Nodes = new INode[2];
            Nodes[0] = null;
            Nodes[1] = null;
        }

        public IVarType Evaluate()
        {
            return null;
        }

        public override string ToString()
        {
            return "while (" + Nodes[0] + ") " + Nodes[1] + "\n";
        }
    }
}
