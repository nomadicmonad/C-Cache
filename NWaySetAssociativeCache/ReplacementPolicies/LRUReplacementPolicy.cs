using NWaySetAssociativeCache.Cache;
using System.Collections.Generic;

namespace NWaySetAssociativeCache.ReplacementPolicies
{
    /**
     * Generic subclass LRUReplacementPolicy of abstract class ReplacementPolicy.
     * Implements the Least Recently Used replacement policy, which replaces a cache entry if it was least recently accessed.
     **/
    /// <include file='documentation.xml' path='docs/members[@name="ReplacementPolicies"]/LRUReplacementPolicy/Class/*'/>
    public class LRUReplacementPolicy<TKey,TValue> : ReplacementPolicy<TKey,TValue>
    {
        #region GetReplacementIndex Method
        //Method for returning the index to be replaced, in this case the least recently used entry in the access history.
        /// <include file='documentation.xml' path='docs/members[@name="ReplacementPolicies"]/LRUReplacementPolicy/GetReplacementIndex/*'/>
        public override int GetReplacementIndex(int lines, int set, List<List<int>> accessList, TKey[] keys, TValue[] values, bool[] isModified)
        {
            return accessList[set].Count > 0 ? accessList[set][0] : 0;
        }
        #endregion
        #region RefreshPolicy
        //Method for refreshing the policy based on cache operations.
        /// <include file='documentation.xml' path='docs/members[@name="ReplacementPolicies"]/LRUReplacementPolicy/RefreshPolicy/*'/>
        public override void RefreshPolicy(NWaySetAssociativeCache<TKey, TValue>.CacheOperation cacheOperation)
        {
        }
        #endregion
    }
}
