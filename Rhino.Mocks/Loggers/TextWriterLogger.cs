using System.IO;
using Castle.DynamicProxy;
using Rhino.Mocks.Helpers;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Loggers
{
    /// <summary>
    /// Logger which writes messages to a Text Writer
    /// </summary>
    public class TextWriterLogger : IExpectationLogger
    {
        private readonly TextWriter writer;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="writer"></param>
        public TextWriterLogger(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Logs the given method and arguments
        /// that was used to create an expectation
        /// </summary>
        /// <param name="invocation"></param>
        public void LogExpectation(IInvocation invocation)
        {
            var method = MethodFormatter.ToString(invocation, invocation.Method, invocation.Arguments);
            writer.WriteLine(string.Format("Expectation: {0}", method));
        }

        /// <summary>
        /// Logs the expected method call
        /// </summary>
        /// <param name="invocation"></param>
        public void LogExpectedMethodCall(IInvocation invocation)
        {
            var method = MethodFormatter.ToString(invocation, invocation.Method, invocation.Arguments);
            writer.WriteLine(string.Format("Method Call: {0}", method));
        }

        /// <summary>
        /// Logs the unexpected method call
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="message"></param>
        public void LogUnexpectedMethodCall(IInvocation invocation, string message)
        {
            var method = MethodFormatter.ToString(invocation, invocation.Method, invocation.Arguments);
            writer.WriteLine(string.Format("{0}: {1}", method, message));
        }
    }
}
