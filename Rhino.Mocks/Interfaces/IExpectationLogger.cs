using Castle.DynamicProxy;

namespace Rhino.Mocks.Interfaces
{
    /// <summary>
    /// Logs expectations
    /// </summary>
    public interface IExpectationLogger
    {
        /// <summary>
        /// Logs the given method and arguments
        /// that was used to create an expectation
        /// </summary>
        /// <param name="invocation"></param>
        void LogExpectation(IInvocation invocation);

        /// <summary>
        /// Logs the expected method call
        /// </summary>
        /// <param name="invocation"></param>
        void LogExpectedMethodCall(IInvocation invocation);

        /// <summary>
        /// Logs the unexpected method call
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="message"></param>
        void LogUnexpectedMethodCall(IInvocation invocation, string message);
    }
}
