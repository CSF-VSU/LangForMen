namespace LangForRealMen.AST
{
    class NegNode : INode
    {
        public INode Child { get; set; }

        public double Evaluate()
        {
            return -Child.Evaluate();
        }
    }
}
