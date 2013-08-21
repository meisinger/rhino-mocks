using System;
using System.Runtime.Serialization;

namespace Rhino.Mocks.Exceptions
{
    /// <summary>
    /// An expectation violation
    /// </summary>
    [Serializable]
    public class ExpectationViolationException : Exception
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="message"></param>
        public ExpectationViolationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// serialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ExpectationViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
