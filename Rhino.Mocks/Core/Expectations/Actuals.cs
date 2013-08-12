using System.Reflection;

namespace Rhino.Mocks.Core.Expectations
{
    /// <summary>
    /// Contains the actual method and
    /// arguments made against a mock
    /// </summary>
    public class Actuals
    {
        private readonly MethodInfo method;
        private readonly object[] arguments;

        /// <summary>
        /// Actual method
        /// </summary>
        public MethodInfo Method
        {
            get { return method; }
        }

        /// <summary>
        /// Actual arguments
        /// </summary>
        public object[] Arguments
        {
            get { return arguments; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        public Actuals(MethodInfo method, object[] arguments)
        {
            if (arguments == null)
                arguments = new object[0];

            this.method = method;
            this.arguments = arguments;
        }
    }
}
