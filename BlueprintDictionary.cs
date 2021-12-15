using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace BlueprintRandomizer
{
    [Serializable]
    class BlueprintDictionary
    {
        public Dictionary<String, TechType> blueprintLookup = new Dictionary<String, TechType>();
        private bool initialized = false;

        public BlueprintDictionary() {}

        public BlueprintDictionary(List<Unlock> unlocks)
        {
            // Iniitalize the dictionary with all keys have TechType.None
            foreach (var u in unlocks)
            {
                this.Add(u.randomizerKey, TechType.None);
            }
        }

        public string ToBase64String()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, this);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static BlueprintDictionary FromBase64String(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return (BlueprintDictionary)(new BinaryFormatter().Deserialize(ms));
            }
        }

        public bool Add(String key, TechType unlock)
        {
            if (blueprintLookup.ContainsKey(key))
            {
                return false;
            }
            blueprintLookup.Add(key, unlock);
            return true;
        }

        public void AddOrUpdate(string key, TechType newUnlock)
        {
            if (this.blueprintLookup.ContainsKey(key))
            {
                this.blueprintLookup[key] = newUnlock;
            } 
            else
            {
                this.blueprintLookup.Add(key, newUnlock);
            }
        }

        public TechType LookupTechType(String key)
        {
            if (!this.initialized || !this.blueprintLookup.ContainsKey(key))
            {
                return TechType.None;
            }

            return this.blueprintLookup[key];
        }

        public void SetInitialized(bool initialized)
        {
            this.initialized = initialized;
        }
    }

}

