using System;
using LangForRealMen.ParserLogic;

namespace LangForRealMen.AST
{
    public class VarNode : INode
    {
        public string Value { get; set; }

        public double Evaluate()
        {
            if (!Parser.GetParser().VarCreator.ContainsVarWithName(Value))
                throw new ASTException(string.Format("Use of undeclared variable {0}", Value));

            Type type;
            var result = Parser.GetParser().VarCreator.GetVar(Value, out type);
            if (type == typeof (int))
                return Convert.ToInt32(result);
            if (type == typeof(double))
                return Convert.ToDouble(result);
            
            //TODO: исправить это месиво
            throw new ASTException(string.Format("Исправить это говно!"));
        }
    }
}
