using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class ArrayNode : INode
    {
        public INode[] Nodes { get; set; }

        public ArrayNode()
        {
            Nodes = new INode[4];
            Nodes[0] = null;    //type
            Nodes[1] = null;    //name
            Nodes[2] = null;    //size
            Nodes[3] = null;    //data
        }

        public IVarType Evaluate()
        {
            return null;
        }

        public override string ToString()
        {
            return "" + Nodes[0] + " "+ Nodes[1] + " [" + Nodes[2] + "] " + Nodes[3] + "\n\n";
        }
    
    }
}
