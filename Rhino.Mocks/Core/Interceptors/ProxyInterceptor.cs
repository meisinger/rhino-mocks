using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Castle.DynamicProxy;
using Rhino.Mocks.Core.Interfaces;

namespace Rhino.Mocks.Core.Interceptors
{
    /// <summary>
    /// Interceptor for a proxy
    /// </summary>
    public class ProxyInterceptor : MarshalByRefObject, IInterceptor
    {
        private readonly IMockInstance instance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instance"></param>
        public ProxyInterceptor(IMockInstance instance)
        {
            this.instance = instance;
        }

        /// <summary>
        /// Intercept a method on a proxy object
        /// </summary>
        /// <param name="invocation"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Intercept(IInvocation invocation)
        {
            instance.ProxyInstance = invocation.Proxy;

            var container = instance as IMockExpectationContainer;
            if (container == null)
            {
                invocation.ReturnValue = null;
                return;
            }

            var method = invocation.GetConcreteMethod();
            var arguments = invocation.Arguments;
            var type = method.ReturnType;

            if (container.ExpectationMarked)
            {
                var expectation = container.GetMarkedExpectation();
                expectation.StoreMethodCall(method, arguments);
                
                invocation.ReturnValue = IdentifyDefaultValue(type);
                return;
            }

            invocation.ReturnValue = container.TrackMethodCall(invocation, method, arguments);
        }

        private object IdentifyDefaultValue(Type type)
        {
            if (!type.IsValueType || type == typeof(void))
                return null;

            return Activator.CreateInstance(type);
        }
    }
}
