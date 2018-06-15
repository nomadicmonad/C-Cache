using System;

namespace NWaySetAssociativeCache.Exceptions
{
    /**
     * Exception subclass thrown when a cache entry cannot be found, and Write-Through is used.
     **/
    /// <include file='documentation.xml' path='docs/members[@name="Exceptions"]/EntryNotFoundException/Class/*'/>
    public class EntryNotFoundException: Exception
    {
        #region EntryNotFoundException() Constructor
        public EntryNotFoundException() {
        }
        #endregion
        #region EntryNotFoundException(string message) Constructor
        public EntryNotFoundException(string message) : base(message)
        {
        }
        #endregion
        #region EntryNotFoundException(string message, Exception inner) Constructor
        public EntryNotFoundException(string message, Exception inner) : base(message,inner)
        {
        }
        #endregion
    }
}
