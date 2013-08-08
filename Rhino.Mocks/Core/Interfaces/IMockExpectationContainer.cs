using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Core.Expectations;

namespace Rhino.Mocks.Core.Interfaces
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
        ExpectationOptions GetMarkedExpectation();

        /// <summary>
        /// Returns all expectations that have
        /// been set for consideration
        /// </summary>
        /// <returns></returns>
        ExpectationOptions[] ListExpectations();

        /// <summary>
        /// Add an expectation into consideration
        /// </summary>
        /// <param name="expectation"></param>
        void MarkForExpectation(ExpectationOptions expectation);

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
