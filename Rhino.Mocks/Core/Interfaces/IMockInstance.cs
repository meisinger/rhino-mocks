
namespace Rhino.Mocks.Core.Interfaces
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
        /// Arguments for the constructor
        /// </summary>
        object[] ConstructorArguments { get; set; }
        
        /// <summary>
        /// Proxy of mocked object
        /// </summary>
        object ProxyInstance { get; set; }
    }
}
