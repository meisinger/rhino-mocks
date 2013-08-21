using System.Reflection;

namespace Rhino.Mocks.Interfaces
{
    /// <summary>
    /// An instance of a mocked expectation
    /// </summary>
    public interface IMockExpectation
    {
        /// <summary>
        /// Indicates whether or not the
        /// expectation has a return value
        /// </summary>
        bool HasReturnValue { get; }

        /// <summary>
        /// Indicates whether or not the
        /// mocked method is executed
        /// </summary>
        bool ForceProceed { get; }

        /// <summary>
        /// Method of the expectation
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// Return value for the expectation
        /// </summary>
        object ReturnValue { get; }
    }
}
