﻿using System;
using System.Linq;

namespace LangForRealMen.ParserLogic.VarInferense
{
    public static class TypeInferer
    {
        #region Defining Types
        public static bool IsSameType(IVarType var1, IVarType var2)
        {
            return var1.GetType() == var2.GetType();
        }

        public static bool IsNumeric(params IVarType[] vars)
        {
            return vars.Aggregate(true, (current, varType) => current && (varType is IntVar || varType is DoubleVar));
        }

        public static bool IsInt(params IVarType[] vars)
        {
            return vars.Aggregate(true, (current, type) => current && (type is IntVar));
        }

        public static bool IsDouble(params IVarType[] vars)
        {
            return vars.Aggregate(true, (current, type) => current && (type is DoubleVar));
        }

        public static bool IsBool(params IVarType[] vars)
        {
            return vars.Aggregate(true, (current, type) => current && (type is BoolVar));
        }

        public static bool IsString(params IVarType[] vars)
        {
            return vars.Aggregate(true, (current, type) => current && (type is StringVar));
        }
        #endregion

        #region Converting

        public static DoubleVar IntToDouble(IntVar var)
        {
            return new DoubleVar
            {
                Value = var.Value
            };
        }

        public static IntVar DoubleToInt(DoubleVar var)
        {
            return new IntVar
            {
                Value = (int) var.Value
            };
        }
        #endregion

        public static double GetNumericValue(IVarType var, out bool isInteger)
        {
            if (var is IntVar)
            {
                isInteger = true;
                return (var as IntVar).Value;
            }
            if (var is DoubleVar)
            {
                isInteger = false;
                return (var as DoubleVar).Value;
            }
            throw new Exception("Не является числовым типом.");
        }

    }
}
