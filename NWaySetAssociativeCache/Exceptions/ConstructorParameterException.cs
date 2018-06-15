using System;

namespace NWaySetAssociativeCache.Exceptions
{
    /**
     * Exception subclass thrown when the NWaySetAssociativeCache constructor is called with corrupt values.
     **/

    /// <include file='documentation.xml' path='docs/members[@name="Exceptions"]/ConstructorParameterException/Class/*'/>
    public class ConstructorParameterException : Exception
    {
        #region ConstructorParameterException() Constructor
        public ConstructorParameterException()
        {
        }
        #endregion
        #region ConstructorParameterException(string message) Constructor
        public ConstructorParameterException(string message) : base(message)
        {
        }
        #endregion
        #region ConstructorParameterException(string message, Exception inner) Constructor
        public ConstructorParameterException(string message, Exception inner) : base(message, inner)
        {
        }
        #endregion
    }
}