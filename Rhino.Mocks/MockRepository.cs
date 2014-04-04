using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Helpers;
using Rhino.Mocks.Interceptors;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Remoting;
using Rhino.Mocks.Expectations;
using Castle.DynamicProxy.Internal;

namespace Rhino.Mocks
{
    /// <summary>
    /// Repository for mocked objects and expectations.
    /// </summary>
    public class MockRepository
    {
        private static readonly Dictionary<Type, ProxyGenerator> generators;
        private static readonly ObjectInterceptor objectInterceptor;

        private readonly RepositoryForDelegates delegateRepository;
        private readonly ProxyGenerationOptions generatorOptions;
        private readonly ProxyGenerationOptions defaultOptions;
        
        /// <summary>
        /// Static Constructor
        /// </summary>
        static MockRepository()
        {
            generators = new Dictionary<Type, ProxyGenerator>();
            objectInterceptor = new ObjectInterceptor();
        }

        /// <summary>
        /// Constructs an instance of the Mock Repository.
        /// </summary>
        /// <remarks>
        /// There is very little reason to instantiate an instance of MockRepository.
        /// This is intended for internal use only.
        /// </remarks>
        public MockRepository()
        {
            delegateRepository = new RepositoryForDelegates();
            generatorOptions = new ProxyGenerationOptions
            {
                Selector = new RhinoSelector(),
            };

            generatorOptions.AdditionalAttributes
                .Add(AttributeUtil.CreateBuilder<__ProtectAttribute>());

            defaultOptions = new ProxyGenerationOptions
            {
                Selector = new RhinoSelector(),
            };

            ArgumentManager.Clear();
        }

