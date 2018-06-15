using NWaySetAssociativeCache.Cache;
using System.Collections.Generic;

namespace NWaySetAssociativeCache.ReplacementPolicies
{
    /**
     * Generic subclass MRUReplacementPolicy of abstract class ReplacementPolicy.
     * Implements the Most Recently Used replacement policy, which replaces a cache entry if it was most recently accessed.
     **/
    /// <include file='documentation.xml' path='docs/members[@name="ReplacementPolicies"]/MRUReplacementPolicy/Class/*'/>
    public class MRUReplacementPolicy<TKey,TValue> : ReplacementPolicy<TKey,TValue>
    {
        #region GetReplacementIndex method
        //Method for returning the index to be replaced, in this case the most recently used entry in the access history.
        /// <include file='documentation.xml' path='docs/members[@name="ReplacementPolicies"]/MRUReplacementPolicy/GetReplacementIndex/*'/>
        public override int GetReplacementIndex(int lines, int set, List<List<int>> accessList, TKey[] keys, TValue[] values, bool[] isModified)
        {
            return accessList[set].Count > 0 ? accessList[set][accessList[set].Count - 1] : 0;
        }
        #endregion

        #region RefreshPolicy method
        //Method for refreshing the policy based on cache operations.
        /// <include file='documentation.xml' path='docs/members[@name="ReplacementPolicies"]/MRUReplacementPolicy/RefreshPolicy/*'/>
        public override void RefreshPolicy(NWaySetAssociativeCache<TKey, TValue>.CacheOperation cacheOperation)
        {
            
        }
        #endregion
    }
}
