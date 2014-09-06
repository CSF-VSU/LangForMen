using System;
using System.Collections.Generic;

namespace LangForRealMen.AST
{
    public class AddNode : INode
    {
        protected static Dictionary<string, Func<double, double, double>> Operations
            = new Dictionary<string, Func<double, double, double>>
            {
                {"+", (x, y) => x + y },
                {"-", (x, y) => x - y }
            };


        public string Value { get; set; }
        public INode[] Nodes { get; set; }

        public double Evaluate()
        {
            if (!Operations.ContainsKey(Value))
                throw new ASTException(string.Format("Operation {0} is invalid", Value));
            return Operations[Value](Nodes[0].Evaluate(), Nodes[1].Evaluate());
        }
    }
}
