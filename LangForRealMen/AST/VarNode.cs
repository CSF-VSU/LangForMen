using LangForRealMen.ParserLogic;

namespace LangForRealMen.AST
{
    public class VarNode : INode
    {
        public string Value { get; set; }

        public double Evaluate()
        {
            if (!Parser.Values.ContainsKey(Value))
                throw new ASTException(string.Format("Use of undeclared variable {0}", Value));
            return Parser.Values[Value];
        }
    }
}
