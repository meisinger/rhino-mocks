using System.Reflection;
using Castle.DynamicProxy;

namespace Rhino.Mocks
{
    /// <summary>
    /// Data structure of the current method invocation
    /// </summary>
    public class MethodInvocation
    {
        private readonly IInvocation invocation;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="invocation"></param>
        internal MethodInvocation(IInvocation invocation)
        {
            this.invocation = invocation;
        }

        /// <summary>
        /// Arguments of the current method invocation
        /// </summary>
        public object[] Arguments
        {
            get { return invocation.Arguments; }
        }

        /// <summary>
        /// Method of the current method invocation
        /// </summary>
        public MethodInfo Method
        {
            get { return invocation.Method; }
        }

        /// <summary>
        /// Gets or sets the return value of the current
        /// method invocation
        /// </summary>
        public object ReturnValue
        {
            get { return invocation.ReturnValue; }
            set { invocation.ReturnValue = value; }
        }
    }
}
