using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Core.Expectations;
using Rhino.Mocks.Core.Interfaces;

namespace Rhino.Mocks.Core
{
    /// <summary>
    /// An instance of a mocked object
    /// </summary>
    public class MockInstance : IMockInstance, IMockExpectationContainer
    {
        private readonly List<Actuals> actuals;
        private readonly List<ExpectationOptions> container;
        private readonly Stack<ExpectationOptions> stack;
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
            container = new List<ExpectationOptions>();
            stack = new Stack<ExpectationOptions>();
            hashcode = MockInstanceEquality.NextHash;
        }

        /// <summary>
        /// Returns the last expectation that has
        /// been marked for consideration
        /// </summary>
        /// <returns></returns>
        public ExpectationOptions GetMarkedExpectation()
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
        public ExpectationOptions[] ListExpectations()
        {
            return container.ToArray();
        }

        /// <summary>
        /// Set an expectation for consideration
        /// </summary>
        /// <param name="expectation"></param>
        public void MarkForAssertion(ExpectationOptions expectation)
        {
            ExpectationMarked = true;

            stack.Push(expectation);
        }

        /// <summary>
        /// Add an expectation into consideration
        /// </summary>
        /// <param name="expectation"></param>
        public void MarkForExpectation(ExpectationOptions expectation)
        {
            ExpectationMarked = true;

            container.Add(expectation);
            stack.Push(expectation);
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

            ExpectationOptions expectation = null;
            for (int entryIndex = 0; entryIndex < container.Count; entryIndex++)
            {
                var entry = container[entryIndex];
                if (!entry.MatchesCall(method, arguments))
                    continue;

                if (!entry.ExpectationMet)
                {
                    expectation = entry;
                    break;
                }
            }

            if (expectation == null)
            {
                // this could be where a strict call
                // would throw an exception

                var returnType = method.ReturnType;
                if (!returnType.IsValueType || returnType == typeof(void))
                    return null;

                return Activator.CreateInstance(returnType);
            }

            // should the expectation get the
            // actual option as well?
            expectation.IncrementActual();
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
    }
}
