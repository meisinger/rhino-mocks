#region license

// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Impl.Invocation;
using Rhino.Mocks.Impl.RemotingMock;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;

namespace Rhino.Mocks
{
    /// <summary>
    /// Responsible for the instantiation and storage of
    /// proxied types
    /// </summary>
    public partial class MockRepository
    {
        /// <summary>
        /// Delegate to handle the creation of different RecordMockStates
        /// </summary>
        /// <param name="mockedObject"></param>
        /// <returns></returns>
        protected delegate IMockState CreateMockState(IMockedObject mockedObject);

        private static readonly IDictionary<Type, ProxyGenerator> generatorMap = 
            new Dictionary<Type, ProxyGenerator>();
        private static readonly DelegateTargetInterfaceCreator delegateTargetInterfaceCreator =
            new DelegateTargetInterfaceCreator();

        internal static MockRepository lastRepository;

        private readonly Stack recorders;
        private readonly IMethodRecorder rootRecorder;
        private readonly ProxyMethodExpectationsDictionary repeatableMethods;
        private ProxyGenerationOptions proxyGenerationOptions;
        private InvocationVisitorsFactory invocationVisitorsFactory;

        /// <summary>
        /// Collection of proxied delegates
        /// </summary>
        protected IDictionary delegateProxies;

        /// <summary>
        /// Collection of proxied instances
        /// </summary>
        protected ProxyStateDictionary proxies;
        internal IMockedObject lastMockedObject;

        internal static IMockedObject LastMockedObject
        {
            get
            {
                if (lastRepository == null)
                    return null;

                return lastRepository.lastMockedObject;
            }
        }

        internal IMethodRecorder Recorder
        {
            get { return recorders.Peek() as IMethodRecorder; }
        }

        internal IMethodRecorder Replayer
        {
            get { return rootRecorder; }
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public MockRepository()
        {
            proxyGenerationOptions = new ProxyGenerationOptions
            {
                AttributesToAddToGeneratedTypes = { new __ProtectAttribute() }
            };

            repeatableMethods = new ProxyMethodExpectationsDictionary();
            rootRecorder = new UnorderedMethodRecorder(repeatableMethods);

            recorders = new Stack();
            recorders.Push(rootRecorder);

            proxies = new ProxyStateDictionary();
            delegateProxies = new Hashtable(MockedObjectsEquality.Instance);
            invocationVisitorsFactory = new InvocationVisitorsFactory();

            ArgManager.Clear();
        }

        internal IDisposable Ordered()
        {
            return new RecorderChanger(this, Recorder, new OrderedMethodRecorder(Recorder, repeatableMethods));
        }

        internal IDisposable Unordered()
        {
            return new RecorderChanger(this, Recorder, new UnorderedMethodRecorder(Recorder, repeatableMethods));
        }

        internal void BackToRecord(object obj)
        {
            BackToRecord(obj, BackToRecordOptions.All);
        }

        internal void BackToRecord(object obj, BackToRecordOptions options)
        {
            IsMockObjectFromThisRepository(obj);

            if ((options & BackToRecordOptions.Expectations) == BackToRecordOptions.Expectations)
            {
                foreach (IExpectation expectation in rootRecorder.GetAllExpectationsForProxy(obj))
                {
                    rootRecorder.RemoveExpectation(expectation);
                }

                rootRecorder.RemoveAllRepeatableExpectationsForProxy(obj);
            }

            GetMockedObject(obj).ClearState(options);

            proxies[obj] = proxies[obj].BackToRecord();

            foreach (IMockedObject dependentMock in GetMockedObject(obj).DependentMocks)
            {
                BackToRecord(dependentMock, options);
            }
        }

        internal void BackToRecordAll()
        {
            BackToRecordAll(BackToRecordOptions.All);
        }

        internal void BackToRecordAll(BackToRecordOptions options)
        {
            if (proxies.Count == 0)
                return;

            foreach (object key in proxies.Keys)
            {
                BackToRecord(key, options);
            }
        }

        internal bool IsInReplayMode(object mock)
        {
            if (mock == null)
                throw new ArgumentNullException("mock");

            IMockState mockState;
            if (!proxies.TryGetValue(mock, out mockState))
                throw new ArgumentException(mock + " is not a mock.", "mock");

            return (mockState is ReplayMockState);
        }

        internal void Replay(object obj)
        {
            ReplayCore(obj, true);
        }

        internal void ReplayAll()
        {
            if (proxies.Count == 0)
                return;

            foreach (object key in proxies.Keys)
            {
                if (proxies[key] is RecordMockState)
                    Replay(key);
            }
        }

        internal void Verify(object obj)
        {
            IsMockObjectFromThisRepository(obj);

            try
            {
                proxies[obj].Verify();

                foreach (IMockedObject dependentMock in GetMockedObject(obj).DependentMocks)
                {
                    Verify(dependentMock);
                }
            }
            finally
            {
                proxies[obj] = proxies[obj].VerifyState;
            }
        }

        internal void VerifyAll()
        {
            if (lastRepository == this)
                lastRepository = null;

            if (proxies.Keys.Count == 0)
                return;

            StringCollection validationErrors = new StringCollection();
            foreach (object key in new ArrayList(proxies.Keys))
            {
                if (proxies[key] is VerifiedMockState)
                    continue;
                try
                {
                    Verify(key);
                }
                catch (ExpectationViolationException e)
                {
                    validationErrors.Add(e.Message);
                }
            }

            if (validationErrors.Count == 0)
                return;

            if (validationErrors.Count == 1)
                throw new ExpectationViolationException(validationErrors[0]);

            StringBuilder sb = new StringBuilder();
            foreach (string validationError in validationErrors)
            {
                sb.AppendLine(validationError);
            }

            throw new ExpectationViolationException(sb.ToString());
        }

        internal object DynamicMock(Type type, params object[] argumentsForConstructor)
        {
            if (ShouldUseRemotingProxy(type, argumentsForConstructor))
                return RemotingMock(type, CreateDynamicRecordState);

            return DynamicMultiMock(type, new Type[0], argumentsForConstructor);
        }

        internal T DynamicMock<T>(params object[] argumentsForConstructor) where T : class
        {
            if (ShouldUseRemotingProxy(typeof(T), argumentsForConstructor))
                return (T)RemotingMock(typeof(T), CreateDynamicRecordState);

            return (T)CreateMockObject(typeof(T), CreateDynamicRecordState, new Type[0], argumentsForConstructor);
        }

        internal object DynamicMockWithRemoting(Type type, params object[] argumentsForConstructor)
        {
            return RemotingMock(type, CreateDynamicRecordState);
        }

        internal T DynamicMockWithRemoting<T>(params object[] argumentsForConstructor)
        {
            return (T)RemotingMock(typeof(T), CreateDynamicRecordState);
        }

        internal object DynamicMultiMock(Type mainType, params Type[] extraTypes)
        {
            return DynamicMultiMock(mainType, extraTypes, new object[0]);
        }

        internal T DynamicMultiMock<T>(params Type[] extraTypes)
        {
            return (T)DynamicMultiMock(typeof(T), extraTypes);
        }

        internal object DynamicMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return CreateMockObject(mainType, CreateDynamicRecordState, extraTypes, argumentsForConstructor);
        }

