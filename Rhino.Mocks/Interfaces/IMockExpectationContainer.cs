using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Expectations;

namespace Rhino.Mocks.Interfaces
{
    /// <summary>
    /// An instance capable of storing
    /// expectations
    /// </summary>
    public interface IMockExpectationContainer
    {
        /// <summary>
        /// Indicates an expectation has been added
        /// for consideration
        /// </summary>
        bool ExpectationMarked { get; }

        /// <summary>
        /// Returns the last expectation that has
        /// been marked for consideration
        /// </summary>
        /// <returns></returns>
        Expectation GetMarkedExpectation();

        /// <summary>
        /// Returns actual calls that have
        /// been made
        /// </summary>
        /// <returns></returns>
        Actuals[] ListActuals();

        /// <summary>
        /// Returns all expectations that have
        /// been set for consideration
        /// </summary>
        /// <returns></returns>
        Expectation[] ListExpectations();

        /// <summary>
        /// Add an expectation without consideration
        /// </summary>
        /// <param name="expectation"></param>
        void AddExpectation(Expectation expectation);

        /// <summary>
        /// Add a property stub without an expectation
        /// </summary>
        /// <param name="property"></param>
        void AddPropertyStub(PropertyInfo property);

        /// <summary>
        /// Set an expectation for consideration
        /// </summary>
        /// <param name="expectation"></param>
        void MarkForAssertion(Expectation expectation);

        /// <summary>
        /// Add an expectation into consideration
        /// </summary>
        /// <param name="expectation"></param>
        void MarkForExpectation(Expectation expectation);

        /// <summary>
        /// Removes the given expectation from
        /// consideration
        /// </summary>
        /// <param name="expectation"></param>
        void RemoveExpectation(Expectation expectation);

        /// <summary>
        /// Handle a method call for the underlying
        /// mocked object
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        object HandleMethodCall(IInvocation invocation, MethodInfo method, object[] arguments);
    }
}
