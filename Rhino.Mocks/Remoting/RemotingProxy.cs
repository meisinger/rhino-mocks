using System;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Castle.DynamicProxy;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Remoting
{
    internal class RemotingProxy : RealProxy
    {
        private readonly IInterceptor interceptor;
        private readonly IMockInstance instance;

        public IMockInstance MockInstance
        {
            get { return instance; }
        }

        public RemotingProxy(Type type, IInterceptor interceptor, IMockInstance instance)
            : base(type)
        {
            this.interceptor = interceptor;
            this.instance = instance;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var mcm = msg as IMethodCallMessage;
            if (mcm == null)
                return null;

            if (IsEqualsCall(mcm))
                return ReturnValue(HandleEquals(mcm), mcm);

            if (IsGetHashCodeCall(mcm))
                return ReturnValue(GetHashCode(), mcm);

            if (IsGetTypeCall(mcm))
                return ReturnValue(GetProxiedType(), mcm);

            if (IsToStringCall(mcm))
            {
                var type = GetProxiedType();
                var hashCode = GetHashCode();
                var value = string.Format("RemotingMock_{0}<{1}>", hashCode, type.Name);

                return ReturnValue(value, mcm);
            }

            var invocation = new RemotingInvocation(this, mcm);
            interceptor.Intercept(invocation);

            return ReturnValue(invocation.ReturnValue, invocation.Arguments, mcm);
        }

        private bool IsEqualsCall(IMethodCallMessage mcm)
        {
            if (!mcm.MethodName.Equals("Equals", StringComparison.Ordinal))
                return false;

            var arguments = mcm.MethodSignature as Type[];
            if (arguments == null)
                return false;

            if (arguments.Length == 1 && arguments[0] == typeof(object))
                return true;

            return false;
        }

        private bool IsGetHashCodeCall(IMethodCallMessage mcm)
        {
            if (!mcm.MethodName.Equals("GetHashCode", StringComparison.Ordinal))
                return false;

            var arguments = mcm.MethodSignature as Type[];
            if (arguments == null)
                return false;

            return (arguments.Length == 0);
        }

        private bool IsGetTypeCall(IMethodCallMessage mcm)
        {
            if (!mcm.MethodName.Equals("GetType", StringComparison.Ordinal))
                return false;

            if (mcm.MethodBase.DeclaringType != typeof(object))
                return false;

            var arguments = mcm.MethodSignature as Type[];
            if (arguments == null)
                return false;

            return (arguments.Length == 0);
        }

        private bool IsToStringCall(IMethodCallMessage mcm)
        {
            if (!mcm.MethodName.Equals("ToString", StringComparison.Ordinal))
                return false;

            var arguments = mcm.MethodSignature as Type[];
            if (arguments == null)
                return false;

            return (arguments.Length == 0);
        }

        private bool HandleEquals(IMethodMessage mm)
        {
            var arg1 = mm.Args[0];
            if (arg1 == null)
                return false;

            var proxyOperation = arg1 as IRemotingProxyOperation;
            if (proxyOperation != null)
            {
                proxyOperation.Process(this);
                return false;
            }

            return ReferenceEquals(GetTransparentProxy(), arg1);
        }

        private IMessage ReturnValue(object value, IMethodCallMessage mcm)
        {
            return new ReturnMessage(value, null, 0, mcm.LogicalCallContext, mcm);
        }

        private IMessage ReturnValue(object value, object[] outArgs, IMethodCallMessage mcm)
        {
            var outArgCount = (outArgs != null)
                ? outArgs.Length
                : 0;

            return new ReturnMessage(value, outArgs, outArgCount, mcm.LogicalCallContext, mcm);
        }
    }
}
