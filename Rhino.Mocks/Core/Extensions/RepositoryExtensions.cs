using System;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Core.Expectations;
using Rhino.Mocks.Core.Helpers;
using Rhino.Mocks.Core.Interfaces;

namespace Rhino.Mocks.Core.Extensions
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Set expectation on an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="action"></param>
        public static IExpectationOptions ExpectCall<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Expectations cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Expectations can only be set on a mocked object or instance.");

            var expectation = new ExpectationOptions();
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
        public static IExpectationOptions<TResult> ExpectCall<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Expectations cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Expectations can only be set on a mocked object or instance.");

            var expectation = new ExpectationOptions<TResult>();
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

            var expectation = new ExpectationOptions();
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
            if (method.IsSpecialName)
                throw new InvalidOperationException("ExpectEvent method can only be used against events.");

            var methodName = method.Name;
            if (!methodName.StartsWith("add_") && !methodName.StartsWith("remove_"))
                throw new InvalidOperationException("ExpectEvent method can only be used against events.");

            return expectation;
        }

        /// <summary>
        /// Verifies all expectations have been met
        /// for the given mocked object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void VerifyExpectations<T>(this T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Verification cannot be performed on a null object or instance.");

            var invocation = instance as IInvocation;
            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Verification can only be performed on a mocked object or instance.");

            var expectations = container.ListExpectations();
            if (expectations.Length == 0)
                return;

            var buffer = new StringBuilder();
            for (int index = 0; index < expectations.Length; index++)
            {
                var expectation = expectations[index];
                if (expectation.ExpectationMet)
                    continue;

                var message = MethodFormatter.ToString(invocation, expectation.Method, expectation.Arguments,
                    (x, i) => expectation.Arguments[i].Message);

                buffer.Append(message)
                    .AppendFormat(" Expected # {0}, Actual # {1}.", expectation.ExpectedCount, expectation.ActualCount)
                    .AppendLine();
            }
            buffer.Remove(buffer.Length - 2, 2);

            throw new Exception(buffer.ToString());
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
    }
}
