using NWaySetAssociativeCache.Cache;
using System.Collections.Generic;

namespace NWaySetAssociativeCache.ReplacementPolicies
{
    /**
     * Generic abstract class ReplacementPolicy, used to decide which cache entries to replace.
     */
    /// <include file='documentation.xml' path='docs/members[@name="ReplacementPolicies"]/ReplacementPolicy/Class/*'/>
    public abstract class ReplacementPolicy<TKey,TValue>
    {

        #region GetReplacementIndex method
        //Abstract method used to decide which cache line to replace.
        /// <include file='documentation.xml' path='docs/members[@name="ReplacementPolicies"]/ReplacementPolicy/GetReplacementIndex/*'/>
        public abstract int GetReplacementIndex(int lines,int set, List<List<int>> accessList, TKey[] keys, TValue[] values, bool[] isModified);
        #endregion

        #region RefreshPolicy method
        //Abstract method used to inform the replacement policy of cache operations.
        /// <include file='documentation.xml' path='docs/members[@name="ReplacementPolicies"]/ReplacementPolicy/RefreshPolicy/*'/>
        public abstract void RefreshPolicy(NWaySetAssociativeCache<TKey,TValue>.CacheOperation cacheOperation);
        #endregion
    }
}
