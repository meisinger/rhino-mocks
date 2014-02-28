using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Remoting
{
    internal class RemotingInvocation : IInvocation
    {
        private readonly IMethodCallMessage message;
        private readonly RealProxy proxy;

        private object[] arguments;

        public object[] Arguments
        {
            get { return arguments; }
        }

        public Type[] GenericArguments
        {
            get 
            {
                var method = message.MethodBase;
                return (method.IsGenericMethod)
                    ? method.GetGenericArguments()
                    : new Type[0];
            }
        }

        public object InvocationTarget
        {
            get { throw new NotSupportedException(); }
        }

        public MethodInfo Method
        {
            get { return GetConcreteMethod(); }
        }

        public MethodInfo MethodInvocationTarget
        {
            get { throw new NotSupportedException(); }
        }

        public object Proxy
        {
            get { return proxy.GetTransparentProxy(); }
        }

        public object ReturnValue { get; set; }

        public Type TargetType
        {
            get { throw new NotSupportedException(); }
        }

        public RemotingInvocation(RealProxy proxy, IMethodCallMessage message)
        {
            this.message = message;
            this.proxy = proxy;

            arguments = message.Args;
            if (arguments == null)
                arguments = new object[0];
        }

        public object GetArgumentValue(int index)
        {
            throw new NotSupportedException();
        }

        public MethodInfo GetConcreteMethod()
        {
            return (MethodInfo)message.MethodBase;
        }

        public MethodInfo GetConcreteMethodInvocationTarget()
        {
            throw new NotSupportedException();
        }

        public void Proceed()
        {
            throw new InvalidOperationException("Proceed() is not applicable for remoting mocks.");
        }

        public void SetArgumentValue(int index, object value)
        {
            throw new NotSupportedException();
        }        
    }
}
