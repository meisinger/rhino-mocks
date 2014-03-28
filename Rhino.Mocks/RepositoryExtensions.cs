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
        /// Asserts the given method was called against the mocked object
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="action">the method to assert was called</param>
        /// <exception cref="Rhino.Mocks.Exceptions.ExpectationViolationException">thrown when the method was not called</exception>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        public static void AssertWasCalled<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Assertion cannot be performed on a null object or instance.");

            var invocation = instance as IInvocation;
            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Assertion can only be performed on a mocked object or instance.");

            var assertion = MockRepository.GetMethodCallArguments(instance, action);
            var methodName = assertion.GetDisplayName(invocation);

            var actuals = container.ListActuals();
            if (actuals.Any())
            {
                for (int index = 0; index < actuals.Length; index++)
                {
                    var actual = actuals[index];
                    if (assertion.MatchesCall(actual.Method, actual.Arguments))
                        return;
                }
            }

            throw new ExpectationViolationException(
                string.Format("{0} Expected #1, Actual #0.", methodName));
        }

        /// <summary>
        /// Asserts the given method was called against the mocked object
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <typeparam name="TResult">the return type of the method</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="func">the method to assert was called</param>
        /// <exception cref="Rhino.Mocks.Exceptions.ExpectationViolationException">thrown when the method was not called</exception>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        public static void AssertWasCalled<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Assertion cannot be performed on a null object or instance.");

            var invocation = instance as IInvocation;
            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Assertion can only be performed on a mocked object or instance.");

            var assertion = MockRepository.GetMethodCallArguments(instance, func);
            var methodName = assertion.GetDisplayName(invocation);

            var actuals = container.ListActuals();
            if (actuals.Any())
            {
                for (int index = 0; index < actuals.Length; index++)
                {
                    var actual = actuals[index];
                    if (assertion.MatchesCall(actual.Method, actual.Arguments))
                        return;
                }
            }

            throw new ExpectationViolationException(
                string.Format("{0} Expected #1, Actual #0.", methodName));
        }

        /// <summary>
        /// Asserts the given method was not called against the mocked object
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="action">the method to assert was called</param>
        /// <exception cref="Rhino.Mocks.Exceptions.ExpectationViolationException">thrown when the method was called</exception>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        public static void AssertWasNotCalled<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Assertion cannot be performed on a null object or instance.");

            var invocation = instance as IInvocation;
            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Assertion can only be performed on a mocked object or instance.");

            var assertion = MockRepository.GetMethodCallArguments(instance, action);
            var methodName = assertion.GetDisplayName(invocation);

            var actuals = container.ListActuals();
            if (!actuals.Any())
                return;

            for (int index = 0; index < actuals.Length; index++)
            {
                var actual = actuals[index];
                if (assertion.MatchesCall(actual.Method, actual.Arguments))
                    throw new ExpectationViolationException(
                        string.Format("{0} Expected #0, Actual #1.", methodName));
            }
        }

        /// <summary>
        /// Asserts the given method was not called against the mocked object
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <typeparam name="TResult">the return type of the method</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="func">the method to assert was called</param>
        /// <exception cref="Rhino.Mocks.Exceptions.ExpectationViolationException">thrown when the method was called</exception>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        public static void AssertWasNotCalled<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Assertion cannot be performed on a null object or instance.");

            var invocation = instance as IInvocation;
            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Assertion can only be performed on a mocked object or instance.");

            var assertion = MockRepository.GetMethodCallArguments(instance, func);
            var methodName = assertion.GetDisplayName(invocation);

            var actuals = container.ListActuals();
            if (!actuals.Any())
                return;

            for (int index = 0; index < actuals.Length; index++)
            {
                var actual = actuals[index];
                if (assertion.MatchesCall(actual.Method, actual.Arguments))
                    throw new ExpectationViolationException(
                        string.Format("{0} Expected #0, Actual #1.", methodName));
            }
        }

        /// <summary>
        /// Creates an expectation against the mocked object for the given method
        /// with a return type of void
        /// </summary>
        /// <example>
        /// The following is an example of how to setup an expectation against a method:
        /// <code>
        /// [Fact]
        /// public void Test() {
        ///     var mock = MockRepository.Mock{ILoggingService}();
        ///     mock.Expect(x => x.Log('User saved.');
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// Applicable for methods with a return type of void
        /// </remarks>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="action">the method to create the expectation against</param>
        /// <returns>expectation targeted for methods</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        /// <exception cref="System.InvalidOperationException">thrown when the method cannot be resolved</exception>
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
        /// Creates an expectation against the mocked object for the given method
        /// with a return type
        /// </summary>
        /// <example>
        /// The following is an example of how to setup an expectation against a method:
        /// <code>
        /// [Fact]
        /// public void Test() {
        ///     var mock = MockRepository.Mock{ICustomerService}();
        ///     mock.Expect(x => x.FindCustomer(1))
        ///         .Return(new Customer
        ///         {
        ///             Id = 1,
        ///         });
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// Applicable for methods with a return type
        /// </remarks>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <typeparam name="TResult">the return type of the method</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="func">the method to create the expectation against</param>
        /// <returns>expectation targeted for methods</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        /// <exception cref="System.InvalidOperationException">thrown when the method cannot be resolved</exception>
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
        /// Creates an expectation against the mocked object for an event
        /// </summary>
        /// <example>
        /// The following is an example of how to setup an expectation against an event:
        /// <code>
        /// [Fact]
        /// public void Test() {
        ///     var mock = MockRepository.Mock{Page}();
        ///     mock.ExpectEvent(x => x.OnLoad += null);
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// Applicable for events only
        /// </remarks>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="action">the event to create the expectation against</param>
        /// <returns>expectation targeted for events</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        /// <exception cref="System.InvalidOperationException">
        /// thrown when the event cannot be resolved or the given method is not an event
        /// </exception>
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
        /// Creates an expectation against the mocked object for a property
        /// </summary>
        /// <example>
        /// The following is an example of how to setup an expectation against a property:
        /// <code>
        /// [Fact]
        /// public void Test() {
        ///     var mock = MockRepository.Mock{ICustomer}();
        ///     mock.ExpectPRoperty(x => x.FirstName);
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// Applicable for properties only
        /// </remarks>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <typeparam name="TResult">the return type of the property</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="func">the property to create the expectation against</param>
        /// <returns>expectation targeted for properties</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        /// <exception cref="System.InvalidOperationException">thrown when the property cannot be resolved</exception>
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
        /// Returns all of the actual calls made against the given method
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="action">the method to retrieve the calls for</param>
        /// <returns>collection of the actual calls</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        public static Actuals[] GetArgumentsForCallsMadeOn<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Assertion cannot be performed on a null object or instance.");

            var invocation = instance as IInvocation;
            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Assertion can only be performed on a mocked object or instance.");

            var assertion = MockRepository.GetMethodCallArguments(instance, action);
            var methodName = assertion.GetDisplayName(invocation);

            var actuals = container.ListActuals();
            if (actuals.Any())
            {
                return actuals
                    .Where(x => assertion.MatchesCall(x.Method, x.Arguments))
                    .ToArray();
            }

            return new Actuals[0];
        }

        /// <summary>
        /// Returns all of the actual calls made against the given method
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <typeparam name="TResult">the return type of the method</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="func">the method to retrieve the calls for</param>
        /// <returns>collection of the actual calls</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        public static Actuals[] GetArgumentsForCallsMadeOn<T, TResult>(this T instance, Func<T, TResult> func)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Assertion cannot be performed on a null object or instance.");

            var invocation = instance as IInvocation;
            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Assertion can only be performed on a mocked object or instance.");

            var assertion = MockRepository.GetMethodCallArguments(instance, func);
            var methodName = assertion.GetDisplayName(invocation);

            var actuals = container.ListActuals();
            if (actuals.Any())
            {
                return actuals
                    .Where(x => assertion.MatchesCall(x.Method, x.Arguments))
                    .ToArray();
            }

            return new Actuals[0];
        }

        /// <summary>
        /// Provides the ability to raise an event that has had an expectation created
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="eventSubscription">the event that has had an expectation created</param>
        /// <param name="args">arguments used to the raise event</param>
        /// <returns>collection of the actual calls</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// thrown when the instance cannot be identified as a mocked object or the given method is not an event
        /// </exception>
        public static void Raise<T>(this T instance, Action<T> eventSubscription, object[] args)
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
        /// Provides the ability to raise an event that has had an expectation created
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="eventSubscription">the event that has had an expectation created</param>
        /// <param name="args">event arguments</param>
        /// <returns>collection of the actual calls</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// thrown when the instance cannot be identified as a mocked object or the given method is not an event
        /// </exception>
        public static void Raise<T>(this T instance, Action<T> eventSubscription, EventArgs args)
            where T : class
        {
            Raise(instance, eventSubscription, new object[] { null, args });
        }

        /// <summary>
        /// Creates a stub against the mocked object for the given method with a return type of void
        /// </summary>
        /// <example>
        /// The following is an example of how to setup a stub against a method:
        /// <code>
        /// [Fact]
        /// public void Test() {
        ///     var mock = MockRepository.Mock{ILoggingService}();
        ///     mock.Stub(x => x.Log('User saved.');
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// Applicable for methods with a return type of void
        /// </remarks>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="action">the method to create the stub against</param>
        /// <returns>stub targeted for methods</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        /// <exception cref="System.InvalidOperationException">thrown when the method cannot be resolved</exception>
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
        /// Creates a stub against the mocked object for the given method with a return type
        /// </summary>
        /// <example>
        /// The following is an example of how to setup a stub against a method:
        /// <code>
        /// [Fact]
        /// public void Test() {
        ///     var mock = MockRepository.Mock{ICustomerService}();
        ///     mock.Stub(x => x.FindCustomer(1))
        ///         .Return(new Customer
        ///         {
        ///             Id = 1,
        ///         });
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// Applicable for methods with a return type
        /// </remarks>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <typeparam name="TResult">the return type of the method</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="func">the method to create the stub against</param>
        /// <returns>stub targeted for methods</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        /// <exception cref="System.InvalidOperationException">thrown when the method cannot be resolved</exception>
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
        /// Creates an stub against the mocked object for an event to provide
        /// the ability to raise the event
        /// </summary>
        /// <example>
        /// The following is an example of how to setup a stub against an event:
        /// <code>
        /// [Fact]
        /// public void Test() {
        ///     var mock = MockRepository.Mock{Page}();
        ///     mock.StubEvent(x => x.OnLoad += null);
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// Applicable for events only
        /// </remarks>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance</param>
        /// <param name="action">the event to create the stub against</param>
        /// <returns>stub targeted for events</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        /// <exception cref="System.InvalidOperationException">
        /// thrown when the event cannot be resolved or the given method is not an event
        /// </exception>
        public static IEventOptions StubEvent<T>(this T instance, Action<T> action)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "Stubs cannot be set on a null object or instance.");

            var container = GetExpectationContainer(instance);
            if (container == null)
                throw new ArgumentOutOfRangeException("instance", "Stubs can only be set on a mocked object or instance.");

            var expectation = new ExpectEvent();
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

            var method = expectation.Method;
            if (!method.IsSpecialName)
                throw new InvalidOperationException("StubEvent method can only be used against events.");

            var methodName = method.Name;
            if (!methodName.StartsWith("add_") && !methodName.StartsWith("remove_"))
                throw new InvalidOperationException("StubEvent method can only be used against events.");

            return expectation;
        }

        /// <summary>
        /// Verifies expectations have been met for the given mocked object
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance to verify</param>
        /// <exception cref="Rhino.Mocks.Exceptions.ExpectationViolationException">thrown when expectations have not been met</exception>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        public static void VerifyAllExpectations<T>(this T instance)
        {
            VerifyExpectations(instance);
        }

        /// <summary>
        /// Verifies expectations have been met for the given mocked object
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance to verify</param>
        /// <exception cref="Rhino.Mocks.Exceptions.ExpectationViolationException">thrown when expectations have not been met</exception>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
        public static void VerifyExpectations<T>(this T instance)
        {
            VerifyExpectations(instance, false);
        }

        /// <summary>
        /// Verifies expectations have been met for the given mocked object.
        /// When strictly is "true" then methods called without an expectation will fail verification
        /// </summary>
        /// <typeparam name="T">the mocked type</typeparam>
        /// <param name="instance">the mocked instance to verify</param>
        /// <param name="strictly">"true" for strict verification, otherwise normal verification</param>
        /// <exception cref="Rhino.Mocks.Exceptions.ExpectationViolationException">
        /// thrown when expectations have not been met or (in the case of strict verification)
        /// if a method was called that was not setup with an expectation
        /// </exception>
        /// <exception cref="System.ArgumentNullException">thrown when the instance is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the instance cannot be identified as a mocked object</exception>
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
