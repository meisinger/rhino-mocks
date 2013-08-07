using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Core
{
    internal class RepositoryForDelegates
    {
        private readonly ModuleScope scope;
        private readonly Dictionary<Type, Type> dictionary;

        private long counter;

        public RepositoryForDelegates()
        {
            scope = new ModuleScope();
            dictionary = new Dictionary<Type, Type>();

            counter = 0;
        }

        public Type CreateTargetInterface(Type type)
        {
            lock (dictionary)
            {
                Type dynamicType;
                if (dictionary.TryGetValue(type, out dynamicType))
                    return dynamicType;

                dynamicType = GenerateTargetInterface(type);
                dictionary[type] = dynamicType;

                return dynamicType;
            }
        }

        private Type GenerateTargetInterface(Type type)
        {
            var count = Interlocked.Increment(ref counter);
            var dynamicName = string.Format("ProxyDelegate_{0}_{1}", type.Name, count);
            
            var method = type.GetMethod("Invoke");
            var returnType = method.ReturnType;
            var parameters = method.GetParameters();
            var parameterTypes = parameters
                .Select(x => x.ParameterType)
                .ToArray();

            var builder = scope.ObtainDynamicModule(true)
                .DefineType(dynamicName, TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public);

            builder.DefineMethod("Invoke", MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.Public,
                CallingConventions.HasThis, returnType, parameterTypes);

            return builder.CreateType();
        }
    }
}
