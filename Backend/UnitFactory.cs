﻿using Bsc_In_Stream_Conversion.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Bsc_In_Stream_Conversion.Lookup.Prefixes;

namespace Bsc_In_Stream_Conversion
{
    public class UnitFactory
    {
        private DatabaseAccess db;

        public UnitFactory(DatabaseAccess db)
        {
            this.db = db;
        }

        public async Task<UserUnit> Parse(string input)
        {
            var uu = new UserUnit();
            input = input.Replace(" ", "").Replace("\t", "").ToUpperInvariant();

            uu = await ParseFractionPart(input.Split("/")[0], uu, true);
            if (input.Contains("/"))
            {
                uu = await ParseFractionPart(input.Split("/")[1], uu, false);
            }
            return uu;
        }

        private async Task<UserUnit> ParseFractionPart(string input, UserUnit uu, bool isNumerator)
        {
            var parts = input.Split("*");
            foreach (var part in parts)
            {
                bool hasPrefix;
                (string, ConversionFactor)? prefixFactor;
                FindPrefix(part, out hasPrefix, out prefixFactor);

                if (part.Contains("^"))
                {
                    return await SeperatePowers(uu, part, hasPrefix, prefixFactor, isNumerator);
                }
                else
                {
                    string partToAdd = part;
                    if (hasPrefix)
                    {
                        partToAdd = part.Replace(prefixFactor?.Item1, "");
                    }
                    var dbUnit = await db.SelectUnit(partToAdd);

                    UserUnit newUU = new UserUnit(numeratorPrefixes:    hasPrefix ? (prefixFactor?.Item2) : ConversionFactor.One,
                                                  denominatorPrefixes:  ConversionFactor.One,
                                                  multiplier:           dbUnit.ConversionMultiplier,
                                                  offSet:               dbUnit.ConversionOffset);

                    if (isNumerator)
                    {
                        uu *= newUU;
                    }
                    else
                    {
                        uu /= newUU;
                    }
                    return uu;
                }
            }
            throw new InvalidOperationException("Something went wrong in passing the unit");
        }

        private async Task<UserUnit> SeperatePowers(UserUnit uu, string part, bool hasPrefix, (string, ConversionFactor)? prefixFactor, bool isNumerator)
        {
            var splitPart = part.Split("^");
            var amount = int.Parse(splitPart[1]);
            if (hasPrefix)
            {
                splitPart[0] = splitPart[0].Replace(prefixFactor?.Item1, "");
            }
            for (int i = 0; i < amount; i++)
            {
                string partToAdd = splitPart[0];
                if (hasPrefix)
                {
                    partToAdd = part.Replace(prefixFactor?.Item1, "");
                }
                var dbUnit = await db.SelectUnit(partToAdd);

                UserUnit newUU = new UserUnit(numeratorPrefixes: hasPrefix ? (prefixFactor?.Item2) : ConversionFactor.One,
                                              denominatorPrefixes: ConversionFactor.One,
                                              multiplier: dbUnit.ConversionMultiplier,
                                              offSet: dbUnit.ConversionOffset);

                if (isNumerator)
                {
                    uu *= newUU;
                }
                else
                {
                    uu /= newUU;
                }
            }
            return uu;
            throw new InvalidOperationException("Something went wrong in passing the unit");
        }

        private void FindPrefix(string part, out bool hasPrefix, out (string, ConversionFactor)? prefixFactor)
        {
            hasPrefix = false;
            prefixFactor = null;
            foreach (var prefix in Prefixes.IntegerFactor.Keys)
            {
                if (part.StartsWith(prefix))
                {
                    hasPrefix = true;
                    prefixFactor = (prefix, Prefixes.IntegerFactor[prefix]);
                }
            }
        }
    }
}