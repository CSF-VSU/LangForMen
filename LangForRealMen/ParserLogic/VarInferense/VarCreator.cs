using System;
using System.Collections.Generic;
using LangForRealMen.AST;

namespace LangForRealMen.ParserLogic.VarInferense
{
    public class VarCreator 
    {
        #region Warehouse
        private readonly Dictionary<string, DoubleVar> _doubleValues;
        public Dictionary<string, DoubleVar> Doubles {
            get
            {
                return _doubleValues;
            } 
        }

        private readonly Dictionary<string, IntVar> _intValues;
        public  Dictionary<string, IntVar> Ints
        {
            get
            {
                return _intValues;
            }
        }

        private readonly Dictionary<string, BoolVar> _boolValues;
        public Dictionary<string, BoolVar> Bools
        {
            get
            {
                return _boolValues;
            }
        }

        private readonly Dictionary<string, StringVar> _stringValues;
        public Dictionary<string, StringVar> Strings
        {
            get
            {
                return _stringValues;
            }
        }

        private readonly Dictionary<string, CharVar> _charValues;
        public Dictionary<string, CharVar> Chars
        {
            get
            {
                return _charValues;
            }
        }
        #endregion
        
        public VarCreator()
        {
            _intValues = new Dictionary<string, IntVar>();
            _doubleValues = new Dictionary<string, DoubleVar>();
            _stringValues = new Dictionary<string, StringVar>();
            _charValues = new Dictionary<string, CharVar>();
            _boolValues = new Dictionary<string, BoolVar>();
        }

        #region Creating Variables

        public void CreateNewVariable(string typename, string name, INode value)
        {
            var result = value.Evaluate();

            switch (typename)
            {
                case "int":
                    if (TypeInferer.IsInt(result))
                        _intValues.Add(name, result as IntVar);
                    else if (TypeInferer.IsDouble(result))
                        _intValues.Add(name, TypeInferer.DoubleToInt(result as DoubleVar));
                    else
                        throw new ASTException("Невозможно привести к типу int.");
                    break;

                case "double":
                    if (TypeInferer.IsDouble(result))
                        _doubleValues.Add(name, result as DoubleVar);
                    else if (TypeInferer.IsInt(result))
                        _doubleValues.Add(name, TypeInferer.IntToDouble(result as IntVar));
                    else
                        throw new ASTException("Невозможно привести к типу double.");
                    break;

                case "string":
                    //_stringValues.Add(name, value.Evaluate().ToString());
                    break;
            }
        }

        public void CreateNewVariable(string typename, string name)
        {
            switch (typename)
            {
                case "int":
                    _intValues.Add(name, new IntVar {IsDefined = false, Value = 0});
                    break;

                case "double":
                    _doubleValues.Add(name, new DoubleVar { IsDefined = false, Value = 0d });
                    break;

                case "string":
                    _stringValues.Add(name, new StringVar{Value = String.Empty});
                    break;
            }
        }

        #endregion

        public bool ContainsVarWithName(string name)
        {
            return _doubleValues.ContainsKey(name) || _intValues.ContainsKey(name) || _stringValues.ContainsKey(name);
        }


        public IVarType GetVar(string name)
        {
            if (_intValues.ContainsKey(name))
            {
                return _intValues[name];
            }
            
            if (_doubleValues.ContainsKey(name))
            {
                return _doubleValues[name];
            }

            if (_boolValues.ContainsKey(name))
            {
                return _boolValues[name];
            }

            if (_charValues.ContainsKey(name))
            {
                return _charValues[name];
            }

            return _stringValues.ContainsKey(name) ? _stringValues[name] : null;
        }
    }
}
