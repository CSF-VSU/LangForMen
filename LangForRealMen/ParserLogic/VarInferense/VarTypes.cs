namespace LangForRealMen.ParserLogic.VarInferense
{
    public interface IVarType
    {
        string ToString();
    }

    public class IntVar : IVarType
    {
        public int Value { get; set; }
        public bool IsDefined { get; set; }

        public override string ToString()
        {
            return Value.ToString(Parser.NFI);
        }
    }

    public class DoubleVar : IVarType
    {
        public double Value { get; set; }
        public bool IsDefined { get; set; }

        public override string ToString()
        {
            return Value.ToString(Parser.NFI);
        }
    }

    public class CharVar : IVarType
    {
        public char Value { get; set; }
        public bool IsDefined { get; set; }

        public override string ToString()
        {
            return Value.ToString(Parser.NFI);
        }
    }

    public class BoolVar : IVarType
    {
        public bool Value { get; set; }
        public bool IsDefined { get; set; }

        public override string ToString()
        {
            return Value ? "Yep" : "Nope";
        }
    }

    public class StringVar : IVarType
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
