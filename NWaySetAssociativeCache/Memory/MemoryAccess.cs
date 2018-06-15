namespace NWaySetAssociativeCache.Memory
{
    /**
     * Generic abstract MemoryAccess class, used to define the channel through which the cache communicates with memory.
     **/
    /// <include file='documentation.xml' path='docs/members[@name="Memory"]/MemoryAccess/Class/*'/>
    public abstract class MemoryAccess<TKey,TValue>
    {
        #region Put method
        /**
         * Abstract Put method for adding a TKey,TValue pair to memory.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Memory"]/MemoryAccess/Put/*'/>
        public abstract void Put(TKey key,  TValue value);
        #endregion

        #region Get method
        /**
         * Abstract Get method for quering a TKey from memory.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Memory"]/MemoryAccess/Get/*'/>
        public abstract TValue Get(TKey key);
        #endregion

        #region Remove method
        /**
         * Abstract Remove method for removing a memory entry and returning its TValue.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Memory"]/MemoryAccess/Remove/*'/>
        public abstract TValue Remove(TKey key);
        #endregion

        #region Clear method
        /**
         * Abstract Clear method for emptying the entries in memory.
         **/
        /// <include file='documentation.xml' path='docs/members[@name="Memory"]/MemoryAccess/Clear/*'/>
        public abstract void Clear();
        #endregion
    }
}
