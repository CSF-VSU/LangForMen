using System;
using System.Collections.Generic;
using System.Globalization;
using LangForRealMen.AST;
using LangForRealMen.ParserLogic.VarInferense;

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
            <typeName> = ["int", "double", "bool", "char", "string"]
            Number  ->  ^(0|[1-9][0-9]*)([.,][0-9]+)?$
            Var     ->  ^\w[\w\d]+$ <doesn't match with func name>
            String  ->  '"'<string>'"'
            Func    ->  "(" Add ( "," Add )* ")"
            Group   ->  "(" Add ")" | Number | Var | Func | String
            Neg     ->  ("-" | "!")? Group
            Mult    ->  Neg (("*" | "/") Neg )*
            Add     ->  Mult (("+" | "-") Mult )*
            Ineq    ->  Add ((">" | ">=" | "<" | "<=" | "==" | "!=" ) Add )?
            Logic   ->  Ineq (("|" | "&") Ineq )*
            Command ->  "print" Logic | <typeName> Var ("=" Logic)?("," Var ("=" Logic)? )*
            Program ->  (Command ";")*
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

            
            number = number.Replace('.', ',');
            var isDouble = number.Contains(",");

            double result;
            if (double.TryParse(number, out result))
            {
                return isDouble
                    ? new NumberNode {Value = new DoubleVar {Value = double.Parse(number)}}
                    : new NumberNode {Value = new IntVar {Value = int.Parse(number)}};
            }

            throw new ParserBaseException(string.Format("Неверный формат числа (pos={0})", Pos));
        }

        protected INode String()
        {
            var result = "";
            while (!IsMatch("\""))
            {
                result += Current;
                Next();
            }
            return new StringNode {Value = result};
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


        protected INode Function()
        {
            var func = new FuncNode {Value = Match(FuncNode.GetNames())};
            Match("(");
            
            var pars = new List<INode> {Add()};
            while (IsMatch(","))
            {
                Match(",");
                pars.Add(Add());
            }

            func.Nodes = new INode[pars.Count];
            func.Nodes = pars.ToArray();

            Match(")");
            return func;
        }


        protected INode Group()
        {
            if (IsMatch("("))
            {
                Match("(");
                var result = Logic();
                Match(")");
                return result;
            }

            if (IsMatch("\""))
            {
                Match(true, "\"");
                var result = String();
                Match("\"");
                return result;
            }

            if (!char.IsLetter(Current)) 
                return Number();

            if (IsMatch(FuncNode.GetNames()))
            {
                return Function();
            }
            
            var identifier = Ident() as VarNode;
            if (identifier != null && !VarCreator.ContainsVarWithName(identifier.Value))
                throw new ParserBaseException(string.Format("Значение {0} не определено", identifier));
                
            return identifier;
        }


        protected INode Neg()
        {
            INode result;
            if (IsMatch("!", "-"))
            {
                var op = Match("!", "-");
                result = IsMatch(op)
                    ? new NegNode {Value = op, Child = Neg()}
                    : new NegNode {Value = op, Child = Group()};
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
                var op = new AddMultNode {Value = Match("*", "/"), Nodes = new INode[2]};
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
                var op = new AddMultNode {Value = Match("+", "-"), Nodes = new INode[2]};
                op.Nodes[0] = left;
                op.Nodes[1] = Mult();

                left = op;
            }
            return left;
        }


        protected INode Inequality()
        {
            var left = Add();
            if (!IsMatch(">", ">=", "<", "<=", "==", "!=")) return left;
            
            var op = new RelationNode {Value = Match(">", ">=", "<", "<=", "==", "!="), Nodes = new INode[2]};
            op.Nodes[0] = left;
            op.Nodes[1] = Add();
            return op;
        }


        protected INode Logic()
        {
            var left = Inequality();
            while (IsMatch("|", "&"))
            {
                var op = new RelationNode { Value = Match("|", "&"), Nodes = new INode[2] };
                op.Nodes[0] = left;
                op.Nodes[1] = Inequality();

                left = op;
            }
            return left;
        }


        protected void Declaring()
        {
            var type = Match("int", "double", "string", "bool");

            var identifier = Ident() as VarNode;
            if (identifier == null)
                throw  new ASTException("Введите имя переменной!");

            if (IsMatch("="))
            {
                Match("=");
                var value = Logic();
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
                    var value = Logic();
                    VarCreator.CreateNewVariable(type, identifier.Value, value);
                }
                else
                    VarCreator.CreateNewVariable(type, identifier.Value);
            }
        }

        protected void Assigning()
        {
            var identifier = Ident() as VarNode;
            Match("=");
            var value = Logic();
            VarCreator.Assign(identifier.Value, value);
        }


        public void Expr()
        {
            if (IsMatch("print"))
            {
                Match("print");
                var value = Logic();
                Console.WriteLine(value.Evaluate().ToString());
            }
            else if (IsMatch("int", "double", "string", "bool"))
            {
                Declaring();
            }
            else
            {
                Assigning();
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

        public void Execute(string source)
        {
            Init(source);
            Result();
        }
    }
}
