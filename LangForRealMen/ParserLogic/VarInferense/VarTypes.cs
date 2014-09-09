namespace LangForRealMen.ParserLogic.VarInferense
{
    public interface IVarType
    {
        string ToString();
    }

    public struct VarPack
    {
        public IVarType Var;
        public bool IsDefined;
    }

    public class UndefinedVar : IVarType
    {
        public string ToString()
        {
            return "<undefined>";
        }
    }

    public class IntVar : IVarType
    {
        public int Value { get; set; }

        public override string ToString()
        {
            return Value.ToString(Parser.NFI);
        }
    }

    public class DoubleVar : IVarType
    {
        public double Value { get; set; }

        public override string ToString()
        {
            return Value.ToString(Parser.NFI);
        }
    }


    public class BoolVar : IVarType
    {
        public bool Value { get; set; }

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
