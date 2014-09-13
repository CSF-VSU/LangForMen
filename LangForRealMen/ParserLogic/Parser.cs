using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            Group   ->  "(" Add ")" | Number | Var | String
            Neg     ->  ("-" | "!")? Group
            Mult    ->  Neg (("*" | "/") Neg )*
            Add     ->  Mult (("+" | "-") Mult )*
            Ineq    ->  Add ((">" | ">=" | "<" | "<=" | "==" | "!=" ) Add )?
            Logic   ->  Ineq (("|" | "&") Ineq )*
            Term    ->  <typeName> Var ("=" Logic)?("," Var ("=" Logic)? )* | Func
            Command ->  (Term ";")*
            Block   ->  "{" Command "}"
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
            
            var pars = new List<INode> { Add() };
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


        protected INode BoolConstant()
        {
            return new BoolNode { Value = new BoolVar { Value = "yep" == Match("yep", "nope") } };
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

            if (IsMatch("yep", "nope"))
                return BoolConstant();

            if (!char.IsLetter(Current)) 
                return Number();
            
            //var identifier = ;
            /*if (identifier != null) // && !VarCreator.ContainsVarWithName(identifier.Value))
                throw new ParserBaseException(string.Format("Значение {0} не определено", identifier));
              */
  
            return Ident() as VarNode;
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
            if (!IsMatch(">", ">=", "<", "<=", "==", "!=")) 
                return left;
            
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


        protected IEnumerable<INode> Declaring()
        {
            var result = new List<INode>();
            var type = Match("int", "double", "string", "bool", "block");

            var identifier = Ident() as VarNode;
            if (identifier == null)
                throw  new ASTException("Введите имя переменной!");

            var op = new AssignNode { Children = new INode[3] };
            op.Children[0] = new StringNode { Value = type };
            op.Children[1] = new StringNode { Value = identifier.Value };
            op.Children[2] = null;

            if (IsMatch("="))
            {
                Match("=");

                INode value;
                if (type == "block" || IsMatch("{"))
                    value = Block();
                else
                    value = Logic();

                op.Children[2] = value;
            }
            result.Add(op);


            while (IsMatch(","))
            {
                Match(",");
                identifier = Ident() as VarNode;
                if (identifier == null)
                    throw new ASTException("Введите имя переменной!");

                var op2 = new AssignNode { Children = new INode[3] };
                op2.Children[0] = new StringNode { Value = type };
                op2.Children[1] = new StringNode { Value = identifier.Value };
                op2.Children[2] = null;
                if (IsMatch("="))
                {
                    Match("=");

                    INode value;
                    if (type == "block" || IsMatch("{"))
                        value = Block();
                    else
                        value = Logic();

                    op2.Children[2] = value;
                }
                result.Add(op2);
            }

            return result;
        }


        protected INode Assigning()
        {
            var identifier = Ident() as VarNode;
            if (identifier == null) 
                throw new ASTException("Ожидался идендификатор");
            Match("=");
            var value = Logic();

            var op = new AssignNode { Children = new INode[3] };
            op.Children[0] = null;
            op.Children[1] = new StringNode { Value = identifier.Value };
            op.Children[2] = value;
            
            return op;
        }


        public IEnumerable<INode> Term()
        {
            if (IsMatch("int", "double", "string", "bool", "block"))
                return Declaring();

            return Enumerable.Repeat(IsMatch(FuncNode.GetNames()) ? Function() : Assigning(), 1);
        }


        public IEnumerable<INode> Command()
        {
            var res = new List<INode>();
            while (!IsMatch("}") && !End)
            {
                res.AddRange(Term());
                Match(";");
            }
            return res;
        }

        public INode Block()
        {
            var result = new BlockNode {Value = new BlockVar()};
            Match("{");
            result.Value.Commands = Command();
            Match("}");
            return result;
        }


        public void Result()
        {
            var commands = Term();
            foreach (var node in commands)
            {
                node.Evaluate();
            }
        }


        public void Execute(string source)
        {
            Init(source);
            Result();
        }
    }
}