        internal static ExpectMethod GetMethodCallArguments<T>(T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Assertions cannot be made on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Assertions can only be made on a mocked object or instance.");

            var assertion = new ExpectMethod();
            container.MarkForAssertion(assertion);

            try
            {
                action(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception caught while making assertion.", ex);
            }

            if (container.ExpectationMarked)
                throw new InvalidOperationException();

            return assertion;
        }

        internal static ExpectMethod<TResult> GetMethodCallArguments<T, TResult>(T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Assertions cannot be made on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Assertions can only be made on a mocked object or instance.");

            var assertion = new ExpectMethod<TResult>();
            container.MarkForAssertion(assertion);

            try
            {
                func(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception caught while making assertion.", ex);
            }

            if (container.ExpectationMarked)
                throw new InvalidOperationException();

            return assertion;
        }

        /// <summary>
        /// Creates a mocked instance of the given type
        /// </summary>
        /// <typeparam name="T">the type to mock</typeparam>
        /// <returns>a mocked instance of the given type</returns>
        public static T Mock<T>()
            where T : class
        {
            return MockMulti<T>(new Type[0], new object[0]);
        }

        /// <summary>
        /// Creates a mocked instance of the given type
        /// </summary>
        /// <remarks>
        /// Applicable for Partial mocks
        /// </remarks>
        /// <typeparam name="T">the type to mock</typeparam>
        /// <param name="arguments">constructor arguments</param>
        /// <returns>a mocked instance of the given type</returns>
        public static T Mock<T>(params object[] arguments)
            where T : class
        {
            return MockMulti<T>(new Type[0], arguments);
        }

        /// <summary>
        /// Creates a mocked instance of the given type which implements
        /// the given interface type
        /// </summary>
        /// <typeparam name="T">the type to mock</typeparam>
        /// <typeparam name="TInterface">the interface implemented by the type</typeparam>
        /// <returns>a mocked instance of the given type</returns>
        public static T Mock<T, TInterface>()
            where T : class
            where TInterface : class
        {
            return MockMulti<T>(new Type[] { typeof(TInterface) }, new object[0]);
        }

        /// <summary>
        /// Creates a mocked instance of the given type which implements
        /// the given interface type
        /// </summary>
        /// <remarks>
        /// Applicable for Partial mocks
        /// </remarks>
        /// <typeparam name="T">the type to mock</typeparam>
        /// <typeparam name="TInterface">the interface implemented by the type</typeparam>
        /// <param name="arguments">constructor arguments</param>
        /// <returns>a mocked instance of the given type</returns>
        public static T Mock<T, TInterface>(params object[] arguments)
            where T : class
        {
            return MockMulti<T>(new Type[] { typeof(TInterface) }, arguments);
        }

        /// <summary>
        /// Creates a mocked instance of the given type which implements
        /// the given interface types
        /// </summary>
        /// <typeparam name="T">the type to mock</typeparam>
        /// <typeparam name="TInterface1">the interface implemented by the type</typeparam>
        /// <typeparam name="TInterface2">additional interface implemented by the type</typeparam>
        /// <returns>a mocked instance of the given type</returns>
        public static T Mock<T, TInterface1, TInterface2>()
            where T : class
        {
            return MockMulti<T>(new Type[] { typeof(TInterface1), typeof(TInterface2) }, new object[0]);
        }

        /// <summary>
        /// Creates a mocked instance of the given type which implements
        /// the given interface types
        /// </summary>
        /// <remarks>
        /// Applicable for Partial mocks
        /// </remarks>
        /// <typeparam name="T">the type to mock</typeparam>
        /// <typeparam name="TInterface1">the interface implemented by the type</typeparam>
        /// <typeparam name="TInterface2">additional interface implemented by the type</typeparam>
        /// <param name="arguments">constructor arguments</param>
        /// <returns>a mocked instance of the given type</returns>
        public static T Mock<T, TInterface1, TInterface2>(params object[] arguments)
            where T : class
        {
            return MockMulti<T>(new Type[] { typeof(TInterface1), typeof(TInterface2) }, arguments);
        }

        /// <summary>
        /// Creates a mocked instance of the given type which implements
        /// the collection of types
        /// </summary>
        /// <remarks>
        /// Applicable for Partial mocks
        /// </remarks>
        /// <typeparam name="T">the type to mock</typeparam>
        /// <param name="extraTypes">array of types implemented by the type</param>
        /// <param name="arguments">constructor arguments</param>
        /// <returns>a mocked instance of the given type</returns>
        public static T MockMulti<T>(Type[] extraTypes, params object[] arguments)
            where T : class
        {
            var type = typeof(T);
            var remoteType = typeof(MarshalByRefObject);

            var repository = new MockRepository();
            if (remoteType.IsAssignableFrom(type))
            {
                if (arguments == null || arguments.Length == 0)
                    return (T)repository.CreateMockRemoted(type);
            }

            return repository.CreateMockObject<T>(extraTypes, arguments);
        }

        /// <summary>
        /// Creates a mocked instance of the given type capable of being used
        /// for remoting purposes
        /// </summary>
        /// <typeparam name="T">the type to mock</typeparam>
        /// <returns>a remotable mocked instance of the given type</returns>
        public static T MockWithRemoting<T>()
        {
            var type = typeof(T);
            var repository = new MockRepository();

            return (T)repository.CreateMockRemoted(type);
        }

        /// <summary>
        /// Creates a mocked instance of the given type targeting classes 
        /// with abstract or virtual members
        /// </summary>
        /// <typeparam name="T">the class to mock</typeparam>
        /// <returns>a mocked instance of the given type</returns>
        public static T Partial<T>()
            where T : class
        {
            return PartialMulti<T>(new Type[0], new object[0]);
        }

        /// <summary>
        /// Creates a mocked instance of the given type targeting classes
        /// with abstract or virtual members
        /// </summary>
        /// <typeparam name="T">the class to mock</typeparam>
        /// <param name="arguments">constructor arguments</param>
        /// <returns>a mocked instance of the given type</returns>
        public static T Partial<T>(params object[] arguments)
            where T : class
        {
            return PartialMulti<T>(new Type[0], arguments);
        }

        /// <summary>
        /// Creates a mocked instance of the given type which implements
        /// the given interface targeting classes with abstract or virtual members
        /// </summary>
        /// <typeparam name="T">the class to mock</typeparam>
        /// <typeparam name="TInterface">the interface implemented by the type</typeparam>
        /// <returns>a mocked instance of the given type</returns>
        public static T Partial<T, TInterface>()
            where T : class
        {
            return PartialMulti<T>(new Type[] { typeof(TInterface) }, new object[0]);
        }

        /// <summary>
        /// Creates a mocked instance of the given type which implements
        /// the given interface targeting classes with abstract or virtual members
        /// </summary>
        /// <typeparam name="T">the class to mock</typeparam>
        /// <typeparam name="TInterface">the interface implemented by the type</typeparam>
        /// <param name="arguments">constructor arguments</param>
        /// <returns>a mocked instance of the given type</returns>
        public static T Partial<T, TInterface>(params object[] arguments)
            where T : class
        {
            return PartialMulti<T>(new Type[] { typeof(TInterface) }, arguments);
        }

        /// <summary>
        /// Creates a mocked instance of the given type which implements
        /// the given interface types targeting classes with abstract or virtual members
        /// </summary>
        /// <typeparam name="T">the class to mock</typeparam>
        /// <typeparam name="TInterface1">the interface implemented by the type</typeparam>
        /// <typeparam name="TInterface2">additional interface implemented by the type</typeparam>
        /// <returns>a mocked instance of the given type</returns>
        public static T Partial<T, TInterface1, TInterface2>()
            where T : class
        {
            return PartialMulti<T>(new Type[] { typeof(TInterface1), typeof(TInterface2) }, new object[0]);
        }

        /// <summary>
        /// Creates a mocked instance of the given type which implements
        /// the given interface types targeting classes with abstract or virtual members
        /// </summary>
        /// <typeparam name="T">the class to mock</typeparam>
        /// <typeparam name="TInterface1">the interface implemented by the type</typeparam>
        /// <typeparam name="TInterface2">additional interface implemented by the type</typeparam>
        /// <param name="arguments">constructor arguments</param>
        /// <returns>a mocked instance of the given type</returns>
        public static T Partial<T, TInterface1, TInterface2>(params object[] arguments)
            where T : class
        {
            return PartialMulti<T>(new Type[] { typeof(TInterface1), typeof(TInterface2) }, arguments);
        }

        /// <summary>
        /// Creates a mocked instance of the given type which implements
        /// the collection of types targeting classes with abstract or virtual members
        /// </summary>
        /// <typeparam name="T">the class to mock</typeparam>
        /// <param name="extraTypes">array of types implemented by the type</param>
        /// <param name="arguments">constructor arguments</param>
        /// <returns>a mocked instance of the given type</returns>
        public static T PartialMulti<T>(Type[] extraTypes, params object[] arguments)
            where T : class
        {
            if (extraTypes.Any(x => !x.IsInterface))
                throw new ArgumentException("Additional types to be implemented for a mocked object must be interfaces.", "extraTypes");

            var type = typeof(T);
            if (type.IsInterface)
                throw new InvalidOperationException("Interfaces cannot be used to create a Partial mock.");

            var repository = new MockRepository();
            return repository.CreateMockClass(type, extraTypes, arguments, true) as T;
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

            return CreateMockClass(type, extraTypes, arguments, false);
        }

        internal static IMockExpectationContainer GetExpectationContainer(object instance)
        {
            if (typeof(Delegate).IsAssignableFrom(instance.GetType()))
            {
                var instanceDelegate = (Delegate)instance;
                return instanceDelegate.Target as IMockExpectationContainer;
            }

            return instance as IMockExpectationContainer;
        }

        private object CreateMockClass(Type type, Type[] extraTypes, object[] arguments, bool isPartial)
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
                mocked.IsPartialInstance = isPartial;
                mocked.ConstructorArguments = arguments;

                GC.SuppressFinalize(proxy);

                instance.ProxyInstance = proxy;
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

            instance.ProxyInstance = proxy;
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

            instance.ProxyInstance = proxy;
            return proxy;
        }

        private object CreateMockRemoted(Type type)
        {
            var instance = new MockInstance(new Type[0]);
            var proxyInterceptor = new ProxyInterceptor(instance);

            var generator = new RepositoryForRemoting();
            var proxy = generator.CreateMockRemoting(type, proxyInterceptor, instance);

            instance.ProxyInstance = proxy;
            return proxy;
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
