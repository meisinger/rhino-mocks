using System;

namespace Rhino.Mocks.Interfaces
{
    /// <summary>
    /// An instance of a mocked object
    /// </summary>
    public interface IMockInstance
    {
        /// <summary>
        /// Hash code of the mock instance that uniquely
        /// identifies the mock (not the same as GetHashCode())
        /// </summary>
        int HashCode { get; }

        /// <summary>
        /// Types implemented by the mock instance
        /// </summary>
        Type[] ImplementedTypes { get; }

        /// <summary>
        /// Indicates whether or not the mock instance
        /// is a partial mock
        /// </summary>
        bool IsPartialInstance { get; set; }

        /// <summary>
        /// Arguments for the constructor
        /// </summary>
        object[] ConstructorArguments { get; set; }

        /// <summary>
        /// Proxy of mocked object
        /// </summary>
        object ProxyInstance { get; set; }
    }
}
