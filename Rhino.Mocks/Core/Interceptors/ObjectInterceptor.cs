using System;
using System.Runtime.CompilerServices;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Core.Interceptors
{
    /// <summary>
    /// Interceptor for <see cref="System.Object"/>
    /// </summary>
    public class ObjectInterceptor : MarshalByRefObject, IInterceptor
    {
        /// <summary>
        /// Intercept a method on <see cref="System.Object"/>
        /// </summary>
        /// <param name="invocation"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}
