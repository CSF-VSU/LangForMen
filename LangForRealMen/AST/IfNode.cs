using LangForRealMen.ParserLogic;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class IfNode : INode
    {
        public INode[] Nodes { get; set; }

        public IfNode()
        {
            Nodes = new INode[3];
            Nodes[0] = null;
            Nodes[1] = null;
            Nodes[2] = null;
        }

        public IVarType Evaluate()
        {
            if (Nodes[0] is BoolNode || Nodes[0] is RelationNode)
            {
                var success = Nodes[0].Evaluate() as BoolVar;
                if (success != null && success.Value)
                    return Nodes[1].Evaluate();
                if (Nodes[2] != null)
                    return Nodes[2].Evaluate();
            }
            else
                throw new ASTException("Узел условия цикла должен иметь логический тип!");
            
            return null;
        }

        public override string ToString()
        {
            var res = "\n";
            for (var i = 0; i < Parser.IdentDepth; i++)
                res += "\t";
            res += "if (" + Nodes[0] + ")\n";
            for (var i = 0; i < Parser.IdentDepth + 1; i++)
                res += "\t";

            res += Nodes[1];
            
            if (Nodes[2] != null)
            {
                for (var i = 0; i < Parser.IdentDepth; i++)
                    res += "\t";

                res += "else\n";

                for (var i = 0; i < Parser.IdentDepth + 1; i++)
                    res += "\t";
                res += Nodes[2];
            }
                
            return res + "\n";
        }
    }
}