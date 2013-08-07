using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Core.Interceptors;
using Rhino.Mocks.Core.Interfaces;

namespace Rhino.Mocks.Core
{
    /// <summary>
    /// Repository for mocked objects and expectations.
    /// </summary>
    public class Repository
    {
        private static readonly Dictionary<Type, ProxyGenerator> generators;
        private static readonly ObjectInterceptor objectInterceptor;

        private readonly RepositoryForDelegates delegateRepository;
        private readonly ProxyGenerationOptions generatorOptions;
        private readonly ProxyGenerationOptions defaultOptions;
        
        static Repository()
        {
            generators = new Dictionary<Type, ProxyGenerator>();
            objectInterceptor = new ObjectInterceptor();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Repository()
        {
            delegateRepository = new RepositoryForDelegates();
            generatorOptions = new ProxyGenerationOptions
            {
                AttributesToAddToGeneratedTypes = { new __ProtectAttribute() },
                Selector = new RhinoSelector(),
            };

            defaultOptions = new ProxyGenerationOptions
            {
                Selector = new RhinoSelector(),
            };
        }

        /// <summary>
        /// Generate a stub-mock for the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static T GenerateStub<T>(params object[] arguments)
            where T : class
        {
            var repository = new Repository();
            return repository.CreateMockObject<T>(arguments);
        }

        internal T CreateMockObject<T>(params Type[] extraTypes)
            where T : class
        {
            return CreateMockObject(typeof(T), extraTypes, new object[0]) as T;
        }

        internal T CreateMockObject<T>(params object[] arguments)
            where T : class
        {
            return CreateMockObject(typeof(T), new Type[0], arguments) as T;
        }

        internal T CreateMockObject<T>(Type[] extraTypes, params object[] arguments)
            where T : class
        {
            return CreateMockObject(typeof(T), extraTypes, arguments) as T;
        }

        internal object CreateMockObject(Type type, params Type[] extraTypes)
        {
            return CreateMockObject(type, extraTypes, new object[0]);
        }

        internal object CreateMockObject(Type type, params object[] arguments)
        {
            return CreateMockObject(type, new Type[0], arguments);
        }

        internal object CreateMockObject(Type type, Type[] extraTypes, params object[] arguments)
        {
            if (extraTypes == null)
                extraTypes = new Type[0];

            if (arguments == null)
                arguments = new object[0];

            if (extraTypes.Any(x => !x.IsInterface))
                throw new ArgumentException("Additional types to be implemented for a mocked object must be interfaces.", "extraTypes");

            if (typeof(MarshalByRefObject).IsAssignableFrom(type))
            {
                if (arguments == null || arguments.Length == 0)
                    return CreateMockRemoted(type);
            }

            if (type.IsInterface)
            {
                if (arguments != null && arguments.Length != 0)
                    throw new ArgumentException("Constructor arguments cannot be supplied when mocking an interface.", "arguments");

                return CreateMockInterface(type, extraTypes);
            }

            if (typeof(Delegate).IsAssignableFrom(type))
            {
                if (arguments != null && arguments.Length != 0)
                    throw new ArgumentException("Constructor arguments cannot be supplied when mocking a delegate.", "arguments");

                return CreateMockDelegate(type);
            }

            return CreateMockClass(type, extraTypes, arguments);
        }

        private object CreateMockClass(Type type, Type[] extraTypes, object[] arguments)
        {
            if (type.IsSealed)
                throw new InvalidOperationException("Sealed classes cannot be mocked.");

            var types = new List<Type>(extraTypes);
            types.Add(type);

            var interfaces = new List<Type>(extraTypes);
            interfaces.Add(typeof(IMockInstance));
            interfaces.Add(typeof(IMockExpectationContainer));

            var instance = new MockInstance(types.ToArray());
            var mockInterceptor = new MockInterceptor(instance);
            var proxyInterceptor = new ProxyInterceptor(instance);
            var generator = IdentifyGenerator(type);

            try
            {
                var proxy = generator.CreateClassProxy(type, interfaces.ToArray(), 
                    defaultOptions, arguments, mockInterceptor, proxyInterceptor, objectInterceptor);

                var mocked = (IMockInstance)proxy;
                mocked.ConstructorArguments = arguments;

                GC.SuppressFinalize(proxy);

                return proxy;
            }
            catch (MissingMethodException ex)
            {
                var message = string.Format("No constructor taking {0} arguments found.", arguments.Length);
                throw new MissingMethodException(message, ex);
            }
            catch (TargetInvocationException ex)
            {
                var message = string.Format("Exception was thrown in constructor: {0}", ex.InnerException);
                throw new Exception(message, ex.InnerException);
            }
        }

        private object CreateMockDelegate(Type type)
        {
            if (typeof(Delegate).Equals(type))
                throw new InvalidOperationException("The base System.Delegate type cannot be mocked.");

            var types = new List<Type>();
            types.Add(type);

            var interfaces = new List<Type>();
            interfaces.Add(typeof(IMockInstance));
            interfaces.Add(typeof(IMockExpectationContainer));

            var instance = new MockInstance(types.ToArray());
            var mockInterceptor = new MockInterceptor(instance);
            var proxyInterceptor = new ProxyInterceptor(instance);
            var dynamicType = delegateRepository.CreateTargetInterface(type);
            var generator = IdentifyGenerator(type);

            var target = generator.CreateInterfaceProxyWithoutTarget(dynamicType, interfaces.ToArray(),
                generatorOptions, mockInterceptor, proxyInterceptor, objectInterceptor);

            var proxy = Delegate.CreateDelegate(type, target, "Invoke");

            return proxy;
        }

        private object CreateMockInterface(Type type, Type[] extraTypes)
        {
            var types = new List<Type>(extraTypes);
            types.Add(type);

            var interfaces = new List<Type>(extraTypes);
            interfaces.Add(typeof(IMockInstance));
            interfaces.Add(typeof(IMockExpectationContainer));

            var instance = new MockInstance(types.ToArray());
            var mockInterceptor = new MockInterceptor(instance);
            var proxyInterceptor = new ProxyInterceptor(instance);
            var generator = IdentifyGenerator(type);

            var proxy = generator.CreateInterfaceProxyWithoutTarget(type, interfaces.ToArray(), 
                generatorOptions, mockInterceptor, proxyInterceptor, objectInterceptor);

            return proxy;
        }

        private object CreateMockRemoted(Type type)
        {
            throw new NotImplementedException();
        }

        private static ProxyGenerator IdentifyGenerator(Type type)
        {
            ProxyGenerator generator;
            if (!generators.TryGetValue(type, out generator))
            {
                generator = new ProxyGenerator();
                generators[type] = generator;
            }

            return generator;
        }
    }
}
