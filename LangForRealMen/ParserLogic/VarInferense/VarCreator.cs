using System;
using System.Collections.Generic;
using System.Linq;
using LangForRealMen.AST;

namespace LangForRealMen.ParserLogic.VarInferense
{
    public class VarCreator 
    {
        #region Warehouse

        private readonly Dictionary<string, VarPack> _vars;

        public Dictionary<string, VarPack> Vars
        {
            get
            {
                return _vars;
            }
        }
        #endregion
        
        public VarCreator()
        {
            _vars = new Dictionary<string, VarPack>();
        }

        #region Creation

        public void CreateNewVariable(string type, string name, INode value)
        {
            IVarType result;
            if (!(value is BlockNode))
                result = value.Evaluate();
            else
                result = (value as BlockNode).Value;

            switch (type)
            {
                case "int":
                    Vars.Add(name, new VarPack{Var = new IntVar(), IsDefined = true});
                    AssignToInt(Vars[name].Var as IntVar, result);
                    break;

                case "double":
                    Vars.Add(name, new VarPack{Var = new DoubleVar(), IsDefined = true});
                    AssignToDouble(Vars[name].Var as DoubleVar, result);
                    break;

                case "bool":
                    Vars.Add(name, new VarPack{Var = new BoolVar(), IsDefined = true});
                    AssignToBool(Vars[name].Var as BoolVar, result);
                    break;

                case "string":
                    Vars.Add(name, new VarPack{Var = new StringVar(), IsDefined = true});
                    AssignToString(Vars[name].Var as StringVar, result);
                    break;

                case "block":
                    Vars.Add(name, new VarPack{Var = new BlockVar(), IsDefined = true});
                    AssignToBlock(Vars[name].Var as BlockVar, result);
                    break;
            }
        }

        

        public void CreateNewVariable(string typename, string name)
        {
            switch (typename)
            {
                case "int":
                    Vars.Add(name, new VarPack {IsDefined = false, Var = new IntVar()/* Type = VarTypeLabel.IntVar*/});
                    break;

                case "double":
                    Vars.Add(name, new VarPack {IsDefined = false, Var = new DoubleVar()/*, Type = VarTypeLabel.DoubleVar*/});
                    break;

                case "string":
                    Vars.Add(name, new VarPack {Var = new StringVar {Value = String.Empty}});
                    break;

                case "bool":
                    Vars.Add(name, new VarPack {IsDefined = false, Var = new BoolVar()/*, Type = VarTypeLabel.BoolVar*/});
                    break;

                case "block":
                    Vars.Add(name, new VarPack {IsDefined = false, Var = new BlockVar()/*, Type = VarTypeLabel.BlockVar*/});
                    break;
            }
        }
        #endregion


        #region Assigning
        public void Assign(string name, INode value)
        {
            if (!Vars.ContainsKey(name))
                throw new ASTException(string.Format("Использование необъявленной переменной {0}", name));

            var var = Vars[name];
            IVarType newValue;
            if (!(value is BlockNode))
                newValue = value.Evaluate();
            else
                newValue = (value as BlockNode).Value;

            if (!var.IsDefined)
                var.IsDefined = true;

            if (var.Var is IntVar)
                AssignToInt(var.Var as IntVar, newValue);
            else if (var.Var is DoubleVar)
                AssignToDouble(var.Var as DoubleVar, newValue);
            else if (var.Var is BoolVar)
                AssignToBool(var.Var as BoolVar, newValue);
            else if (var.Var is StringVar)
                AssignToString(var.Var as StringVar, newValue);
            else
                AssignToBlock(var.Var as BlockVar, newValue);
        }


        private static void AssignToInt(IntVar var, IVarType newValue)
        {
            if (newValue is IntVar)
                var.Value = (newValue as IntVar).Value;
            else if (newValue is DoubleVar)
                var.Value = TypeInferer.DoubleToInt(newValue as DoubleVar).Value;
            else if (newValue is BoolVar)
                var.Value = (newValue as BoolVar).Value ? 1 : 0;
            else if (newValue is StringVar)
            {
                int result;
                var s = (newValue as StringVar).Value;
                if (Int32.TryParse(s, out result))
                    var.Value = Int32.Parse(s);
                else 
                    throw new ASTException("Нельзя привести к типу int.");
            }
            else
                throw new ASTException("Нельзя привести к типу int.");
        }



        private static void AssignToDouble(DoubleVar var, IVarType newValue)
        {
            if (newValue is IntVar)
                var.Value = TypeInferer.IntToDouble(newValue as IntVar).Value;
            else if (newValue is DoubleVar)
                var.Value = (newValue as DoubleVar).Value;
            else if (newValue is BoolVar)
                var.Value = (newValue as BoolVar).Value ? 1 : 0;
            else if (newValue is StringVar)
            {
                double result;
                var s = (newValue as StringVar).Value;
                if (Double.TryParse(s, out result))
                    var.Value = Double.Parse(s);
                else
                    throw new ASTException("Нельзя привести к типу double.");
            }
            else
                throw new ASTException("Нельзя привести к типу double.");
        }


        private static void AssignToBool(BoolVar var, IVarType newValue)
        {
            if (newValue is IntVar)
                var.Value = (newValue as IntVar).Value != 0;
            else if (newValue is DoubleVar)
                throw new ASTException("Нельзя привести к типу bool.");
            else if (newValue is BoolVar)
                var.Value = (newValue as BoolVar).Value;
            else if (newValue is StringVar)
            {
                var s = (newValue as StringVar).Value.ToLower();
                switch (s)
                {
                    case "yep":
                        var.Value = true;
                        break;
                    case "nope":
                        var.Value = false;
                        break;
                    default:
                        throw new ASTException("Нельзя привести к типу bool.");
                }
            }
            else
                throw new ASTException("Нельзя привести к типу bool.");
        }


        private static void AssignToString(StringVar var, IVarType newValue)
        {
            var.Value = newValue.ToString();
        }


        private static void AssignToBlock(BlockVar var, IVarType newValue)
        {
            if (newValue is BlockVar)
            {
                var.Commands = (newValue as BlockVar).Commands;
            }
            else
                throw new ASTException("Нельзя привести к типу block.");
        }
        #endregion


        public bool ContainsVarWithName(string name)
        {
            return Vars.ContainsKey(name);
        }

        public bool IsBlockVar(string name)
        {
            return Vars.ContainsKey(name) && (Vars[name].Var is BlockVar);
        }

        public IVarType GetVar(string name)
        {
            if (_vars.ContainsKey(name))
                return _vars[name].IsDefined ? _vars[name].Var : new UndefinedVar();
            return null;
        }

        public string[] GetBlockVarNames()
        {
            return Vars.Keys.Where(key => Vars[key].Var is BlockVar).ToArray();
        }
    }
}