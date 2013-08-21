using Castle.DynamicProxy;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Loggers
{
    /// <summary>
    /// Default logger which performs no logging
    /// </summary>
    public class NullLogger : IExpectationLogger
    {
        /// <summary>
        /// Logs the given method and arguments
        /// that was used to create an expectation
        /// </summary>
        /// <param name="invocation"></param>
        public void LogExpectation(IInvocation invocation)
        {
        }

        /// <summary>
        /// Logs the expected method call
        /// </summary>
        /// <param name="invocation"></param>
        public void LogExpectedMethodCall(IInvocation invocation)
        {
        }

        /// <summary>
        /// Logs the unexpected method call
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="message"></param>
        public void LogUnexpectedMethodCall(IInvocation invocation, string message)
        {
        }
    }
}
