﻿using System;
using System.Collections.Generic;
using System.Text;
using Validator.World;

namespace Validator
{
    public interface IFormulaValidate
    {
        Result<EValidationResult> Validate(IWorldPL1Structure pL1Structure, Dictionary<string, string> dictVariables);
    }
}
