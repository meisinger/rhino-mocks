using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Castle.DynamicProxy;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Interceptors
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
                RhinoMocks.Logger.LogExpectation(invocation);

                var expectation = container.GetMarkedExpectation();
                if (expectation.ReturnType.Equals(type))
                {
                    expectation.HandleMethodCall(method, arguments);
                    invocation.ReturnValue = IdentifyDefaultValue(type);
                    return;
                }
                
                var recursive = ParseRecursiveExpectation(container, expectation, type);
                recursive.HandleMethodCall(method, arguments);
                invocation.ReturnValue = recursive.ReturnValue;
                return;
            }

            invocation.ReturnValue = container
                .HandleMethodCall(invocation, method, arguments);
        }

        private object IdentifyDefaultValue(Type type)
        {
            if (!type.IsValueType || type == typeof(void))
                return null;

            return Activator.CreateInstance(type);
        }

        private object IdentifyReturnType(Type type)
        {
            if (type == typeof(void))
                return null;

            if (!type.IsValueType)
            {
                if (type == typeof(string))
                    return null;

                object typeInstance;

                try
                {
                    var repository = new Repository();
                    typeInstance = repository.CreateMockObject(type, new Type[0], new object[0]);
                }
                catch (Exception)
                {
                    typeInstance = null;
                }

                return typeInstance;
            }

            return Activator.CreateInstance(type);
        }

        private Expectation ParseRecursiveExpectation(IMockExpectationContainer container, Expectation current, Type type)
        {
            var typeInstance = IdentifyReturnType(type);
            var typeContainer = typeInstance as IMockExpectationContainer;
            if (typeContainer == null)
                return current;

            var genericType = typeof(ExpectMethod<>);
            var replaceType = genericType.MakeGenericType(type);
            var replaceInstance = Activator.CreateInstance(replaceType) as Expectation;
            replaceInstance.SetReturnValue(typeInstance);
            container.RemoveExpectation(current);
            container.AddExpectation(replaceInstance);

            typeContainer.MarkForExpectation(current);
            return replaceInstance;
        }
    }
}