        internal T DynamicMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return (T)DynamicMultiMock(typeof(T), extraTypes, argumentsForConstructor);
        }
        
        internal object PartialMock(Type type, params object[] argumentsForConstructor)
        {
            return PartialMultiMock(type, new Type[0], argumentsForConstructor);
        }

        internal T PartialMock<T>(params object[] argumentsForConstructor) where T : class
        {
            return (T)PartialMock(typeof(T), argumentsForConstructor);
        }

        internal object PartialMultiMock(Type type, params Type[] extraTypes)
        {
            return PartialMultiMock(type, extraTypes, new object[0]);
        }

        internal T PartialMultiMock<T>(params Type[] extraTypes)
        {
            return (T)PartialMultiMock(typeof(T), extraTypes);
        }

        internal object PartialMultiMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor)
        {
            if (type.IsInterface)
                throw new InvalidOperationException("Can't create a partial mock from an interface");

            List<Type> extraTypesWithMarker = new List<Type>(extraTypes);
            extraTypesWithMarker.Add(typeof(IPartialMockMarker));

            return CreateMockObject(type, CreatePartialRecordState, extraTypesWithMarker.ToArray(), argumentsForConstructor);
        }

        internal T PartialMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return (T)PartialMultiMock(typeof(T), extraTypes, argumentsForConstructor);
        }
        
        internal object StrictMock(Type type, params object[] argumentsForConstructor)
        {
            if (ShouldUseRemotingProxy(type, argumentsForConstructor))
                return RemotingMock(type, CreateRecordState);

            return StrictMultiMock(type, new Type[0], argumentsForConstructor);
        }

        internal T StrictMock<T>(params object[] argumentsForConstructor)
        {
            if (ShouldUseRemotingProxy(typeof(T), argumentsForConstructor))
                return (T)RemotingMock(typeof(T), CreateRecordState);

            return (T)CreateMockObject(typeof(T), CreateRecordState, new Type[0], argumentsForConstructor);
        }

        internal object StrictMockWithRemoting(Type type, params object[] argumentsForConstructor)
        {
            return RemotingMock(type, CreateRecordState);
        }

        internal T StrictMockWithRemoting<T>(params object[] argumentsForConstructor)
        {
            return (T)RemotingMock(typeof(T), CreateRecordState);
        }

        internal object StrictMultiMock(Type mainType, params Type[] extraTypes)
        {
            return StrictMultiMock(mainType, extraTypes, new object[0]);
        }

        internal T StrictMultiMock<T>(params Type[] extraTypes)
        {
            return (T)StrictMultiMock(typeof(T), extraTypes);
        }

        internal object StrictMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
        {
            if (argumentsForConstructor == null)
                argumentsForConstructor = new object[0];

            return CreateMockObject(mainType, CreateRecordState, extraTypes, argumentsForConstructor);
        }

        internal T StrictMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return (T)StrictMultiMock(typeof(T), extraTypes, argumentsForConstructor);
        }

        internal object Stub(Type type, params object[] argumentsForConstructor)
        {
            CreateMockState createStub = mockedObject => new StubRecordMockState(mockedObject, this);

            if (ShouldUseRemotingProxy(type, argumentsForConstructor))
                return RemotingMock(type, createStub);

            return CreateMockObject(type, createStub, new Type[0], argumentsForConstructor);
        }

        internal T Stub<T>(params object[] argumentsForConstructor)
        {
            return (T)Stub(typeof(T), argumentsForConstructor);
        }

        internal IMethodOptions<T> LastMethodCall<T>(object mockedInstance)
        {
            object mock = GetMockObjectFromInvocationProxy(mockedInstance);
            IsMockObjectFromThisRepository(mock);
            return proxies[mock].GetLastMethodOptions<T>();
        }

        internal object MethodCall(IInvocation invocation, object proxy, MethodInfo method, object[] args)
        {
            // This can happen only if a vritual method call originated from 
            // the constructor, before Rhino Mocks knows about the existance 
            // of this proxy. Those type of calls will be ignored and not count
            // as expectations, since there is not way to relate them to the 
            // proper state.

            IMockState mockState;
            if (!proxies.TryGetValue(proxy, out mockState))
            {
                if (proxy is IPartialMockMarker)
                {
                    invocation.Proceed();
                    return invocation.ReturnValue;
                }

                return null;
            }

            GetMockedObject(proxy).MethodCall(method, args);
            return mockState.MethodCall(invocation, method, args);
        }

        internal object GetMockObjectFromInvocationProxy(object invocationProxy)
        {
            object proxy = delegateProxies[invocationProxy];
            if (proxy != null)
                return proxy;

            return invocationProxy;
        }

        internal void PopRecorder()
        {
            if (recorders.Count > 1)
                recorders.Pop();
        }

        internal void PushRecorder(IMethodRecorder newRecorder)
        {
            recorders.Push(newRecorder);
        }

        /// <summary>
        /// Register a call on a property behavior
        /// </summary>
        /// <param name="instance">proxied instance</param>
        protected internal void RegisterPropertyBehaviorOn(IMockedObject instance)
        {
            lastRepository = this;
            lastMockedObject = instance;

            proxies[instance].NotifyCallOnPropertyBehavior();
        }

        /// <summary>
        /// Changes the mock state to Replay
        /// </summary>
        /// <remarks>
        /// Any further call is compared to the ones that
        /// were called in the record state
        /// </remarks>
        /// <param name="obj">the object to move to Replay state</param>
        /// <param name="checkInsideOrdering"></param>
        protected internal void ReplayCore(object obj, bool checkInsideOrdering)
        {
            if (checkInsideOrdering)
                NotInsideOrderring();

            IsMockObjectFromThisRepository(obj);
            ClearLastProxy(obj);

            IMockState state = proxies[obj];
            proxies[obj] = state.Replay();

            foreach (IMockedObject dependentMock in GetMockedObject(obj).DependentMocks)
            {
                ReplayCore(dependentMock, checkInsideOrdering);
            }
        }

        /// <summary>
        /// Gets the proxy from a mocked object instance or 
        /// throws if the object is not a mocked object
        /// </summary>
        /// <param name="mockedInstance"></param>
        /// <returns></returns>
        protected internal static IMockedObject GetMockedObject(object mockedInstance)
        {
            IMockedObject mockedObj = GetMockedObjectOrNull(mockedInstance);
            if (mockedObj == null)
                throw new InvalidOperationException("The object '" + mockedInstance +
                                                    "' is not a mocked object.");
            return mockedObj;
        }

        /// <summary>
        /// Gets the proxy from a mocked object instance or
        /// null if the object is not a mocked object
        /// </summary>
        /// <param name="mockedInstance"></param>
        /// <returns></returns>
        protected internal static IMockedObject GetMockedObjectOrNull(object mockedInstance)
        {
            Delegate mockedDelegate = mockedInstance as Delegate;

            if (mockedDelegate != null)
            {
                mockedInstance = mockedDelegate.Target;
            }

            // must be careful not to call any methods on mocked objects,
            // or it may cause infinite recursion
            if (mockedInstance is IMockedObject)
            {
                return (IMockedObject)mockedInstance;
            }

            if (RemotingMockGenerator.IsRemotingProxy(mockedInstance))
            {
                return RemotingMockGenerator.GetMockedObjectFromProxy(mockedInstance);
            }

            return null;
        }

        /// <summary>
        /// Determines if the proxy is a Stub
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        protected internal static bool IsStub(object proxy)
        {
            MockRepository repository = GetMockedObject(proxy).Repository;

            IMockState mockState = repository.proxies[proxy];
            return (mockState is StubRecordMockState || mockState is StubReplayMockState);
        }

        /// <summary>
        /// Sets the exception to be thrown when Verify is called
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="expectationViolationException"></param>
        protected internal static void SetExceptionToBeThrownOnVerify(object proxy, ExpectationViolationException expectationViolationException)
        {
            MockRepository repository = GetMockedObject(proxy).Repository;
            ProxyStateDictionary dictionary = repository.proxies;

            IMockState mockState;
            if (!dictionary.TryGetValue(proxy, out mockState))
                return;

            mockState.SetExceptionToThrowOnVerify(expectationViolationException);
        }

        /// <summary>
        /// Gets the proxy generator for the specified type
        /// </summary>
        /// <remarks>
        /// Having a single generator with multiple types linearly
        /// degrades the performance so this implementation keeps
        /// a single generator per type
        /// </remarks>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual ProxyGenerator GetProxyGenerator(Type type)
        {
            ProxyGenerator generator;
            if (!generatorMap.TryGetValue(type, out generator))
            {
                generator = new ProxyGenerator();
                generatorMap[type] = generator;
            }

            return generator;
        }

        /// <summary>
        /// Generates a mocked object for the given type
        /// </summary>
        /// <param name="type">type to be mocked</param>
        /// <param name="factory">delegate to create the initial state (default Record)</param>
        /// <param name="extras">additional types to be implemented for interfaces</param>
        /// <param name="argumentsForConstructor">optional arguments for constructor</param>
        /// <returns></returns>
        protected object CreateMockObject(Type type, CreateMockState factory, Type[] extras, params object[] argumentsForConstructor)
        {
            foreach (Type extraType in extras)
            {
                if (!extraType.IsInterface)
                {
                    throw new ArgumentException("Extra types must all be interfaces", "extras");
                }
            }

            if (type.IsInterface)
            {
                if (argumentsForConstructor != null && argumentsForConstructor.Length > 0)
                {
                    throw new ArgumentException(
                        "Constructor arguments should not be supplied when mocking an interface",
                        "argumentsForConstructor");
                }

                return MockInterface(factory, type, extras);
            }
            else if (typeof(Delegate).IsAssignableFrom(type))
            {
                if (argumentsForConstructor != null && argumentsForConstructor.Length > 0)
                {
                    throw new ArgumentException("Constructor arguments should not be supplied when mocking a delegate",
                                                "argumentsForConstructor");
                }

                return MockDelegate(factory, type);
            }
            else
                return MockClass(factory, type, extras, argumentsForConstructor);
        }

        private void ClearLastProxy(object obj)
        {
            if (GetMockedObjectOrNull(obj) == lastMockedObject)
                lastMockedObject = null;
        }

        private IMockState CreateRecordState(IMockedObject mockedObject)
        {
            return new RecordMockState(mockedObject, this);
        }

        private IMockState CreateDynamicRecordState(IMockedObject mockedObject)
        {
            return new RecordDynamicMockState(mockedObject, this);
        }

        private IMockState CreatePartialRecordState(IMockedObject mockedObject)
        {
            return new RecordPartialMockState(mockedObject, this);
        }

        private void IsMockObjectFromThisRepository(object obj)
        {
            if (proxies.ContainsKey(obj) == false)
                throw new ObjectNotMockFromThisRepositoryException(
                    "The object is not a mock object that belong to this repository.");
        }

        private object MockClass(CreateMockState mockStateFactory, Type type, Type[] extras, object[] argumentsForConstructor)
        {
            if (type.IsSealed)
                throw new NotSupportedException("Can't create mocks of sealed classes");

            List<Type> collection = new List<Type>(extras);
            collection.Add(type);

            ProxyInstance proxyInstance = new ProxyInstance(this, collection.ToArray());
            RhinoInterceptor interceptor = new RhinoInterceptor(this, proxyInstance,
                    invocationVisitorsFactory.CreateStandardInvocationVisitors(proxyInstance, this));

            List<Type> types = new List<Type>(extras);
            types.Add(typeof(IMockedObject));

            object proxy;

            try
            {
                proxyGenerationOptions = ProxyGenerationOptions.Default;
                proxy = GetProxyGenerator(type).CreateClassProxy(type, types.ToArray(),
                        proxyGenerationOptions, argumentsForConstructor, interceptor);
            }
            catch (MissingMethodException mme)
            {
                throw new MissingMethodException("Can't find a constructor with matching arguments", mme);
            }
            catch (TargetInvocationException tie)
            {
                throw new Exception("Exception in constructor: " + tie.InnerException, tie.InnerException);
            }

            IMockedObject mockedObject = (IMockedObject)proxy;
            mockedObject.ConstructorArguments = argumentsForConstructor;

            IMockState value = mockStateFactory(mockedObject);
            proxies.Add(proxy, value);

            // avoid issues with expectations created/validated on the finalizer thread
            GC.SuppressFinalize(proxy);

            return proxy;
        }

        private object MockInterface(CreateMockState mockStateFactory, Type type, Type[] extras)
        {
            List<Type> collection = new List<Type>(extras);
            collection.Add(type);

            ProxyInstance proxyInstance = new ProxyInstance(this, collection.ToArray());
            RhinoInterceptor interceptor = new RhinoInterceptor(this, proxyInstance,
                    invocationVisitorsFactory.CreateStandardInvocationVisitors(proxyInstance, this));

            List<Type> types = new List<Type>(extras);
            types.Add(typeof(IMockedObject));

            object proxy = GetProxyGenerator(type).CreateInterfaceProxyWithoutTarget(type, types.ToArray(),
                    proxyGenerationOptions, interceptor);

            IMockState value = mockStateFactory((IMockedObject)proxy);
            proxies.Add(proxy, value);
            return proxy;
        }

        private object MockDelegate(CreateMockState mockStateFactory, Type type)
        {
            if (typeof(Delegate).Equals(type))
                throw new InvalidOperationException("Cannot mock the Delegate base type.");

            ProxyInstance proxyInstance = new ProxyInstance(this, type);
            RhinoInterceptor interceptor = new RhinoInterceptor(this, proxyInstance,
                    invocationVisitorsFactory.CreateStandardInvocationVisitors(proxyInstance, this));

            Type[] types = new Type[] { typeof(IMockedObject) };
            var delegateTargetInterface = delegateTargetInterfaceCreator.GetDelegateTargetInterface(type);
            object target = GetProxyGenerator(type).CreateInterfaceProxyWithoutTarget(delegateTargetInterface, types,
                    proxyGenerationOptions, interceptor);

            object proxy = Delegate.CreateDelegate(type, target, "Invoke");
            delegateProxies.Add(target, proxy);

            IMockState value = mockStateFactory(GetMockedObject(proxy));
            proxies.Add(proxy, value);
            return proxy;
        }

        private void NotInsideOrderring()
        {
            if (Recorder != rootRecorder)
                throw new InvalidOperationException(
                    "Can't start replaying because Ordered or Unordered properties were call and not yet disposed.");
        }

        private object RemotingMock(Type type, CreateMockState factory)
        {
            ProxyInstance rhinoProxy = new ProxyInstance(this, type);
            RhinoInterceptor interceptor = new RhinoInterceptor(this, rhinoProxy,
                invocationVisitorsFactory.CreateStandardInvocationVisitors(rhinoProxy, this));

            object transparentProxy = new RemotingMockGenerator()
                .CreateRemotingMock(type, interceptor, rhinoProxy);

            IMockState value = factory(rhinoProxy);
            proxies.Add(transparentProxy, value);

            return transparentProxy;
        }

        private static bool ShouldUseRemotingProxy(Type type, object[] argumentsForConstructor)
        {
            return typeof(MarshalByRefObject).IsAssignableFrom(type) &&
                (argumentsForConstructor == null || argumentsForConstructor.Length == 0);
        }
    }
}
