using System;
using System.Collections.Generic;
using System.Linq;
using BlueprintRandomizer.Data;

namespace BlueprintRandomizer.Logic
{
    class BasicLogic
    {
        private readonly Random random;
        private Config config;
        private List<Blueprint> blueprints;
        private List<Unlock> unlocks;

        public BasicLogic(Config config, List<Blueprint> blueprints, List<Unlock> unlocks) 
        {
            this.config = config;
            this.blueprints = blueprints;
            this.unlocks = unlocks;

            if (String.IsNullOrEmpty(config.Seed))
            {
                random = new System.Random();
            }
            else
            {
                random = new System.Random(config.Seed.GetHashCode());
            }
        }

        public BlueprintDictionary Generate()
        {
            var openMode = config.UseAllAnalysis || config.UseAllScanner || config.UseAllGoals;
            if (openMode)
            {
                return GenerateOpenMode();
            }
            else
            {
                return GenerateNormalMode();
            }
        }

        private BlueprintDictionary GenerateOpenMode()
        {
            BlueprintDictionary dict;
            List<TechType> possibleTech = GetCandidateTech();

            if (this.config.RandomStartingBlueprints)
            {
                dict = new BlueprintDictionary(this.unlocks);
                // If starting blueprints are random, assign them here before anything else.
                List<Unlock> startingUnlocks = GetDefaultUnlocks();
                foreach (Unlock u in startingUnlocks)
                {
                    var nextTechIdx = GetValidTechIndex(u, possibleTech);

                    dict.AddOrUpdate(u.randomizerKey, possibleTech[nextTechIdx]);

                    possibleTech.RemoveAt(nextTechIdx);
                }
            }
            else
            {
                dict = new BlueprintDictionary(ClearDefaults());
            }

            // In open mode, choose from all available unlocks at random until there are no possible tech to assign.
            // Defaults are always assigned, so don't include them in this list.
            var allUnlocks = unlocks.Where((u) => IsIncluded(u, false)).ToList();

            while(possibleTech.Count > 0)
            {
                // First, choose an unlock at random
                var keyIndex = this.random.Next(0, allUnlocks.Count);
                var nextUnlock = allUnlocks[keyIndex];

                var nextTechIdx = GetValidTechIndex(nextUnlock, possibleTech);

                dict.AddOrUpdate(nextUnlock.randomizerKey, possibleTech[nextTechIdx]);

                allUnlocks.RemoveAt(keyIndex);
                possibleTech.RemoveAt(nextTechIdx);
            }

            dict.SetInitialized(true);

            return dict;
        }

        private BlueprintDictionary GenerateNormalMode()
        {
            BlueprintDictionary dict;

            if (config.RandomStartingBlueprints)
            {
                dict = new BlueprintDictionary(this.unlocks);
            }
            else
            {
                dict = new BlueprintDictionary(ClearDefaults());
            }

            List<TechType> possibleTech = GetCandidateTech();
            List<Unlock> restrictedUnlocks = GetRestrictedCandidateKeys(config.RandomStartingBlueprints);
            List<Unlock> openUnlocks = GetOpenCandidateKeys(config.RandomStartingBlueprints);

            // To maximize the chance of a valid randomizer, handle the restricted unlocks first
            while (restrictedUnlocks.Count > 0)
            {
                // First, choose an unlock at random
                var keyIndex = this.random.Next(0, restrictedUnlocks.Count);
                var nextUnlock = restrictedUnlocks[keyIndex];

                var nextTechIdx = GetValidTechIndex(nextUnlock, possibleTech);

                // Check for a softlock. If one is found, reject this pairing and generate another
                if (!CheckSoftlock(dict, nextUnlock, possibleTech[nextTechIdx]) || config.AllowSoftlocks)
                {
                    dict.AddOrUpdate(nextUnlock.randomizerKey, possibleTech[nextTechIdx]);
                    restrictedUnlocks.RemoveAt(keyIndex);
                    possibleTech.RemoveAt(nextTechIdx);
                }
            }

            // Once all conditional options are used up, handle the remaining tech
            while (possibleTech.Count > 0)
            {
                // First, choose an unlock at random
                var keyIndex = this.random.Next(0, openUnlocks.Count);
                var nextUnlock = openUnlocks[keyIndex];

                var nextTechIdx = GetValidTechIndex(nextUnlock, possibleTech);

                dict.AddOrUpdate(nextUnlock.randomizerKey, possibleTech[nextTechIdx]);

                openUnlocks.RemoveAt(keyIndex);
                possibleTech.RemoveAt(nextTechIdx);
            }

            dict.SetInitialized(true);

            return dict;
        }

