using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class BlockNode : INode
    {
        public BlockVar Value { get; set; }

        public IVarType Evaluate()
        {
            foreach (var command in Value.Commands)
            {
                command.Evaluate();
            }
            
            return null;            
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
