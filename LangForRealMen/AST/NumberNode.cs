namespace LangForRealMen.AST
{
    public class NumberNode : INode
    {
        public double Value { get; set; }

        public double Evaluate()
        {
            return Value;
        }
    }
}
