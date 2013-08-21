using System.Diagnostics;
using System.IO;
using Castle.DynamicProxy;
using Rhino.Mocks.Helpers;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Loggers
{
    /// <summary>
    /// Logger which writes messages to <see cref="System.Diagnostics.Debug"/>
    /// and includes the <see cref="System.Diagnostics.StackTrace"/>
    /// </summary>
    public class TraceWriterWithStackTraceLogger : IExpectationLogger
    {
        /// <summary>
        /// Alternative writer to redirect output to a 
        /// different location
        /// </summary>
        public TextWriter AlternativeWriter { get; set; }

        /// <summary>
        /// Logs the given method and arguments
        /// that was used to create an expectation
        /// </summary>
        /// <param name="invocation"></param>
        public void LogExpectation(IInvocation invocation)
        {
            var method = MethodFormatter.ToString(invocation, invocation.Method, invocation.Arguments);
            Debug.WriteLine(string.Format("Expectation: {0}", method));

            if (AlternativeWriter != null)
                AlternativeWriter.WriteLine(string.Format("Expectation: {0}", method));

            WriteStackTrace();
        }

        /// <summary>
        /// Logs the expected method call
        /// </summary>
        /// <param name="invocation"></param>
        public void LogExpectedMethodCall(IInvocation invocation)
        {
            var method = MethodFormatter.ToString(invocation, invocation.Method, invocation.Arguments);
            Debug.WriteLine(string.Format("Method Call: {0}", method));

            if (AlternativeWriter != null)
                AlternativeWriter.WriteLine(string.Format("Method Call: {0}", method));

            WriteStackTrace();
        }

        /// <summary>
        /// Logs the unexpected method call
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="message"></param>
        public void LogUnexpectedMethodCall(IInvocation invocation, string message)
        {
            var method = MethodFormatter.ToString(invocation, invocation.Method, invocation.Arguments);
            Debug.WriteLine(string.Format("{0}: {1}", method, message));

            if (AlternativeWriter != null)
                AlternativeWriter.WriteLine(string.Format("{0}: {1}", method, message));

            WriteStackTrace();
        }

        private void WriteStackTrace()
        {
            var stackTrace = new StackTrace(true);
            var stackTraceMessage = stackTrace.ToString();

            if (AlternativeWriter != null)
            {
                AlternativeWriter.WriteLine(stackTraceMessage);
                return;
            }

            Debug.WriteLine(stackTraceMessage);
        }
    }
}
