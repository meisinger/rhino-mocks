using System;
using System.Runtime.CompilerServices;
using Castle.DynamicProxy;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Interceptors
{
    /// <summary>
    /// Interceptor for <see cref="IMockInstance"/>
    /// </summary>
    public class MockInterceptor : MarshalByRefObject, IInterceptor
    {
        private readonly IMockInstance instance;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instance"></param>
        public MockInterceptor(IMockInstance instance)
        {
            this.instance = instance;
        }

        /// <summary>
        /// Intercept a method on an instance of
        /// <see cref="IMockInstance"/>
        /// </summary>
        /// <param name="invocation"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = invocation.Method
                .Invoke(instance, invocation.Arguments);
        }
    }
}
