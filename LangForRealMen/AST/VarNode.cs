using System;
using LangForRealMen.ParserLogic;
using LangForRealMen.ParserLogic.VarInferense;

namespace LangForRealMen.AST
{
    public class VarNode : INode
    {
        public string Value { get; set; }

        public IVarType Evaluate()
        {
            if (!Parser.GetParser().VarCreator.ContainsVarWithName(Value))
                throw new ASTException(string.Format("Use of undeclared variable {0}", Value));

            return Parser.GetParser().VarCreator.GetVar(Value);
        }
    }
}
