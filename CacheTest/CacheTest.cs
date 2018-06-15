using NWaySetAssociativeCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NWaySetAssociativeCache.Memory;
using NWaySetAssociativeCache.Exceptions;
using System;
using System.Collections.Generic;
using NWaySetAssociativeCache.Utility;
using NWaySetAssociativeCache.ReplacementPolicies;
using NWaySetAssociativeCache.Cache;

namespace CacheTest
{
    /**
     * Test Class for unit testing the NWaySetAssociativeCache project.
     **/
    [TestClass]
    public class CacheTest
    {
        #region Test methods
        //Test whether initialization works.
        [TestMethod]
        public void TestInitialization()
        {
            LRUReplacementPolicy<string, int> lruReplacement = new LRUReplacementPolicy<string, int>();
            MyMemory<string, int> memoryAccess = new MyMemory<string, int>();
            NWaySetAssociativeCache<string, int> cache;
            cache = new NWaySetAssociativeCache<string, int>(memoryAccess, lruReplacement, 100, 10, true);
        }
        //Test whether adding an entry works.
        [TestMethod]
        public void TestAddingEntry()
        {
            LRUReplacementPolicy<string, int> lruReplacement = new LRUReplacementPolicy<string, int>();
            MyMemory<string, int> memoryAccess = new MyMemory<string, int>();
            NWaySetAssociativeCache<string, int> cache;
            cache = new NWaySetAssociativeCache<string, int>(memoryAccess, lruReplacement, 100, 10, true);
            cache.Put("Hello", 5);
        }
        //Test whether adding and then reading an entry works.
        [TestMethod]
        public void TestAddingAndReadingEntry()
        {
            LRUReplacementPolicy<string, int> lruReplacement = new LRUReplacementPolicy<string, int>();
            MyMemory<string, int> memoryAccess = new MyMemory<string, int>();
            NWaySetAssociativeCache<string, int> cache;
            cache = new NWaySetAssociativeCache<string, int>(memoryAccess, lruReplacement, 100, 10, true);
            cache.Put("Hello", 5);
            Assert.AreEqual(cache.Get("Hello"), 5);
        }
        //Test whether requesting a non-existent entry fails predictably.
        [TestMethod]
        public void TestGettingNonExistentEntryInt()
        {
            LRUReplacementPolicy<string, int> lruReplacement = new LRUReplacementPolicy<string, int>();
            MyMemory<string, int> memoryAccess = new MyMemory<string, int>();
            NWaySetAssociativeCache<string, int> cache;
            cache = new NWaySetAssociativeCache<string, int>(memoryAccess, lruReplacement, 100, 10, true);
            cache.Put("Hello", 5);
            Assert.AreEqual(cache.Get("Bye"), default(int));
        }
        //Test whether requesting a non-existent entry fails predictably, with strings.
        [TestMethod]
        public void TestGettingNonExistentEntryString()
        {
            LRUReplacementPolicy<string, string> lruReplacement = new LRUReplacementPolicy<string, string>();
            MyMemory<string, string> memoryAccess = new MyMemory<string, string>();
            NWaySetAssociativeCache<string, string> cache;
            cache = new NWaySetAssociativeCache<string, string>(memoryAccess, lruReplacement, 100, 10, true);
            cache.Put("Hello", 3 + "");
            Assert.AreEqual(cache.Get("Bye"), default(string));
        }
        //Test whether the ContainsKey method works with strings.
        [TestMethod]
        public void TestContainsKeyString()
        {
            LRUReplacementPolicy<string, string> lruReplacement = new LRUReplacementPolicy<string, string>();
            MyMemory<string, string> memoryAccess = new MyMemory<string, string>();
            NWaySetAssociativeCache<string, string> cache;
            cache = new NWaySetAssociativeCache<string, string>(memoryAccess, lruReplacement, 100, 10, true);
            cache.Put("Hello", 3 + "");
            Assert.IsTrue(cache.ContainsKey("Hello"));

        }
        //Test whether the PutAll method works with strings.
        [TestMethod]
        public void TestPutAllString()
        {
            LRUReplacementPolicy<string, string> lruReplacement = new LRUReplacementPolicy<string, string>();
            MyMemory<string, string> memoryAccess = new MyMemory<string, string>();
            NWaySetAssociativeCache<string, string> cache;
            cache = new NWaySetAssociativeCache<string, string>(memoryAccess, lruReplacement, 100, 10, true);
            Tuple<string, string>[] tuples = new Tuple<string, string>[2];
            tuples[0] = Tuple.Create("a", "b");
            tuples[1] = Tuple.Create("c", "d");
            cache.PutAll(tuples);
            Assert.AreEqual(cache.Get("c"), "d");
        }
        //Test whether adding, removing, and then requesting an entry fails in the predicted manner.
        [TestMethod]
        public void TestPutRemoveGet()
        {
            LRUReplacementPolicy<string, string> lruReplacement = new LRUReplacementPolicy<string, string>();
            MyMemory<string, string> memoryAccess = new MyMemory<string, string>();
            NWaySetAssociativeCache<string, string> cache;
            cache = new NWaySetAssociativeCache<string, string>(memoryAccess, lruReplacement, 100, 10, true);
            cache.Put("A", "B");
            cache.Remove("A");
            Assert.AreEqual(cache.Get("A"), default(string));
        }
        //Test whether the LRUReplacementPolicy works.
        [TestMethod]
        public void TestLRUReplacementOverCapacity()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            cache = new NWaySetAssociativeCache<int, int>(memoryAccess, lruReplacement, 8, 2, true);
            Tuple<int, int>[] tuples = new Tuple<int, int>[100];
            for (int i = 0; i < 100; i++)
            {
                tuples[i] = Tuple.Create(i + 1, i + 1);
            }
            cache.PutAll(tuples);
            cache.Get(1);
        }
        //Test whether the MRUReplacementPolicy works.
        [TestMethod]
        public void TestMRUReplacementOverCapacity()
        {
            MRUReplacementPolicy<int, int> mruReplacement = new MRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            cache = new NWaySetAssociativeCache<int, int>(memoryAccess, mruReplacement, 8, 2, true);
            Tuple<int, int>[] tuples = new Tuple<int, int>[100];
            for (int i = 0; i < 100; i++)
            {
                tuples[i] = Tuple.Create(i + 1, i + 1);
            }
            cache.PutAll(tuples);
            cache.Get(1);
        }
        //Test whether a custom replacement policy (random replacement) works.
        [TestMethod]
        public void TestCustomReplacementOverCapacity()
        {
            RandomReplacementPolicy<int, int> ranReplacement = new RandomReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            cache = new NWaySetAssociativeCache<int, int>(memoryAccess, ranReplacement, 8, 2, true);
            Tuple<int, int>[] tuples = new Tuple<int, int>[100];
            for (int i = 0; i < 100; i++)
            {
                tuples[i] = Tuple.Create(i + 1, i + 1);
            }
            cache.PutAll(tuples);
            cache.Get(1);
        }
        //Test whether having coprime cache size and set number produces problems.
        [TestMethod]
        public void TestIncongruentNWay()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            cache = new NWaySetAssociativeCache<int, int>(memoryAccess, lruReplacement, 8, 3, true);
            Tuple<int, int>[] tuples = new Tuple<int, int>[100];
            for (int i = 0; i < 100; i++)
            {
                tuples[i] = Tuple.Create(i + 1, i + 1);
            }
            cache.PutAll(tuples);
            cache.Get(1);
        }
        //Test that having more sets than cache space fails predictably.
        [TestMethod]
        public void TestConstructorError()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            try
            {
                cache = new NWaySetAssociativeCache<int, int>(memoryAccess, lruReplacement, 8, 9, true);
                Assert.Fail();
            }
            catch (ConstructorParameterException e)
            {

                Console.WriteLine(e);
            }
        }
        //Test that getting a non-existent entry with no writeBack fails predictably.
        [TestMethod]
        public void TestGetError()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            try
            {
                cache = new NWaySetAssociativeCache<int, int>(memoryAccess, lruReplacement, 8, 2, false);
                cache.Get(5);
                Assert.Fail();
            }
            catch (EntryNotFoundException e)
            {
                Console.WriteLine(e);
            }
        }
        //Test that getting the last added entry with LRUReplacement works.
        [TestMethod]
        public void TestNoWriteThrough()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            cache = new NWaySetAssociativeCache<int, int>(memoryAccess, lruReplacement, 8, 2, false);
            Tuple<int, int>[] tuples = new Tuple<int, int>[100];
            for (int i = 0; i < 100; i++)
            {
                tuples[i] = Tuple.Create(i + 1, i + 1);
                memoryAccess.Put(i + 1, i + 1);
            }

            int[] oldValues = cache.PutAll(tuples);
            for (int i = 0; i < 100; i++)
            {
                if (oldValues[i] != 0)
                {
                    if (memoryAccess.Contains(i + 1)) { memoryAccess.Remove(i + 1); }
                    memoryAccess.Put(i + 1, oldValues[i]);
                }
            }
            cache.Get(100);
        }
        //Test that getting the first added entry with LRUReplacement fails predictably when no WriteBack is used.
        [TestMethod]
        public void TestNoWriteBackException()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            cache = new NWaySetAssociativeCache<int, int>(memoryAccess, lruReplacement, 8, 2, false);
            Tuple<int, int>[] tuples = new Tuple<int, int>[100];
            for (int i = 0; i < 100; i++)
            {
                tuples[i] = Tuple.Create(i + 1, i + 1);
                memoryAccess.Put(i + 1, i + 1);
            }

            int[] oldValues = cache.PutAll(tuples);
            for (int i = 0; i < 100; i++)
            {
                if (oldValues[i] != 0)
                {
                    if (memoryAccess.Contains(i + 1)) { memoryAccess.Remove(i + 1); }
                    memoryAccess.Put(i + 1, oldValues[i]);
                }
            }
            try
            {
                cache.Get(1);
                Assert.Fail();
            }
            catch (EntryNotFoundException e)
            {
                Console.WriteLine(e);
            }
        }

        //Test that trying to use the default value of a key as a key invokes an exception.
        [TestMethod]
        public void TestKeyError()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            try
            {
                cache = new NWaySetAssociativeCache<int, int>(memoryAccess, lruReplacement, 8, 2, false);
                cache.Put(0, 5);
                Assert.Fail();
            }
            catch (KeyValueUsesDefaultValueException<Int32> e)
            {
                Console.WriteLine(e);
            }
        }

        //Test that trying to remove a non-existent entry produces the expected exception.
        [TestMethod]
        public void TestRemoveNonExistent()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            cache = new NWaySetAssociativeCache<int, int>(memoryAccess, lruReplacement, 8, 2, false);
            try
            {
                cache.Remove(5);
                Assert.Fail();
            }
            catch (EntryNotFoundException e)
            {
                Console.WriteLine(e);
            }
        }
        //Test that no replacement or loss occurs when adding N entries into a fully associative cache of size N.
        [TestMethod]
        public void TestCompletelyAssociative()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            cache = new NWaySetAssociativeCache<int, int>(memoryAccess, lruReplacement, 100, 100, true);
            Tuple<int, int>[] tuples = new Tuple<int, int>[100];
            for (int i = 0; i < 100; i++)
            {
                tuples[i] = Tuple.Create(i + 1, i + 1);
            }
            cache.PutAll(tuples);
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i + 1, cache.Get(i + 1));
            }
        }
        //Test that misses are restored from memory in a WriteBack cache.
        [TestMethod]
        public void TestMissingValueRetrievalFromMemory()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            MyMemory<int, int> memoryAccess = new MyMemory<int, int>();
            NWaySetAssociativeCache<int, int> cache;
            cache = new NWaySetAssociativeCache<int, int>(memoryAccess, lruReplacement, 8, 2, true);
            memoryAccess.Put(5, 6);
            Assert.AreEqual(cache.Get(5), 6);
        }
        //Test that the SetOptimizer utility class works.
        [TestMethod]
        public void TestSetOptimizer()
        {
            LRUReplacementPolicy<int, int> lruReplacement = new LRUReplacementPolicy<int, int>();
            Tuple<int, int, NWaySetAssociativeCache<int, int>.CacheOperation>[] tuples = new Tuple<int, int, NWaySetAssociativeCache<int, int>.CacheOperation>[1000];
            for (int i = 0; i < 1000; i++)
            {
                if ((new Random()).Next(0, 2) > 0)
                {
                    tuples[i] = Tuple.Create(Math.Max(1, (i + 1) % 50), Math.Max(1, (i + 1) % 50), NWaySetAssociativeCache<int, int>.CacheOperation.Put);
                }
                else
                {
                    tuples[i] = Tuple.Create(Math.Max(1, (i + 1) % 50), Math.Max(1, (i + 1) % 50), NWaySetAssociativeCache<int, int>.CacheOperation.Get);
                }
            }
            Console.WriteLine(SetOptimizer<int, int>.GetLowestMissN(10, tuples, lruReplacement));
        }
        #endregion
    }

    #region RandomReplacementPolicy implementation
    /**
     * Generic RandomReplacementPolicy class, inheriting from ReplacementPolicy
     * Picks replacement indexes at random
     **/
    public class RandomReplacementPolicy<Tkey, TValue> : ReplacementPolicy<Tkey, TValue>
    {
        //return a random index to replace
        public override int GetReplacementIndex(int lineNumber, int set, List<List<int>> accessList, Tkey[] keys, TValue[] values, bool[] isModified)
        {
            return (new Random()).Next(lineNumber);
        }

        public override void RefreshPolicy(NWaySetAssociativeCache<Tkey, TValue>.CacheOperation cacheOperation)
        {
        }
    }
    #endregion

    #region MemoryAccess subclass implementation
    /**
     * Generic MyMemory class, inheriting from MemoryAccess.
     * Stores entries in a Dictionary to simulate memory for testing purposes.
     **/
    public class MyMemory<TKey, TValue> : MemoryAccess<TKey, TValue>
    {
        //Dictionary of generic key-value pairs.
        private Dictionary<TKey, TValue> memory = new Dictionary<TKey, TValue>();

        //Clear the Dictionary.
        public override void Clear()
        {
            memory.Clear();
        }
        //Get a generic value from the Dictionary.
        public override TValue Get(TKey key)
        {
            Console.WriteLine("Get request for " + key);
            return memory.ContainsKey(key) ? memory[key] : default(TValue);
        }

        //Put a generic value in the Dictionary.
        public override void Put(TKey key, TValue value)
        {
            Console.WriteLine("Put key " + key + " with value " + value);
            memory.Add(key, value);
        }

        //Remove a generic value from the Dictionary.
        public override TValue Remove(TKey key)
        {
            TValue returnValue = memory.ContainsKey(key) ? memory[key] : default(TValue);
            if (memory.ContainsKey(key)) memory.Remove(key);
            return returnValue;
        }

        //Check whether a generic key exists in the Dictionary.
        public bool Contains(TKey key)
        {
            return memory.ContainsKey(key);
        }
    }
    #endregion
}
