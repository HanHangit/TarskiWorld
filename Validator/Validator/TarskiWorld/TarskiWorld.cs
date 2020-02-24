﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validator
{
    public class TarskiWorld : IWorldPL1Structure, IWorldSignature
    {
        private Dictionary<string, IFunctionValidation> _funcValidation = new Dictionary<string, IFunctionValidation>
        {
            { TarskiWorldDataFields.FRONTOF, new FrontOf() }
        };

        private PL1Structure _pl1Structure = new PL1Structure();
        private Signature _signature = new Signature();

        private Dictionary<string, IPredicateValidation> _predValidation = new Dictionary<string, IPredicateValidation>
        {
            { TarskiWorldDataFields.BIGGER, new Bigger() },
            { TarskiWorldDataFields.SMALLER, new Smaller() }
        };


        public TarskiWorld()
        {
            _signature = CreateTarskiWorld();
        }

        internal IEnumerable<List<string>> AllConstCombinations(List<string> worldObjects, int length)
        {
            if (length > 1)
                foreach (var objList in AllConstCombinations(worldObjects, length - 1))
                    foreach (var obj in worldObjects)
                    {
                        List<string> result = new List<string>() { obj };
                        result.AddRange(objList);

                        yield return result;
                    }
            else
                foreach (var obj in worldObjects)
                    yield return new List<string> { obj };
        }

        internal IEnumerable<List<string>> AllConstCombinations(List<WorldObject> worldObjects)
        {
            foreach (var con in worldObjects.First().Consts)
            {
                if (worldObjects.Count > 1)
                {
                    foreach (var conList in AllConstCombinations(worldObjects.Skip(1).ToList()))
                    {
                        List<string> result = new List<string>();
                        result.Add(con);
                        result.AddRange(conList);
                        yield return result;
                    }
                }
                else
                    yield return new List<string> { con };
            }
        }

        internal IEnumerable<List<WorldObject>> AllWorldObjectsCombinations(List<WorldObject> worldObjects, int length)
        {
            if (length > 1)
                foreach (var objList in AllWorldObjectsCombinations(worldObjects, length - 1))
                    foreach (var obj in worldObjects)
                    {
                        List<WorldObject> result = new List<WorldObject>() { obj };
                        result.AddRange(objList);

                        yield return result;
                    }
            else
                foreach (var obj in worldObjects)
                    yield return new List<WorldObject> { obj };
        }

        private Signature CreateTarskiWorld()
        {
            List<string> consts = new List<string>()
            {
                "a",
                "b",
                "c",
                "d",
                "e",
                "f"
            };

            List<(string, int)> predicates = new List<(string, int)>
            {
                (TarskiWorldDataFields.TET, 1),
                (TarskiWorldDataFields.CUBE, 1),
                (TarskiWorldDataFields.SMALL, 1),
                (TarskiWorldDataFields.BIG, 1),
                (TarskiWorldDataFields.BIGGER, 2),
                (TarskiWorldDataFields.SMALLER, 2),
                (TarskiWorldDataFields.FRONTOF, 2),
                (TarskiWorldDataFields.INBETWEEN, 3)
            };

            List<(string, int)> functions = new List<(string, int)>
            {
                (TarskiWorldDataFields.FRONTOF, 1)
            };

            return new Signature(consts, predicates, functions);
        }


        public PL1Structure GetPl1Structure()
        {
            return _pl1Structure;
        }

        public Signature GetSignature()
        {
            return _signature;
        }

        public WorldResult<bool> Check(WorldParameter parameter)
        {
            //--Initialize consts and static predicates--//
            foreach (var data in parameter.Data)
            {
                string keyUniverseIdentifier = _pl1Structure.AddConst();
                foreach (var pred in data.Predicates)
                {
                    if (_signature.Predicates.Any(s => s.Item1 == pred))
                    {
                        foreach (var con in data.Consts)
                        {
                            if (_signature.Consts.Contains(con))
                            {
                                _pl1Structure.AddConst(con, keyUniverseIdentifier);
                            }
                            _pl1Structure.AddPredicate(pred, new List<string> { con });
                        }
                    }
                }
            }

            //--Calc <dynamic> predicates--//
            foreach (var pred in _signature.Predicates)
            {
                if (_predValidation.ContainsKey(pred.Item1))
                {
                    IPredicateValidation predValidation = _predValidation[pred.Item1];
                    IEnumerable<List<WorldObject>> worldObjects = AllWorldObjectsCombinations(parameter.Data, pred.Item2);

                    foreach (var objects in worldObjects)
                    {
                        if (predValidation.Check(objects))
                        {
                            IEnumerable<List<string>> consts = AllConstCombinations(objects);
                            foreach (var constList in consts)
                            {
                                _pl1Structure.AddPredicate(pred.Item1, constList);
                            }
                        }
                    }
                }
            }

            //--Calc functions--//
            foreach (var func in _signature.Functions)
            {
                if (_funcValidation.ContainsKey(func.Item1))
                {
                    IFunctionValidation funcValidation = _funcValidation[func.Item1];
                    IEnumerable<IEnumerable<string>> allConsts = AllConstCombinations(parameter.Data.Where(s => s.Consts.Count > 0).Select(s => s.Consts.First()).ToList(), func.Item2);
                    foreach (var objects in allConsts)
                    {
                        List<WorldObject> inputObjects = new List<WorldObject>();
                        foreach (var con in objects)
                            inputObjects.Add(parameter.Data.Find(x => x.Consts.Contains(con)));

                        WorldObject result = funcValidation.Check(inputObjects.ToList(), parameter.Data);
                        _pl1Structure.AddFunctions(func.Item1, objects.ToList(), result.Consts.First());
                    }
                }
            }

            return new WorldResult<bool>();
        }
    }
}
