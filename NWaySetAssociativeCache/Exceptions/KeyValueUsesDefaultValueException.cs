using System;

namespace NWaySetAssociativeCache.Exceptions
{
    /**
     * Exception subclass thrown when a cache addition attempts to use a TKey equal to default(Tkey).
     **/
    /// <include file='documentation.xml' path='docs/members[@name="Exceptions"]/KeyValueUsesDefaultValue/Class/*'/>
    public class KeyValueUsesDefaultValueException<TKey> : Exception
    {
        #region KeyValueUsesDefaultValue() Constructor
        public KeyValueUsesDefaultValueException()
        {
        }
        #endregion
        #region KeyValueUsesDefaultValue(string message) Constructor
        public KeyValueUsesDefaultValueException(string message) : base(message)
        {
        }
        #endregion
        #region KeyValueUsesDefaultValue(string message, Exception inner) Constructor
        public KeyValueUsesDefaultValueException(string message, Exception inner) : base(message, inner)
        {
        }
        #endregion
    }
}
