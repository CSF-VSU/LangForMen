using System.Collections.Generic;
using System.Linq;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class ArrayInitNode : INode
    {
        public List<IVarType> Data { get; set; }

        public ArrayInitNode()
        {
            Data = new List<IVarType>();
        }

        public IVarType Evaluate()
        {
            return null;
        }

        public override string ToString()
        {
            var res = Data.Aggregate("< ", (s, type) => s + type.ToString() + ", ");
            res = res.Substring(0, res.Length - 2) + " >";
            return res;
        }
    }
}
