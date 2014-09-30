using LangForRealMen.ParserLogic;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class AssignNode : INode
    {
        public INode[] Children { get; set; }

        public IVarType Evaluate()
        {
            if (Children[0] != null)
            {
                if (Children[2] != null)
                    Parser.GetParser().VarCreator
                        .CreateNewVariable(Children[0].ToString(), Children[1].ToString(), Children[2]);
                else
                    Parser.GetParser().VarCreator.CreateNewVariable(Children[0].ToString(), Children[1].ToString());
            }
            else
                Parser.GetParser().VarCreator
                    .Assign(Children[1].ToString(), Children[2]);

            return null;
        }

        public override string ToString()
        {
            var s = Children[0] != null ? Children[0] + " " : "";
            return s + Children[1] + " = " + Children[2] + "\n";
        }
    }
}
