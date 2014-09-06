using System;
using System.Collections.Generic;
using System.Globalization;
using LangForRealMen.AST;

namespace LangForRealMen.ParserLogic
{
    public class Parser : ParserBase
    {
        private Parser(string source) : base(source) { }
        
        // "культуронезависимый" формат для чисел (с разделителем точкой)
        public static readonly NumberFormatInfo NFI = new NumberFormatInfo();

        // значения идентификаторов
        private static readonly Dictionary<string, double> _values = new Dictionary<string, double>(); 
        public static Dictionary<string, double> Values {
            get
            {
                return _values;
            } 
        }

        
        /*
            Number -> ^(0|[1-9][0-9]*)([.,][0-9]+)?$
            Var -> ^\w[\w\d]+$ <doesn't match with func name>
            Group -> "(" Add ")" | Number | Var
            Neg -> "-"? Group
            Mult -> Neg (("*" | "/") Neg )*
            Add -> Mult (("+" | "-") Mult )*
            Command -> (Var "=" Add) ";"
            Program -> Term *
         */

        public INode Number()
        {
            var number = "";
            while (Current == '.' || char.IsDigit(Current))
            {
                number += Current;
                Next();
            }
            if (number.Length == 0)
                throw new ParserBaseException(string.Format("Ожидалось число (pos={0})", Pos));
            Skip();

            double result;
            if (double.TryParse(number, out result))
            {
                return new NumberNode {Value = double.Parse(number, NFI)};
            }
            throw new ParserBaseException(string.Format("Неверный формат числа (pos={0})", Pos));
        }


        public INode Ident()
        {
            var identifier = "";
            if (char.IsLetter(Current))
            {
                identifier += Current;
                Next();
                while (char.IsLetterOrDigit(Current))
                {
                    identifier += Current;
                    Next();
                }
            }
            else
                throw new ParserBaseException(string.Format("Ожидался идентификатор (pos={0})", Pos));
            Skip();

            return new VarNode {Value = identifier};
        }


        public INode Group()
        {
            if (IsMatch("("))
            {
                Match("(");
                var result = Term();
                Match(")");
                return result;
            }

            if (!char.IsLetter(Current)) 
                return Number();
            
            var pos = Pos;
            var identifier = Ident() as VarNode;
            if (identifier != null && !Values.ContainsKey(identifier.Value))
                throw new ParserBaseException(string.Format("Значение {0} не определено (pos={1})", identifier, pos));

            return identifier;
        }


        public INode Neg()
        {
            INode result;
            if (IsMatch("-"))
            {
                Match("-");
                result = new NegNode {Child = Group()};
            }
            else
                result = Group();

            return result;
        }


        public INode Mult()
        {
            var left = Neg();
            while (IsMatch("*", "/"))
            {
                var op = new MultNode {Value = Match("*", "/"), Nodes = new INode[2]};
                op.Nodes[0] = left;
                op.Nodes[1] = Neg();

                left = op;
            }
            return left;
        }


        public INode Add()
        {
            var left = Mult();
            while (IsMatch("+", "-"))
            {
                var op = new AddNode { Value = Match("+", "-"), Nodes = new INode[2] };
                op.Nodes[0] = left;
                op.Nodes[1] = Mult();

                left = op;
            }
            return left;
        }


        public INode Term()
        {
            return Add();
        }


        public void Expr()
        {
            if (IsMatch("print"))
            {
                Match("print");
                var value = Term();
                Console.WriteLine(value.Evaluate().ToString(NFI));
            }
            else
            {
                var identifier = Ident() as VarNode;
                Match("=");
                var value = Term();
                Values[identifier.Value] = value.Evaluate();
                Console.WriteLine(Values[identifier.Value]);
            }
            Match(";");
        }

        public void Program()
        {
            while (!End)
                Expr();
        }

        public void Result()
        {
            Program();
        }

        public static void Execute(string source)
        {
            new Parser(source).Result();
        }
    }
}
