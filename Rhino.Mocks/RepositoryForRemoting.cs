using System;
using Castle.DynamicProxy;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Remoting;

namespace Rhino.Mocks
{
    internal class RepositoryForRemoting
    {
        internal object CreateMockRemoting(Type type, IInterceptor interceptor, IMockInstance instance)
        {
            if (!type.IsInterface && !typeof(MarshalByRefObject).IsAssignableFrom(type))
            {
                var message = string.Format(
                    "Cannot create Remoting Proxy. \"{0}\" is not derived from MarketByRefObject",
                        type.Name);

                throw new InvalidCastException(message);
            }

            return new RemotingProxy(type, interceptor, instance);
        }

        internal static bool IsRemotingProxy(object obj)
        {
            if (obj == null)
                return false;

            var detector = new RemotingProxyDetector();
            obj.Equals(detector);

            return detector.Detected;
        }

        internal static IMockInstance GetMockedInstanceFromProxy(object obj)
        {
            if (obj == null)
                return null;

            var selector = new RemotingProxySelector();
            obj.Equals(selector);

            return selector.MockInstance;
        }
    }
}
