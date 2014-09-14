using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class IfNode : INode
    {
        public INode[] Nodes { get; set; }

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
    }
}
