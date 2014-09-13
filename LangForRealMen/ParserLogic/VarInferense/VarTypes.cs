using System.Collections.Generic;
using System.Linq;
using LangForRealMen.AST;

namespace LangForRealMen.ParserLogic.VarInferense
{
    public interface IVarType
    {
        string ToString();
    }

    /*public enum VarTypeLabel
    {
        IntVar,
        DoubleVar,
        BoolVar,
        StringVar,
        BlockVar
    }*/

    public class VarPack
    {
        public IVarType Var { get; set; }
        //public VarTypeLabel Type { get; set; }
        public bool IsDefined { get; set; }
    }

    public class UndefinedVar : IVarType
    {
        public override string ToString()
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

    public class BlockVar : IVarType
    {
        public IEnumerable<INode> Commands { get; set; }

        public override string ToString()
        {
            if (Commands == null) return "Block is empty";
            
            var s = Commands.Aggregate("", (current, command) => current + command.ToString() + " | ");
            s = s.Substring(0, s.Length - 2);
            
            return s;
        }
    }
}
