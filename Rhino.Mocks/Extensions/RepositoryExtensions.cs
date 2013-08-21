using System;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Helpers;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Asserts the given action was called against the 
        /// mocked object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static void AssertWasCalled<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Assertions cannot be made on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Assertions can only be made on a mocked object or instance.");

            var actuals = container.ListActuals();
            if (actuals.Length == 0)
                throw new Exception("Nope");

            var assertion = new Expectation();
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

            for (int index = 0; index < actuals.Length; index++)
            {
                var actual = actuals[index];
                if (assertion.MatchesCall(actual.Method, actual.Arguments))
                    return;
            }

            throw new Exception("Nope");
        }

        /// <summary>
        /// Asserts the given action was called against the 
        /// mocked object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="instance"></param>
        /// <param name="func"></param>
        public static void AssertWasCalled<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Assertions cannot be made on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Assertions can only be made on a mocked object or instance.");

            var actuals = container.ListActuals();
            if (actuals.Length == 0)
                throw new Exception("Nope");

            var assertion = new Expectation();
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

            for (int index = 0; index < actuals.Length; index++)
            {
                var actual = actuals[index];
                if (assertion.MatchesCall(actual.Method, actual.Arguments))
                    return;
            }

            throw new Exception("Nope");
        }

        /// <summary>
        /// Set expectation on an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static IExpectationOptions Expect<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Expectations cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Expectations can only be set on a mocked object or instance.");

            var expectation = new Expectation();
            container.MarkForExpectation(expectation);

            try
            {
                action(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception caught while setting up expectation", ex);
            }

            if (container.ExpectationMarked)
                throw new InvalidOperationException();

            return expectation;
        }

        /// <summary>
        /// Set expectation on an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="instance"></param>
        /// <param name="func"></param>
        public static IExpectationOptions<TResult> Expect<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Expectations cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Expectations can only be set on a mocked object or instance.");

            var expectation = new Expectation<TResult>();
            container.MarkForExpectation(expectation);

            try
            {
                func(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception caught while setting up expectation", ex);
            }

            if (container.ExpectationMarked)
                throw new InvalidOperationException();

            return expectation;
        }

        /// <summary>
        /// Set expectation for an event on an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static IExpectationOptions ExpectEvent<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Expectations cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Expectations can only be set on a mocked object or instance.");

            var expectation = new Expectation();
            container.MarkForExpectation(expectation);

            try
            {
                action(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception caught while setting up expectation", ex);
            }

            if (container.ExpectationMarked)
                throw new InvalidOperationException();

            var method = expectation.Method;
            if (!method.IsSpecialName)
                throw new InvalidOperationException("ExpectEvent method can only be used against events.");

            var methodName = method.Name;
            if (!methodName.StartsWith("add_") && !methodName.StartsWith("remove_"))
                throw new InvalidOperationException("ExpectEvent method can only be used against events.");

            return expectation;
        }

        /// <summary>
        /// Set stub on an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static IExpectationOptions Stub<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Stubs cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Stubs can only be set on a mocked object or instance.");

            var expectation = new Expectation();
            expectation.SetExpectedCount(new Range(int.MaxValue, int.MaxValue));
            container.MarkForExpectation(expectation);

            try
            {
                action(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception caught while setting up stub", ex);
            }

            if (container.ExpectationMarked)
                throw new InvalidOperationException();

            return expectation;
        }

        /// <summary>
        /// Set stub on an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="instance"></param>
        /// <param name="func"></param>
        public static IExpectationOptions<TResult> Stub<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Stubs cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Stubs can only be set on a mocked object or instance.");

            var expectation = new Expectation<TResult>();
            expectation.SetExpectedCount(new Range(int.MaxValue, int.MaxValue));
            container.MarkForExpectation(expectation);

            try
            {
                func(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception caught while setting up stub", ex);
            }

            if (container.ExpectationMarked)
                throw new InvalidOperationException();

            return expectation;
        }

        /// <summary>
        /// Set stub an event on an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static IExpectationOptions StubEvent<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Stubs cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Stubs can only be set on a mocked object or instance.");

            var expectation = new Expectation();
            container.MarkForExpectation(expectation);

            try
            {
                action(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception caught while setting up stub", ex);
            }

            if (container.ExpectationMarked)
                throw new InvalidOperationException();

            var method = expectation.Method;
            if (!method.IsSpecialName)
                throw new InvalidOperationException("StubEvent method can only be used against events.");

            var methodName = method.Name;
            if (!methodName.StartsWith("add_") && !methodName.StartsWith("remove_"))
                throw new InvalidOperationException("StubEvent method can only be used against events.");

            return expectation;
        }

        /// <summary>
        /// Stubs all of the available properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void StubProperties<T>(this T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Properties cannot be stubbed on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Properties can only be stubbed on a mocked object or instance.");

            var mock = instance as IMockInstance;
            if (mock == null)
                throw new ArgumentOutOfRangeException("instance", "Properties can only be stubbed on a mocked object or instance.");

            HandleProperties(container, mock, mock.ImplementedTypes);
        }

        /// <summary>
        /// Verifies all expectations have been met
        /// for the given mocked object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void VerifyAllExpectations<T>(this T instance)
        {
            VerifyExpectations(instance);
        }

        /// <summary>
        /// Verifies all expectations have been met
        /// for the given mocked object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void VerifyExpectations<T>(this T instance)
        {
            VerifyExpectations(instance, false);
        }

        /// <summary>
        /// Verifies all expectations have been met
        /// for the given mocked object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="strictly"></param>
        public static void VerifyExpectations<T>(this T instance, bool strictly)
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Verification cannot be performed on a null object or instance.");

            var invocation = instance as IInvocation;
            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Verification can only be performed on a mocked object or instance.");

            var actuals = container.ListActuals();
            var expectations = container.ListExpectations();
            if (expectations.Length == 0 && actuals.Length == 0)
                return;

            var unmet = expectations
                .Where(x => !x.ExpectationMet)
                .ToArray();

            if (!unmet.Any() && !strictly)
                return;

            var buffer = new StringBuilder();
            if (strictly)
            {
                for (int actualIndex = 0; actualIndex < actuals.Length; actualIndex++)
                {
                    var foundActual = false;
                    var actual = actuals[actualIndex];
                    for (int expectationIndex = 0; expectationIndex < expectations.Length; expectationIndex++)
                    {
                        var expectation = expectations[expectationIndex];
                        if (expectation.HandledActual(actual))
                        {
                            foundActual = true;
                            break;
                        }
                    }

                    if (foundActual)
                        continue;

                    var methodName = MethodFormatter.ToString(invocation, actual.Method, actual.Arguments);
                    buffer.Append(methodName)
                        .Append(" Expected # 0, Actual # 1.")
                        .AppendLine();
                }
            }

            for (int index = 0; index < unmet.Length; index++)
            {
                var item = unmet[index];
                var methodName = MethodFormatter.ToString(invocation, item.Method, item.Arguments,
                    (x, i) => item.Arguments[i].Message);

                buffer.Append(methodName)
                    .AppendFormat(" Expected # {0}, Actual # {1}.", item.ExpectedCount, item.ActualCount)
                    .AppendLine();
            }

            if (buffer.Length == 0)
                return;

            var message = buffer
                .Remove(buffer.Length - 2, 2)
                .ToString();

            throw new ExpectationViolationException(message);
        }

        private static IMockExpectationContainer GetExpectationContainer(object instance)
        {
            if (typeof(Delegate).IsAssignableFrom(instance.GetType()))
            {
                var instanceDelegate = (Delegate)instance;
                return instanceDelegate.Target as IMockExpectationContainer;
            }
            
            return instance as IMockExpectationContainer;
        }

        private static void HandleProperties(IMockExpectationContainer container, IMockInstance instance, params Type[] types)
        {
            if (types == null || types.Length == 0)
                return;

            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                var type = types[typeIndex];

                var interfaces = type.GetInterfaces();
                if (interfaces != null && interfaces.Length != 0)
                    HandleProperties(container, instance, interfaces);

                var properties = type.GetProperties();
                if (properties == null || properties.Length == 0)
                    continue;

                for (int index = 0; index < properties.Length; index++)
                {
                    var property = properties[index];
                    if (property.CanRead && property.CanWrite && property.PropertyType.IsValueType && 
                        (property.GetSetMethod(false) != null))
                    {
                        container.AddPropertyStub(property);
                    }
                }
            }
        }
    }
}