        private List<Unlock> GetOpenCandidateKeys(bool includeDefaults)
        {
            return unlocks.Where((u) => (u.invalidTechTypes.Count == 0 && IsIncluded(u, includeDefaults))).ToList();
        }

        private List<Unlock> GetRestrictedCandidateKeys(bool includeDefaults)
        {
            return unlocks.Where((u) => (u.invalidTechTypes.Count > 0 && IsIncluded(u, includeDefaults))).ToList();
        }

        private List<Unlock> ClearDefaults()
        {
            return unlocks.Where((u) => (!u.randomizerKey.StartsWith("Default|"))).ToList();
        }

        private List<Unlock> GetDefaultUnlocks()
        {
            return unlocks.Where((u) => (u.randomizerKey.StartsWith("Default|"))).ToList();
        }

        private List<TechType> GetCandidateTech()
        {
            return blueprints.Where((b) => (!b.startingTech || config.RandomStartingBlueprints)).Select(b => b.type).ToList();
        }
        
        private bool IsIncluded(Unlock u, bool includeDefaults)
        {
            string type = u.randomizerKey.Split('|')[0];
            switch(type)
            {
                case "Scanner": return u.standardSet || config.UseAllScanner;
                case "Analysis": return u.standardSet || config.UseAllAnalysis;
                case "Goal": return u.standardSet || config.UseAllGoals;
                case "Default": return includeDefaults;
                default: return true;
            }
        }

        private int GetValidTechIndex(Unlock unlock, List<TechType> possibleTechTypes)
        {
            // make 1000 attempts to find a valid before throwing an exception
            for (int i = 0; i < 1000; i++)
            {
                var candidateIdx = random.Next(0, possibleTechTypes.Count);
                if (!unlock.invalidTechTypes.Contains(possibleTechTypes[candidateIdx]) || config.AllowSoftlocks)
                {
                    return candidateIdx;
                }
            }

            throw new Exception("Unable to find a valid tech type for unlock after 1000 tries: " + unlock.randomizerKey);
        }

        private bool CheckSoftlock(BlueprintDictionary dict, Unlock nextUnlock, TechType candidateTech)
        {
            // situation to avoid:
            // Analysis|Cyclops -> RocketStage1
            // Rocket|2 -> CyclopsHullBlueprint

            // Analysis|Exosuit -> Seamoth
            // Analysis|Seamoth -> Exosuit

            // Goal|UnlockHatchingEnzymes -> RocketBase
            // Rocket|1 -> HatchingEnzymes

            // To check: If the candidate tech (as a string) is already in the dict, and the entry is for 
            // the nextUnlock in any form, reject.
            var candidateStr = candidateTech.AsString();
            if (candidateStr.StartsWith("Rocket"))
            {
                candidateStr = "Rocket";
            }
            else if (candidateStr.StartsWith("Cyclops")) // Overkill, but should be fine.
            {
                candidateStr = "Cyclops";
            }

            foreach (string key in dict.blueprintLookup.Keys)
            {
                if (key.Contains(candidateStr)) {
                    var otherStr = dict.blueprintLookup[key].AsString();
                    if (otherStr.StartsWith("Rocket"))
                    {
                        otherStr = "Rocket";
                    } else if (otherStr.StartsWith("Cyclops"))
                    {
                        otherStr = "Cyclops";
                    }

                    if (nextUnlock.randomizerKey.Contains(otherStr))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

}
