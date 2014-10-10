using System.Linq;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class ParamNode : INode
    {
        public INode[] Nodes { get; set; }

        public IVarType Evaluate()
        {
            return null;
        }

        public override string ToString()
        {
            var part = "(";
            part = Nodes.Aggregate(part, (current, node) => current + (node + ", "));
            part = part.Substring(0, part.Length - 2);
            part += ")";
            return part;
        }
    }
}
