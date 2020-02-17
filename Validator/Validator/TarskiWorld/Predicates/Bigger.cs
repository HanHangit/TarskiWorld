﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    internal class Bigger : IPredicateValidation
    {
        private bool CheckBiggerPredicate(WorldObject o1, WorldObject o2, string p1, string p2)
        {
            return o1.Predicates.Contains(p1) && o2.Predicates.Contains(p2);
        }


        public bool Check(List<WorldObject> obj)
        {
            if (obj.Count != 2)
            {
                return false;
            }
            else
            {
                WorldObject obj1 = obj[0];
                WorldObject obj2 = obj[1];
                
                if(CheckBiggerPredicate(obj1, obj2, TarskiWorldDataFields.BIG, TarskiWorldDataFields.MEDIUM)||
                    CheckBiggerPredicate(obj1, obj2, TarskiWorldDataFields.BIG, TarskiWorldDataFields.SMALL) ||
                    CheckBiggerPredicate(obj1, obj2, TarskiWorldDataFields.MEDIUM, TarskiWorldDataFields.SMALL))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}