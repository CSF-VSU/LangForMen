using System;
using System.Collections.Generic;
using LangForRealMen.AST;

namespace LangForRealMen.ParserLogic
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

        private readonly Dictionary<string, string> _stringValues;
        public Dictionary<string, string> Strings
        {
            get
            {
                return _stringValues;
            }
        }
        #endregion
        
        public VarCreator()
        {
            _intValues = new Dictionary<string, IntVar>();
            _doubleValues = new Dictionary<string, DoubleVar>();
            _stringValues = new Dictionary<string, string>();
        }

        public void CreateNewVariable(string typename, string name, INode value)
        {
            switch (typename)
            {
                case "int":
                    _intValues.Add(name, new IntVar {IsDefined = true, Value = (int) value.Evaluate()});
                    break;
                case "double":
                    _doubleValues.Add(name, new DoubleVar {IsDefined = true, Value = value.Evaluate()});
                    break;
                case "string":
                    _stringValues.Add(name, value.Evaluate().ToString());
                    break;
            }
        }

        public void CreateNewVariable(string typename, string name)
        {
            switch (typename)
            {
                case "int":
                    _intValues.Add(name, new IntVar { IsDefined = false, Value = 0 });
                    break;
                case "double":
                    _doubleValues.Add(name, new DoubleVar { IsDefined = false, Value = 0d });
                    break;
                case "string":
                    _stringValues.Add(name, String.Empty);
                    break;
            }
        }


        public bool ContainsVarWithName(string name)
        {
            return _doubleValues.ContainsKey(name) || _intValues.ContainsKey(name) || _stringValues.ContainsKey(name);
        }


        public object GetVar(string name, out Type type)
        {
            if (_intValues.ContainsKey(name))
            {
                type = typeof(Int32);
                return _intValues[name].Value;
            }
            
            if (_doubleValues.ContainsKey(name))
            {
                type = typeof(Double);
                return _doubleValues[name].Value;
            }

            type = typeof(String);
            return _stringValues[name];
        }
    }
}
