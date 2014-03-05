using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Helpers;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Asserts the given method was called against the 
        /// mocked object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static void AssertWasCalled<T>(this T instance, Action<T> action)
            where T : class
        {
            var actuals = Repository.GetMethodCallArguments(instance, action);
            if (actuals.Any())
                return;

            throw new Exception("Nope");
        }

        /// <summary>
        /// Asserts the given method was called against the 
        /// mocked object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="instance"></param>
        /// <param name="func"></param>
        public static void AssertWasCalled<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            var actuals = Repository.GetMethodCallArguments(instance, func);
            if (actuals.Any())
                return;

            throw new Exception("Nope");
        }

        /// <summary>
        /// Asserts the given method was not called against the 
        /// mocked object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static void AssertWasNotCalled<T>(this T instance, Action<T> action)
            where T : class
        {
            var actuals = Repository.GetMethodCallArguments(instance, action);
            if (!actuals.Any())
                return;

            throw new Exception("Nope");
        }

        /// <summary>
        /// Asserts the given method was not called against the 
        /// mocked object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="instance"></param>
        /// <param name="func"></param>
        public static void AssertWasNotCalled<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            var actuals = Repository.GetMethodCallArguments(instance, func);
            if (!actuals.Any())
                return;

            throw new Exception("Nope");
        }

        /// <summary>
        /// Set expectation on an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static IMethodOptions Expect<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Expectations cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Expectations can only be set on a mocked object or instance.");

            var expectation = new ExpectMethod();
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
        public static IMethodOptions<TResult> Expect<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Expectations cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Expectations can only be set on a mocked object or instance.");

            var expectation = new ExpectMethod<TResult>();
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
        public static IEventOptions ExpectEvent<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Expectations cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Expectations can only be set on a mocked object or instance.");

            var expectation = new ExpectEvent();
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="instance"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IPropertyOptions<TResult> ExpectProperty<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Expectations cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Expectations can only be set on a mocked object or instance.");

            var expectation = new ExpectProperty<TResult>();
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="eventSubscription"></param>
        /// <param name="args"></param>
        public static void Raise<T>(this T instance, Action<T> eventSubscription, params object[] args)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Events cannot be raised from a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Events can only be raised from a mocked object or instance.");

            var expectation = new ExpectEvent();
            container.MarkForAssertion(expectation);

            try
            {
                eventSubscription(instance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception caught while identifying event", ex);
            }

            if (container.ExpectationMarked)
                throw new InvalidOperationException();

            var method = expectation.Method;
            if (!method.IsSpecialName)
                throw new InvalidOperationException("Raise method can only be used against events.");

            var methodName = method.Name;
            if (!methodName.StartsWith("add_"))
                throw new InvalidOperationException("Raise method can only be used against events.");

            var eventName = methodName.Substring(4);
            var subscription = container.GetEventSubscribers(eventName);
            if (subscription == null)
                return;

            var raiser = new EventRaiser(instance);
            raiser.Raise(subscription, args);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="eventSubscription"></param>
        /// <param name="args"></param>
        public static void Raise<T>(this T instance, Action<T> eventSubscription, EventArgs args)
            where T : class
        {
            Raise(instance, eventSubscription, new object[] { null, args });
        }

        /// <summary>
        /// Set stub on an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static IMethodOptions Stub<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Stubs cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Stubs can only be set on a mocked object or instance.");

            var expectation = new ExpectMethod();
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
        public static IMethodOptions<TResult> Stub<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Stubs cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Stubs can only be set on a mocked object or instance.");

            var expectation = new ExpectMethod<TResult>();
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
        public static IMethodOptions StubEvent<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Stubs cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Stubs can only be set on a mocked object or instance.");

            var expectation = new ExpectMethod();
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
                        .Append(" Expected #0, Actual #1.")
                        .AppendLine();
                }
            }

            for (int index = 0; index < unmet.Length; index++)
            {
                var item = unmet[index];
                var methodName = item.GetDisplayName(invocation);

                buffer.Append(methodName)
                    .AppendFormat(" Expected #{0}, Actual #{1}.", item.ExpectedCount, item.ActualCount)
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
                instance = instanceDelegate.Target;
            }

            if (instance is IMockExpectationContainer)
                return instance as IMockExpectationContainer;

            if (RepositoryForRemoting.IsRemotingProxy(instance))
            {
                var proxiedInstance = RepositoryForRemoting
                    .GetMockedInstanceFromProxy(instance);

                return proxiedInstance as IMockExpectationContainer;
            }

            return null;
        }
    }
}
