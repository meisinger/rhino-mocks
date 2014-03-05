using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks
{
    /// <summary>
    /// An instance of a mocked object
    /// </summary>
    public class MockInstance : IMockInstance, IMockExpectationContainer
    {
        private readonly List<Actuals> actuals;
        private readonly List<Expectation> container;
        private readonly Stack<Expectation> stack;
        private readonly Dictionary<string, object> dynamicProperties;
        private readonly Dictionary<string, Delegate> eventSubscriptions;
        private readonly Dictionary<string, Expectation> expectedProperties;
        private readonly Type[] types;
        private readonly int hashcode;

        /// <summary>
        /// Hash code of the mock instance that uniquely
        /// identifies the mock (not the same as GetHashCode())
        /// </summary>
        public int HashCode
        {
            get { return hashcode; }
        }

        /// <summary>
        /// Types implemented by the mock instance
        /// </summary>
        public Type[] ImplementedTypes
        {
            get { return types; }
        }

        /// <summary>
        /// Indicates whether or not the mock instance
        /// is a partial mock
        /// </summary>
        public bool IsPartialInstance { get; set; }

        /// <summary>
        /// Indicates an expectation has been added
        /// for consideration
        /// </summary>
        public bool ExpectationMarked { get; private set; }

        /// <summary>
        /// Arguments for the constructor
        /// </summary>
        public object[] ConstructorArguments { get; set; }

        /// <summary>
        /// Proxy of mocked object
        /// </summary>
        public object ProxyInstance { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="types"></param>
        public MockInstance(Type[] types)
        {
            this.types = types;

            actuals = new List<Actuals>();
            container = new List<Expectation>();
            stack = new Stack<Expectation>();
            dynamicProperties = new Dictionary<string, object>();
            eventSubscriptions = new Dictionary<string, Delegate>();
            expectedProperties = new Dictionary<string, Expectation>();
            hashcode = MockInstanceEquality.NextHash;
        }

        /// <summary>
        /// Returns subscribers that have been added
        /// for the given event
        /// </summary>
        /// <param name="name">event name</param>
        /// <returns></returns>
        public Delegate GetEventSubscribers(string name)
        {
            if (!eventSubscriptions.ContainsKey(name))
                return null;

            return eventSubscriptions[name];
        }

        /// <summary>
        /// Returns the last expectation that has
        /// been marked for consideration
        /// </summary>
        /// <returns></returns>
        public Expectation GetMarkedExpectation()
        {
            ExpectationMarked = false;
            return stack.Pop();
        }

        /// <summary>
        /// Returns actual calls that have
        /// been made
        /// </summary>
        /// <returns></returns>
        public Actuals[] ListActuals()
        {
            return actuals.ToArray();
        }

        /// <summary>
        /// Returns all expectations that have
        /// been set for consideration
        /// </summary>
        /// <returns></returns>
        public Expectation[] ListExpectations()
        {
            return container.ToArray();
        }

        /// <summary>
        /// Add an expectation without consideration
        /// </summary>
        /// <param name="expectation"></param>
        public void AddExpectation(Expectation expectation)
        {
            container.Add(expectation);
        }

        /// <summary>
        /// Set an expectation for consideration
        /// </summary>
        /// <param name="expectation"></param>
        public void MarkForAssertion(Expectation expectation)
        {
            ExpectationMarked = true;

            stack.Push(expectation);
        }

        /// <summary>
        /// Add an expectation into consideration
        /// </summary>
        /// <param name="expectation"></param>
        public void MarkForExpectation(Expectation expectation)
        {
            ExpectationMarked = true;

            container.Add(expectation);
            stack.Push(expectation);
        }

        /// <summary>
        /// Removes the given expectation from
        /// consideration
        /// </summary>
        /// <param name="expectation"></param>
        public void RemoveExpectation(Expectation expectation)
        {
            container.Remove(expectation);
        }

        /// <summary>
        /// Handle a method call for the underlying mocked object
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public object HandleMethodCall(IInvocation invocation, MethodInfo method, object[] arguments)
        {
            if (arguments == null)
                arguments = new object[0];

            if (method.IsSpecialName)
            {
                var methodName = method.Name;
                if (methodName.StartsWith("get_", StringComparison.Ordinal) ||
                    methodName.StartsWith("set_", StringComparison.Ordinal))
                {
                    if (container.Any(x => x.Type == ExpectationType.Property))
                        return HandlePropertyCall(invocation, method, arguments);
                }

                if (methodName.StartsWith("add_", StringComparison.Ordinal) ||
                    methodName.StartsWith("remove_", StringComparison.Ordinal))
                {
                    if (container.Any(x => x.Type == ExpectationType.Event))
                        return HandleEventCall(invocation, method, arguments);
                }
            }

            var actual = new Actuals(method, arguments);
            actuals.Add(actual);

            var methodCollection = container
                .Where(x => x.Type == ExpectationType.Method)
                .ToArray();

            Expectation expectation = null;
            for (int entryIndex = 0; entryIndex < methodCollection.Length; entryIndex++)
            {
                var entry = container[entryIndex];
                if (!entry.MatchesCall(method, arguments))
                    continue;

                if (entry.ExpectationSatisfied)
                    continue;

                expectation = entry;
                break;
            }

            //NOTE: this could be where a "strict" mock call would throw an exception
            if (expectation == null)
                return HandleUnexpectedMethodCall(invocation, method, arguments);
            
            RhinoMocks.Logger.LogExpectedMethodCall(invocation);
            expectation.AddActualCall(actual);

            if (expectation.HasDelegateToInvoke)
            {
                var methodParameters = method.GetParameters();

                var callback = expectation.DelegateToInvoke;
                var callbackParameters = callback.Method.GetParameters();
                
                try
                {
                    if (callbackParameters.Length == 0)
                    {
                        var value = callback.DynamicInvoke(new object[0]);

                        if (callback.Method.ReturnType != (typeof(void)) &&
                            expectation.DelegateReturnsValue)
                            expectation.SetReturnValue(value);
                    }
                    else
                    {
                        var invokeCallback = true;
                        var invokeArguments = new object[callbackParameters.Length];
                        for (int index = 0; index < callbackParameters.Length; index++)
                        {
                            var parameter = callbackParameters[index];
                            var parameterType = parameter.ParameterType;

                            var argument = methodParameters[index];
                            var argumentType = argument.ParameterType;

                            if (!parameterType.IsAssignableFrom(argumentType))
                            {
                                invokeCallback = false;
                                break;
                            }
                        }

                        if (invokeCallback)
                        {
                            var value = callback.DynamicInvoke(arguments);

                            if (callback.Method.ReturnType != (typeof(void)) &&
                                expectation.DelegateReturnsValue)
                                expectation.SetReturnValue(value);
                        }
                    }
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }

            if (expectation.ReturnArguments != null && expectation.ReturnArguments.Any())
            {
                var lenth = expectation.ReturnArguments.Length;
                var parameters = method.GetParameters();
                for (int index = 0, returnIndex = 0; index < parameters.Length && returnIndex < lenth; index++)
                {
                    var parameter = parameters[index];
                    if (!parameter.IsOut && !parameter.ParameterType.IsByRef)
                        continue;

                    arguments[index] = expectation.ReturnArguments[returnIndex];
                    returnIndex++;
                }
            }

            if (expectation.ThrowsException)
                throw expectation.ExceptionToThrow;

            if (expectation.ForceProceed)
            {
                invocation.Proceed();

                if (expectation.HasDelegateToIntercept)
                    expectation.DelegateToIntercept(new MethodInvocation(invocation));

                return invocation.ReturnValue;
            }

            invocation.ReturnValue = expectation.ReturnValue;

            if (expectation.HasDelegateToIntercept)
                expectation.DelegateToIntercept(new MethodInvocation(invocation));

            return invocation.ReturnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public object HandleEventCall(IInvocation invocation, MethodInfo method, object[] arguments)
        {
            var actual = new Actuals(method, arguments);
            actuals.Add(actual);

            var subscription = (Delegate)arguments[0];
            HandleEventSubscription(method, subscription);

            var eventCollection = container
                .Where(x => x.Type == ExpectationType.Event)
                .ToArray();

            Expectation expectation = null;
            for (int entryIndex = 0; entryIndex < eventCollection.Length; entryIndex++)
            {
                var entry = container[entryIndex];
                if (!entry.MatchesCall(method, arguments))
                    continue;

                if (entry.ExpectationSatisfied)
                    continue;

                expectation = entry;
                break;
            }

            //NOTE: this could be where a "strict" mock call would throw an exception
            if (expectation == null)
                return HandleUnexpectedMethodCall(invocation, method, arguments);

            RhinoMocks.Logger.LogExpectedMethodCall(invocation);
            expectation.AddActualCall(actual);

            if (expectation.ThrowsException)
                throw expectation.ExceptionToThrow;

            if (expectation.ForceProceed)
            {
                invocation.Proceed();
                return invocation.ReturnValue;
            }

            return expectation.ReturnValue;
        }

        /// <summary>
        /// Handles a property call
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public object HandlePropertyCall(IInvocation invocation, MethodInfo method, object[] arguments)
        {
            var actual = new Actuals(method, arguments);
            actuals.Add(actual);

            var methodName = method.Name;
            var propertyKey = GeneratePropertyKey(method, arguments);

            var propertyCollection = container
                .Where(x => x.Type == ExpectationType.Property)
                .ToArray();

            Expectation expectation = null;
            for (int entryIndex = 0; entryIndex < propertyCollection.Length; entryIndex++)
            {
                var entry = container[entryIndex];
                if (!entry.MatchesCall(method, arguments))
                    continue;

                if (entry.ExpectationSatisfied)
                    continue;

                expectation = entry;
                break;
            }

            //NOTE: this could be where a "strict" mock call would throw an exception
            if (expectation == null)
                RhinoMocks.Logger.LogUnexpectedMethodCall(invocation, 
                    "Property: Dynamic handling of property.");
            else
            {
                RhinoMocks.Logger.LogExpectedMethodCall(invocation);
                expectation.AddActualCall(actual);

                if (expectation.ThrowsException)
                    throw expectation.ExceptionToThrow;

                if (methodName.StartsWith("get_", StringComparison.Ordinal))
                    return expectation.ReturnValue;

                if (expectation.ForceProceed)
                {
                    invocation.Proceed();
                    return invocation.ReturnValue;
                }
            }

            if (methodName.StartsWith("get_", StringComparison.Ordinal))
            {
                if (dynamicProperties.ContainsKey(propertyKey))
                    return dynamicProperties[propertyKey];
            }

            if (methodName.StartsWith("set_", StringComparison.Ordinal))
            {
                if (expectation != null && expectation.HasReturnValue)
                    dynamicProperties[propertyKey] = expectation.ReturnValue;
                else
                    dynamicProperties[propertyKey] = arguments.Last();
            }

            return null;
        }

        /// <summary>
        /// Handles an unexpected method call
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public object HandleUnexpectedMethodCall(IInvocation invocation, MethodInfo method, object[] arguments)
        {
            if (IsPartialInstance && !method.IsAbstract)
            {
                RhinoMocks.Logger.LogUnexpectedMethodCall(invocation,
                    "Partial: Calling original method.");

                invocation.Proceed();
                return invocation.ReturnValue;
            }

            if (method.IsSpecialName)
            {
                var methodName = method.Name;
                if (methodName.StartsWith("get_", StringComparison.Ordinal) ||
                    methodName.StartsWith("set_", StringComparison.Ordinal))
                {
                    RhinoMocks.Logger.LogUnexpectedMethodCall(invocation,
                        "Property: Dynamic handling of property.");

                    var propertyKey = GeneratePropertyKey(method, arguments);
                    if (expectedProperties.ContainsKey(propertyKey))
                        expectedProperties.Remove(propertyKey);


                    if (methodName.StartsWith("get_", StringComparison.Ordinal))
                    {
                        if (dynamicProperties.ContainsKey(propertyKey))
                            return dynamicProperties[propertyKey];
                    }
                    else if (methodName.StartsWith("set_", StringComparison.Ordinal))
                    {
                        dynamicProperties[propertyKey] = arguments.Last();
                        return null;
                    }
                }

                if (methodName.StartsWith("add_", StringComparison.Ordinal) ||
                    methodName.StartsWith("remove_", StringComparison.Ordinal))
                {
                    RhinoMocks.Logger.LogUnexpectedMethodCall(invocation,
                        "Event: Dynamic handling of event.");

                    var subscription = (Delegate)arguments[0];
                    HandleEventSubscription(method, subscription);
                }
            }

            if (!method.IsSpecialName)
            {
                RhinoMocks.Logger.LogUnexpectedMethodCall(invocation,
                    "Mock: No expectation or stub created.");
            }

            return GetDefaultValue(method.ReturnType);
        }

        private void HandleEventSubscription(MethodInfo method, Delegate subscription)
        {
            var methodName = method.Name;
            if (methodName.StartsWith("remove_"))
            {
                var removeName = methodName.Substring(7);
                if (!eventSubscriptions.ContainsKey(removeName))
                    return;

                Delegate removeDelegate = (MulticastDelegate)eventSubscriptions[removeName];
                removeDelegate = MulticastDelegate.Remove(removeDelegate, subscription);
                eventSubscriptions[removeName] = removeDelegate;
                return;
            }

            var addName = methodName.Substring(4);
            if (!eventSubscriptions.ContainsKey(addName))
            {
                eventSubscriptions[addName] = subscription;
                return;
            }

            Delegate addDelegate = (MulticastDelegate)eventSubscriptions[addName];
            addDelegate = MulticastDelegate.Combine(addDelegate, subscription);
            eventSubscriptions[addName] = addDelegate;
        }

        private static string GeneratePropertyKey(MethodInfo method, object[] arguments)
        {
            var methodName = method.Name;
            var name = string.Concat(method.DeclaringType.FullName, methodName.Substring(4));
            if ((methodName.StartsWith("get_", StringComparison.Ordinal) && arguments.Length == 0) ||
                (methodName.StartsWith("set_", StringComparison.Ordinal) && arguments.Length == 1))
            {
                return name;
            }

            var buffer = new StringBuilder()
                .Append(name);

            var length = arguments.Length;
            if (methodName.StartsWith("set_", StringComparison.Ordinal))
                length--;

            for (int index = 0; index < length; index++)
                buffer.Append(arguments[index].GetHashCode());

            return buffer.ToString();

        }

        private static object GetDefaultValue(Type type)
        {
            if (!type.IsValueType || type == typeof(void))
                return null;

            return Activator.CreateInstance(type);
        }
    }
}
