
namespace Rhino.Mocks.Core.Helpers
{
    /// <summary>
    /// Provides a dummy field for reference parameters
    /// </summary>
    public class ByRefDummy<T>
    {
        /// <summary>
        /// Dummy field for compilation to be used
        /// with 'out' and 'ref' parameters
        /// </summary>
        public T Dummy;
    }
}
