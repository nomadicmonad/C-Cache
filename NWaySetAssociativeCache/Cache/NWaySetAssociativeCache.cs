using System;
using System.Collections.Generic;
using NWaySetAssociativeCache.Exceptions;
using NWaySetAssociativeCache.Memory;
using NWaySetAssociativeCache.ReplacementPolicies;

namespace NWaySetAssociativeCache.Cache
{
    /**
     * Generic NWaySetAssociativeCache class, which stores generic key-value pairs of any type.
     * The class enforces Type-safety through generics.
     * The class supports cache WriteBack and WriteThrough policies, mediated through a subclass of MemoryAccess.
     * The class decides its replacement policy in the case of a cache miss using the getReplacementIndex method of the constructor supplied subclass of ReplacementPolicy.
     **/
    /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/Class/*'/>
    public class NWaySetAssociativeCache<TKey, TValue>
    {
        #region global variables
        //Generic keys to be stored.
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/keys/*'/>
        public TKey[] keys { private set; get; }
        //Generic values to be stored.
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/values/*'/>
        public TValue[] values { private set; get; }
        //Array keeping track of entry modification, similar to a 'dirty bit'.
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/isModified/*'/>
        public bool[] isModified { private set; get; }
        //Size of the cache in terms of total lines.
        private int cacheSize;
        //Number of sets into which the cache is divided.
        private int nWay;
        //Number of cache lines in each set.
        private int lineNumber;
        //HashCode of a given key, used to specify the set into which it belongs.
        private int hashCode;
        //The starting index of a given set in the cache.
        private int setIndex;
        //The first index of a given set, which is null (relative index).
        private int firstNullIndex = -1;
        //The number of a given set.
        private int set;
        //The index, at which an entry is to be inserted.
        private int insertionIndex;
        //The index, at which a given entry has been found.
        private int entryIndex;
        //The generic MemoryAccess subclass, through which the cache communicates with the memory.
        private MemoryAccess<TKey, TValue> memoryAccess;
        //The generic ReplacementPolicy subclass, through which the replacement policy is retrieved.
        private ReplacementPolicy<TKey, TValue> replacementPolicy;
        //A previous generic value in the cache.
        private TValue oldValue;
        //A list of sets, each of which contains a list of ordered access history of cache entries.
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/accessList/*'/>
        public List<List<int>> accessList { private set; get; }
        //The policy, of whether data is written to memory through the cache, or in conjunction to cache writes.
        private bool isWriteBack;
        //An enum listing the possible cache operations.
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/CacheOperation/*'/>
        public enum CacheOperation {Put, Get, Remove, PutAll, Clear, DeepClear, ContainsKey};
        #endregion

        #region NWaySetAssociativeCache constructor
        /**
         * Constructor of the generic NWaySetAssociativeCache.
         * Requires a MemoryAccess subclass instance, a ReplacementPolicy subclass instance, the cache size, number of sets, and writing policy.
         * Throws an exception in case of null values being passed to the constructor, or the cache and set parameters being out of bounds.
         * Note: the cache size is treated as a suggestion, and can expand by (b - 1 - ((a-1)%b))
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/Constructor/*'/>
        public NWaySetAssociativeCache(MemoryAccess<TKey, TValue> memoryAccess, ReplacementPolicy<TKey, TValue> replacementPolicy, int cacheSize, int nWay, bool isWriteBack)
        {
            //Non-sensical parameters: throw Exception
            if (memoryAccess == null || replacementPolicy == null || cacheSize < 1 || nWay > cacheSize || nWay < 1) {
                throw new ConstructorParameterException();
            }
            this.memoryAccess = memoryAccess;
            this.replacementPolicy = replacementPolicy;
            //Calculate the number of lines per set. This might slightly increase the cache size (by (b - 1 - ((a-1)%b))).
            lineNumber = (int)Math.Ceiling(cacheSize / ((double)nWay));
            this.nWay = nWay;
            this.cacheSize = lineNumber * nWay;
            this.isWriteBack = isWriteBack;
            isModified = new bool[this.cacheSize];
            keys = new TKey[this.cacheSize];
            values = new TValue[this.cacheSize];
            accessList = new List<List<int>>();
            //Add a new List of ints for each set in the cache
            for (int i = 0; i < nWay; i++)
            {
                accessList.Add(new List<int>());
            }
        }
        #endregion

        #region Put method
        /**
         * Put a generic key-value entry into the cache
         * Returns a generic TValue of an old entry if Write-Through is used and a cache entry is replaced.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/Put/*'/>
        public TValue Put(TKey key, TValue value)
        {
            //To distinguish empty cache entries from geuine ones: check whether the key uses the default value of TKey, and if so, throw an exception.
            if (EqualityComparer<TKey>.Default.Equals(key,default(TKey))) {
                throw new KeyValueUsesDefaultValueException<TKey>();
            }
            //Set the old value to the default.
            oldValue = default(TValue);
            //Calculate the hash for the key, and which set it belongs to based on that hash.
            RefreshIndexes(key);
            insertionIndex = -1;
            //Check whether the key exists in the cache, and return its index if it does (or -1 otherwise).
            entryIndex = SearchIfExists(key);
            /**
             * If the key was found in the cache, check whether its value is different, and record whether it has been modified.
             * Then record where the current value should be inserted based on where the previous entry was.
             **/ 
            if (entryIndex != -1)
            {
                isModified[entryIndex] = !EqualityComparer<TValue>.Default.Equals(values[entryIndex],value);
                insertionIndex = entryIndex;
            }
            /**
             * If the key was not found in the cache, yet there are empty entries in the cache, set the modification record to true,
             * record the insertion point as where the first empty line occurs, and insert the key into that point.
             **/
            else if (entryIndex == -1 && firstNullIndex != -1)
            {
                isModified[firstNullIndex] = true;
                insertionIndex = firstNullIndex;
                keys[insertionIndex] = key;
            }
            /**
             * If the key was not found in the cache, and the given set is at full capacity, calculate a line to be replaced using the replacement policy.
             * If the replaced entry has been tagged as modified, then depending on the write policy of the cache either write the entry into memory or pass back
             * the replaced value as a return value.
             * The record is then removed from the access history of the set.
             **/
            else
            {
                //Find the index of the entry to be replaced.
                insertionIndex = replacementPolicy.GetReplacementIndex(lineNumber,setIndex / lineNumber, accessList, keys, values, isModified);
                //Check whether the replaced entry had been modified.
                if (isModified[insertionIndex])
                {
                    oldValue = values[insertionIndex];
                    accessList[setIndex / lineNumber].Remove(hashCode);
                    accessList[setIndex / lineNumber].Add(hashCode);
                    if (isWriteBack)
                    {
                        memoryAccess.Put(key, oldValue);
                    }
                }
                isModified[insertionIndex] = true;
                keys[insertionIndex] = key;
            }
            //Insert the current value in the value array based on the insertion index calculated above
            values[insertionIndex] = value;
            //Inform the replacement policy that this method was invoked.
            replacementPolicy.RefreshPolicy(CacheOperation.Put);
            //Return the old value, which is non-default if write-through is used and a line was replaced.
            return oldValue;
        }
        #endregion

        #region DeepClear() method
        //Clear the cache and memory completely.
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/DeepClear/*'/>
        public void DeepClear()
        {
            //Inform the replacement policy that this method was invoked.
            replacementPolicy.RefreshPolicy(CacheOperation.DeepClear);
            Clear(true);
        }
        #endregion

        #region Clear() method
        //Clear the cache only.
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/Clear/*'/>
        public void Clear()
        {
            //Inform the replacement policy that this method was invoked.
            replacementPolicy.RefreshPolicy(CacheOperation.Clear);
            Clear(false);
        }
        #endregion

        #region Clear(bool isDeepClear) method
        /**
         * Method used to clear cache, or cache and memory, into their pre-entry state.
         **/
        private void Clear(bool isDeepClear)
        {
            keys = new TKey[keys.Length];
            values = new TValue[values.Length];
            isModified = new bool[isModified.Length];
            accessList.Clear();
            for (int i = 0; i < nWay; i++)
            {
                accessList.Add(new List<int>());
            }
            if (isDeepClear)
            {
                memoryAccess.Clear();
            }
        }
        #endregion

        #region PutAll method
        /**
         * PutAll method, used to enter tuples of generic TKey,TValue pairs into the cache at once.
         * If write-through is used, replaced entries are returned as a TValue array.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/PutAll/*'/>
        public TValue[] PutAll(Tuple<TKey, TValue>[] entryTuples)
        {
            TValue[] oldValues = new TValue[entryTuples.Length];
            for (int i = 0; i < entryTuples.Length; i++)
            {
                oldValues[i] = Put(entryTuples[i].Item1, entryTuples[i].Item2);
            }
            //Inform the replacement policy that this method was invoked.
            replacementPolicy.RefreshPolicy(CacheOperation.PutAll);
            return oldValues;
        }
        #endregion

        #region Remove method
        /**
         * Remove method, which removes entries from the cache.
         * If WriteBack is used, entries are removed from memory as well.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/Remove/*'/>
        public TValue Remove(TKey key)
        {
            if (isWriteBack) {memoryAccess.Remove(key);}
            //Inform the replacement policy that this method was invoked.
            replacementPolicy.RefreshPolicy(CacheOperation.Remove);
            return Get(key, true);
        }
        #endregion

        #region Get(TKey key) method
        /**
         * Get method, used to query the cache for a TValue.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/Get/*'/>
        public TValue Get(TKey key)
        {
            //Inform the replacement policy that this method was invoked.
            replacementPolicy.RefreshPolicy(CacheOperation.Get);
            return Get(key, false);
        }
        #endregion

        #region ContainsKey method
        /**
         * ContainsKey method, used to query the cache as to the existence of a TKey.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/ContainsKey/*'/>
        public bool ContainsKey(TKey key) {
            //Inform the replacement policy that this method was invoked.
            replacementPolicy.RefreshPolicy(CacheOperation.ContainsKey);
            //Calculate hash and set of key.
            RefreshIndexes(key);
            return SearchIfExists(key) != -1;
        }
        #endregion

        #region Get(TKey key, bool isRemove) method
        /**
         * Get method, which returns a TValue with an option to remove the entry after access.
         * If WriteBack is used, then missing entries are recovered from memory.
         * Throws an EntryNotFoundException for missing entries when Write-Through is used.
         **/
        private TValue Get(TKey key, bool isRemove)
        {
            //Calculate hash and set of key.
            RefreshIndexes(key);
            TValue returnValue = default(TValue);
            //Check if the key exists in the cache.
            entryIndex = SearchIfExists(key);
            //If the entry does not exist in the cache, either check the memory or throw an exception.
            if (entryIndex == -1)
            {
                if (isWriteBack) {
                    returnValue = memoryAccess.Get(key);
                    Put(key, returnValue);
                    return returnValue;
                }
                else throw new EntryNotFoundException();
            }
            /**
             * If the entry exists in the cache, refresh its access history and set the return value to the entry value.
             * If the remove option is specified, then remove the entry from the cache (and memory too, if Write-Back is used).
             **/            
            else
            {
                if (accessList[set].Contains(hashCode))
                {
                    accessList[set].Remove(hashCode);
                }
                accessList[set].Add(hashCode);
                returnValue = values[entryIndex];
                if (isRemove)
                {
                    keys[entryIndex] = default(TKey);
                    values[entryIndex] = default(TValue);
                }
            }
            if (isRemove && isWriteBack) memoryAccess.Remove(key);
            return returnValue;
        }
        #endregion

        #region SearchIfExists method
        /**
         * Check whether a given TKey exists in the set it should belong to, and also record the first empty cache line in that set.
         **/
        private int SearchIfExists(TKey key)
        {
            int entryIndex = -1;
            firstNullIndex = -1;
            for (int i = 0; i < lineNumber; i++)
            {
                //Does the key match with the stored key?
                if (EqualityComparer<TKey>.Default.Equals(keys[setIndex + i],key))
                {
                    entryIndex = setIndex + i;
                }
                //Is the key entry empty?
                if (EqualityComparer<TKey>.Default.Equals(keys[setIndex + i], default(TKey)) && firstNullIndex == -1) {
                    firstNullIndex = setIndex + i;
                }
            }
            return entryIndex;
        }
        #endregion

        #region RefreshIndexes method
        /**
         * Method for calculating the hash of a key, and which set it belongs to based upon that hash.
         */
        private void RefreshIndexes(TKey key)
        {
            //Take the absolute value of the hash modulo the cache to map a given generic key object to a specific cache line.
            hashCode = Math.Abs(key.GetHashCode() % cacheSize);
            //Calculate the starting index of the set to which the key belongs.
            setIndex = (hashCode - (hashCode % lineNumber));
            //Calculate the set which the key belongs to.
            set = setIndex / lineNumber;
        }
        #endregion

        #region InvokeReflected method
        /**
         * A pseudo-reflection method used to call operations of the cache upon tuples of entries, based on the CacheOperation enum,
         * which encodes the methods the cache implements. Used by the SetOptimizer utility class.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Cache"]/NWaySetAssociativeCache/InvokeReflected/*'/>
        public void InvokeReflected(Tuple<TKey,TValue,CacheOperation> entry)
        {
            switch (entry.Item3) {
                case CacheOperation.Put:
                    Put(entry.Item1,entry.Item2);
                    break;
                case CacheOperation.Get:
                    Get(entry.Item1);
                    break;
                case CacheOperation.Remove:
                    Remove(entry.Item1);
                    break;
                case CacheOperation.PutAll:
                    PutAll(new[] { Tuple.Create(entry.Item1, entry.Item2) });
                    break;
                case CacheOperation.Clear:
                    Clear();
                    break;
                case CacheOperation.DeepClear:
                    DeepClear();
                    break;
                case CacheOperation.ContainsKey:
                    ContainsKey(entry.Item1);
                    break;
            }
        }
        #endregion

    }
}
