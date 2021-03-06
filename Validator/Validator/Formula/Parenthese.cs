﻿using System.Collections.Generic;
using Validator.World;

namespace Validator
{
    public class Parenthese : Formula, IFormulaValidate
    {
        private Formula _formula = null;

        public Parenthese(Formula formula) : base(formula.Name, formula.RawFormula)
        {
            _formula = formula;
        }

        public Result<EValidationResult> Validate(IWorldPL1Structure pL1Structure, Dictionary<string, string> dictVariables)
        {
            if (_formula != null && _formula is IFormulaValidate formulaValidate)
            {
                return formulaValidate.Validate(pL1Structure, dictVariables);
            }

            return Result<EValidationResult>.CreateResult(false, EValidationResult.UnexpectedResult, "No Formula in Parenthese");
        }
    }
}
