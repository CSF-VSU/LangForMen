using System.Linq;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class ArgumentNode : INode
    {
        public INode[] Childen { get; set; }

        public IVarType Evaluate()
        {
            return null;
        }

        public override string ToString()
        {
            var s = "(";
            s = Childen.Aggregate(s, (current, node) => current + (node + ", "));
            s = s.Substring(0, s.Length - 2);
            s += ")";
            return s;
        }
    }
}
