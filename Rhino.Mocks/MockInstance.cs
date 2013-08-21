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
        private readonly Dictionary<string, object> properties;
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
            properties = new Dictionary<string, object>();
            hashcode = MockInstanceEquality.NextHash;
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
        /// Add a property stub without an expectation
        /// </summary>
        /// <param name="property"></param>
        public void AddPropertyStub(PropertyInfo property)
        {
            var type = property.PropertyType;
            if (!type.IsValueType)
                return;

            var setter = property.GetSetMethod(true);
            var value = Activator.CreateInstance(property.PropertyType);

            var key = GeneratePropertyKey(setter, new[] { value });
            if (!properties.ContainsKey(key))
                properties[key] = value;
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
        /// Handle a method call for the underlying
        /// mocked object
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public object HandleMethodCall(IInvocation invocation, MethodInfo method, object[] arguments)
        {
            if (arguments == null)
                arguments = new object[0];

            var actual = new Actuals(method, arguments);
            actuals.Add(actual);

            Expectation expectation = null;
            for (int entryIndex = 0; entryIndex < container.Count; entryIndex++)
            {
                var entry = container[entryIndex];
                if (!entry.MatchesCall(method, arguments))
                    continue;

                if (entry.ExpectationSatisfied)
                    continue;

                expectation = entry;
                break;
            }

            if (expectation == null)
            {
                //NOTE: this could be where a "strict" mock call would throw an exception

                if (IsPartialInstance && !method.IsAbstract)
                {
                    RhinoMocks.Logger.LogUnexpectedMethodCall(invocation, 
                        "Partial: Calling original method.");

                    invocation.Proceed();
                    return invocation.ReturnValue;
                }

                if (method.IsSpecialName)
                {
                    RhinoMocks.Logger.LogUnexpectedMethodCall(invocation,
                        "Property: Dynamic handling of property.");

                    var key = GeneratePropertyKey(method, arguments);

                    var methodName = method.Name;
                    if (methodName.StartsWith("get_", StringComparison.Ordinal))
                    {
                        if (properties.ContainsKey(key))
                            return properties[key];
                    }
                    else if (methodName.StartsWith("set_", StringComparison.Ordinal))
                    {
                        properties[key] = arguments.Last();
                        return null;
                    }
                }

                if (!method.IsSpecialName)
                {
                    RhinoMocks.Logger.LogUnexpectedMethodCall(invocation,
                        "Mock: No expectation or stub created.");
                }

                var returnType = method.ReturnType;
                if (!returnType.IsValueType || returnType == typeof(void))
                    return null;

                return Activator.CreateInstance(returnType);
            }

            RhinoMocks.Logger.LogExpectedMethodCall(invocation);

            expectation.AddActualCall(actual);
            if (expectation.ReturnArguments != null && expectation.ReturnArguments.Any())
            {
                var parameters = method.GetParameters();
                for (int index = 0, returnIndex = 0; index < parameters.Length; index++)
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
                return invocation.ReturnValue;
            }

            return expectation.ReturnValue;
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
    }
}
