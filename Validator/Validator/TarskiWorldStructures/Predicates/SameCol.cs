﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    internal class SameCol : IPredicateValidation
    {
        public bool Check(List<WorldObject> obj)
        {
            if (obj.Count != 2)
            {
                throw new Exception("Invalid input parameter: " + nameof(SameCol));
            }
            else
            {
                bool result = false;
                WorldObject obj1 = obj[0];
                WorldObject obj2 = obj[1];

                var pos1 = obj1.TryGetPosition();
                var pos2 = obj2.TryGetPosition();

                if (pos1.IsValid && pos2.IsValid && pos1.Value.X == pos2.Value.X)
                    result = true;

                return result;
            }
        }
    }
}
