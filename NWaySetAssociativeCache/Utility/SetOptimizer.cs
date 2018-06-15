using NWaySetAssociativeCache.Cache;
using NWaySetAssociativeCache.Memory;
using NWaySetAssociativeCache.ReplacementPolicies;
using System;
using System.Collections.Generic;

namespace NWaySetAssociativeCache.Utility
{
    /**
     * Generic Static SetOptimizer utility class, used to calculate the N-way with the lowest miss rate, given a set of sample entries and operations.
     **/
    /// <include file='documentation.xml' path='docs/members[@name="Utility"]/SetOptimizer/Class/*'/>
    public static class SetOptimizer<TKey,TValue>
    {
        #region GetLowestMissN Method
        /**
         * GetLowestMissN Method
         * Parametrized by the generic TKey,TValue, which are set at runtime.
         * Inputs: cacheSize, test entries (keys,values, and which operations to call on them), and a replacementPolicy.
         * Outputs: int optimalN, which corresponds to the N-way with the lowest miss rate.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Utility"]/SetOptimizer/GetLowestMissN/*'/>
        public static int GetLowestMissN(int cacheSize, Tuple<TKey, TValue,NWaySetAssociativeCache<TKey,TValue>.CacheOperation>[] testEntries, ReplacementPolicy<TKey,TValue> replacementPolicy) {
            //The N-way with the lowest miss rate
            int optimalN = 1;
            //Int keeping track of the minimum amount of misses
            int minCount = -1;
            //Loop, which initializes a cache for each N value from 1 to the cache size.
            for (int i = 1; i <= cacheSize; i++) {
                //Initialize the MemoryAccess subclass to keep track of misses.
                MemoryNode<TKey, TValue> memory = new MemoryNode<TKey, TValue>();
                NWaySetAssociativeCache<TKey, TValue> cache;
                cache = new NWaySetAssociativeCache<TKey, TValue>(memory, replacementPolicy, cacheSize, i, true);
                //For each tuple (key,value,operation), call the InvokeReflected method of the cache, which will use the defined operation upon the key-value pair.
                foreach (Tuple<TKey,TValue, NWaySetAssociativeCache<TKey, TValue>.CacheOperation> entry in testEntries) {
                    cache.InvokeReflected(entry);
                }
                //If minCount has not been changed, set it to the first miss rate value.
                if (minCount == -1) { minCount = memory.getCount; optimalN = i; }
                //Else if the current miss rate is lower than the lowest miss rate, set the lowest miss rate to the current one.
                else if (memory.getCount < minCount) { minCount = memory.getCount; optimalN = i; }
            }
            return optimalN;
        }
        #endregion
    }
    #region MemoryAccess subclass implementation
    /**
     * Internal Generic MemoryNode class, a subclass of MemoryAccess, used to keep track of the miss rate of a given cache instance, using the getCount variable.
     **/
    class MemoryNode<TKey, TValue> : MemoryAccess<TKey, TValue>
    {
        //Dictionary of generic key value pairs.
        private Dictionary<TKey, TValue> memory = new Dictionary<TKey, TValue>();
        //Total count of the times the Get method has been called.
        public int getCount { private set; get; } = 0;

        //Clear the whole Dictionary.
        public override void Clear()
        {
            memory.Clear();
        }

        //Get a generic value, given a generic key, from the Dictionary.
        public override TValue Get(TKey key)
        {
            getCount++;
            return memory.ContainsKey(key) ? memory[key] : default(TValue);
        }

        //Put a generic key-value pair into the Dictionary.
        public override void Put(TKey key, TValue value)
        {
            if (memory.ContainsKey(key)) memory.Remove(key);
            memory.Add(key, value);
        }
        //Remove a generic entry from the Dictionary, given a generic key.
        public override TValue Remove(TKey key)
        {
            TValue returnValue = memory.ContainsKey(key) ? memory[key] : default(TValue);
            if (memory.ContainsKey(key)) memory.Remove(key);
            return returnValue;
        }
        //Verify, whether the Dictionary contains a given generic key.
        public bool Contains(TKey key)
        {
            return memory.ContainsKey(key);
        }
    }
    #endregion

}
