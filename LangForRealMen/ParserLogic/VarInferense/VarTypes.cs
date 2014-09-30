using System.Collections.Generic;
using System.Linq;
using LangForRealMen.AST;

namespace LangForRealMen.ParserLogic.VarInferense
{
    public interface IVarType
    {
        string ToString();
    }

    public class VarPack
    {
        public IVarType Var { get; set; }
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
        public List<INode> Commands { get; set; }

        public BlockVar()
        {
            Commands = new List<INode>();    
        }

        public override string ToString()
        {
            if (Commands == null) return "Block is empty";

            var s = "{\n";
            Parser.IdentDepth++;

            foreach (var command in Commands)
            {
                for (var i = 0; i < Parser.IdentDepth; i++)
                    s += "\t";
                s += command;
            }

            Parser.IdentDepth--;
            for (var i = 0; i < Parser.IdentDepth; i++)
                s += "\t";
            s += "}";
            return s;
        }
    }
}
