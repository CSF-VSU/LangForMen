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

        public static int IdentDepth = 0;

        public INode _program { get; set; }

        // значения идентификаторов
        public VarCreator VarCreator { get; set; }

        /* <typeName> = ["int", "double", "bool", "char", "string"]
         * BoolConst->  ["yep", "nope"]
         * Number   ->  ^(0|[1-9][0-9]*)([.,][0-9]+)?$
         * Var      ->  ^\w[\w\d]+$ <doesn't match with func name>
         * String   ->  '"'<string>'"'
         * Func     ->  <funcName> "(" Logic ( "," Logic )* ")"
         * Group    ->  "(" Logic ")" | Number | Var | String | Function | BoolConst
         * Neg      ->  (("-" | "!") ("-"|"!") <?= Neg : Group ) | Group
         * Mult     ->  Neg (("*" | "/") Neg )*
         * Add      ->  Mult (("+" | "-") Mult )*
         * Ineq     ->  Add ((">" | ">=" | "<" | "<=" | "==" | "!=" ) Add )?
         * Logic    ->  Ineq (("|" | "&") Ineq )*
         * Assigning->  Var "="  "{" <?= Block : Logic
         * 
         * Declaring->  <typeName> Var ("="  "{" <?= Block : Logic)? ("," Var ("="  "{" <?= Block : Logic)? )*
         * 
         * DecArray ->  <typeName> "[" Number? "]" Var ( "<" <typeNameVar>* ">" )?   // int[] x <5, 4, 3>;  int[4] x <4, 3, 4>;  int[] x;  int[6] y;
         * 
         * If       -> "if" Logic  "{" <?= Block : Term ("else"  "{" <?= Block : Term)?
         * Cycle    -> "while" Logic "{" <?= Block : Term | "do" "{" <?= Block : Term
         * Term     ->  Declaring | Assigning | If | Cycle
         * Command  ->  Term ";"
         * Program  -> Command*
         * Block    ->  "{" Program "}"
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

            if (FuncNode.GetNames().Contains(identifier))
                throw new ParserBaseException("У переменной и функции не может быть одинакового имени!");

            return new VarNode {Value = identifier};
        }

        protected INode Function()
        {
            var func = new FuncNode { Value = Match(FuncNode.GetNames()) };
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

            if (IsMatch(FuncNode.GetNames()))
            {
                return Function();
            }

            if (IsMatch("yep", "nope"))
                return BoolConstant();

            if (!char.IsLetter(Current)) 
                return Number();
            
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

        protected INode DeclaringArray(string type, VarNode ident)
        {
            Match("[");

            var size = -1;
            if (!IsMatch("]"))
            {
                bool isInt;
                size = (int) TypeInferer.GetNumericValue(Number().Evaluate(), out isInt);
            }
            Match("]");

            var res = new ArrayNode();
            res.Nodes[0] = new StringNode {Value = type};
            res.Nodes[1] = new StringNode {Value = ident.Value};
            res.Nodes[2] = new NumberNode {Value = new IntVar {Value = size}};

            if (!IsMatch("<")) 
                return res;
            
            Match("<");

            res.Nodes[3] = new ArrayInitNode();
            (res.Nodes[3] as ArrayInitNode).Data.Add((Number() as NumberNode).Value);

            while (IsMatch(","))
            {
                Match(",");
                (res.Nodes[3] as ArrayInitNode).Data.Add((Number() as NumberNode).Value);
            }

            Match(">");

            ((res.Nodes[2] as NumberNode).Value as IntVar).Value = (res.Nodes[3] as ArrayInitNode).Data.Count;

            return res;
        }

        protected INode Declaring()
        {
            var result = new List<INode>();
            var type = Match("int", "double", "string", "bool", "block");

            var identifier = Ident() as VarNode;
            if (identifier == null)
                throw  new ASTException("Введите имя переменной!");

            if (IsMatch("["))
                return DeclaringArray(type, identifier);

            var op = new AssignNode { Children = new INode[3] };
            op.Children[0] = new StringNode { Value = type };
            op.Children[1] = new StringNode { Value = identifier.Value };
            op.Children[2] = null;

            if (IsMatch("="))
            {
                Match("=");

                var value = IsMatch("{") ? Block() : Logic();
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

                    var value = IsMatch("{") ? Block() : Logic();

                    op2.Children[2] = value;
                }
                result.Add(op2);
            }

            if (result.Count == 1)
                return result[0];
            
            var block = new BlockNode { Value = new BlockVar() };
            foreach (var node in result)
            {
                block.Value.Commands.Add(node);
            }
            return block;
        }

        protected INode Assigning(VarNode identifier)
        {
            Match("=");

            INode value;
            if (VarCreator.IsBlockVar(identifier.Value) || IsMatch("{"))
                value = Block();
            else
                value = Logic();

            var op = new AssignNode { Children = new INode[3] };
            op.Children[0] = null;
            op.Children[1] = new StringNode { Value = identifier.Value };
            op.Children[2] = value;
            
            return op;
        }

        protected INode IfStatement()
        {
            Match("if");
            Match("(");
            var res = new IfNode {Nodes = new INode[3]};
            res.Nodes[0] = Logic();
            Match(")");
            if (IsMatch("{"))
                res.Nodes[1] = Block();
            else
                res.Nodes[1] = Term();
            res.Nodes[2] = null;

            if (!IsMatch("else")) 
                return res;
            
            Match("else");
            if (IsMatch("{"))
                res.Nodes[2] = Block();
            else
                res.Nodes[2] = Term();

            return res;
        }

        protected INode WhileStatement()
        {
            Match("while");
            Match("(");

            var result = new WhileNode();
            result.Nodes[0] = Logic();

            Match(")");

            if (IsMatch("{"))
                result.Nodes[1] = Block();
            else
                result.Nodes[1] = Term();

            return result;
        }

        protected INode DoWhileStatement()
        {
            Match("do");

            var result = new DoWhileNode();

            if (IsMatch("{"))
                result.Nodes[1] = Block();
            else
                result.Nodes[1] = Term();

            Match("while");
            Match("(");

            result.Nodes[0] = Logic();

            Match(")");

            return result;
        }

        public INode Term()
        {
            if (IsMatch("if"))
                return IfStatement();

            if (IsMatch("do"))
                return DoWhileStatement();

            if (IsMatch("while"))
                return WhileStatement();

            if (IsMatch("int", "double", "string", "bool", "block"))
                return Declaring();

            if (IsMatch(FuncNode.GetNames()))
                return Function();

            var var = Ident() as VarNode;
            if (var == null)
                throw new ASTException("Ожидался идендификатор");

            return !IsMatch(";") ? Assigning(var) : new BlockNode { Value = (VarCreator.GetVar(var.Value) as BlockVar) };
        }

        public INode Command()
        {
            var res = Term();
            Match(";");
            return res;
        }

        public INode Program()
        {
            var result = new BlockNode { Value = new BlockVar() };

            while (!IsMatch("}"))
                result.Value.Commands.Add(Command());

            return result;
        } 

        public INode Block()
        {
            Match("{");
            var result = Program();
            Match("}");
            return result;
        }

        public void Parse(string source)
        {
            Init(source);
            _program = Block();
        }

        public void Execute()
        {
            _program.Evaluate();
        }
    }
}
