#define DEBUG

using System;
using System.Globalization;
using LangForRealMen.AST;

namespace LangForRealMen.ParserLogic
{
    public class Parser : ParserBase
    {
        #region Singleton

        private static Parser _instance;

        private Parser()
        {
            VarCreator = new VarCreator();
        }

        public static Parser GetParser()
        {
            return _instance ?? (_instance = new Parser());
        }

        #endregion
        
        
        // "культуронезависимый" формат для чисел (с разделителем точкой)
        public static readonly NumberFormatInfo NFI = new NumberFormatInfo();

        // значения идентификаторов
        public VarCreator VarCreator { get; set; }
        

         
        
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

        protected INode Number()
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


        protected INode Ident()
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


        protected INode Group()
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
            if (identifier != null && !VarCreator.ContainsVarWithName(identifier.Value))
                throw new ParserBaseException(string.Format("Значение {0} не определено (pos={1})", identifier, pos));

            return identifier;
        }


        protected INode Neg()
        {
            INode result;
            if (IsMatch("-"))
            {
                Match("-");
                result = IsMatch("-") ? new NegNode {Child = Neg()} : new NegNode {Child = Group()};
            }
            else
                result = Group();

            return result;
        }


        protected INode Mult()
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


        protected INode Add()
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


        protected INode Term()
        {
            return Add();
        }


        protected void Declaring()
        {
            string type;
            if (IsMatch("int"))
            {
                Match("int");
                type = "int";
            }
            else if (IsMatch("double"))
            {
                Match("double");
                type = "double";
            }
            else
            {
                Match("string");
                type = "string";
            }

            var identifier = Ident() as VarNode;
            if (identifier == null)
                throw  new ASTException("Введите имя переменной!");

            if (IsMatch("="))
            {
                Match("=");
                var value = Term();
                VarCreator.CreateNewVariable(type, identifier.Value, value);
            }
            else
                VarCreator.CreateNewVariable(type, identifier.Value);

            while (IsMatch(","))
            {
                Match(",");
                identifier = Ident() as VarNode;
                if (identifier == null)
                    throw new ASTException("Введите имя переменной!");

                if (IsMatch("="))
                {
                    Match("=");
                    var value = Term();
                    VarCreator.CreateNewVariable(type, identifier.Value, value);
                }
                else
                    VarCreator.CreateNewVariable(type, identifier.Value);
            }
        }


        public void Expr()
        {
            if (IsMatch("print"))
            {
                Match("print");
                var value = Term();
                Console.WriteLine(value.Evaluate().ToString(NFI));
            }
            else if (IsMatch("int", "double", "string"))
            {
                Declaring();
            }
            /*else
            {
                var identifier = Ident() as VarNode;
                Match("=");
                var value = Term();
                Values[identifier.Value] = value;
#if DEBUG
                Console.WriteLine(Values[identifier.Value].Evaluate());
#endif
            }*/

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

        public void Execute(string source)
        {
            Init(source);
            Result();
        }
    }
}
