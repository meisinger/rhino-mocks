
namespace Rhino.Mocks.Interfaces
{
    /// <summary>
    /// Access to repeatable options
    /// on expectation
    /// </summary>
    public interface IRepeatOptions
    {
        /// <summary>
        /// Expectation will be called once
        /// </summary>
        /// <returns></returns>
        IMethodOptions Once();

        /// <summary>
        /// Expectation will be called twice
        /// </summary>
        /// <returns></returns>
        IMethodOptions Twice();

        /// <summary>
        /// Expectation will be called exactly
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IMethodOptions Times(int expected);
        
        /// <summary>
        /// Expectation will be called at least
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IMethodOptions AtLeast(int expected);
        
        /// <summary>
        /// Expectation could be called as many
        /// times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IMethodOptions AtMost(int expected);
        
        /// <summary>
        /// Expectation may be called
        /// </summary>
        /// <returns></returns>
        IMethodOptions Any();

        /// <summary>
        /// Expectation will not be called
        /// </summary>
        /// <returns></returns>
        IMethodOptions Never();
    }

    /// <summary>
    /// Access to repeatable options
    /// on expectation
    /// </summary>
    public interface IRepeatOptions<T>
    {
        /// <summary>
        /// Expectation will be called once
        /// </summary>
        /// <returns></returns>
        IMethodOptions<T> Once();

        /// <summary>
        /// Expectation will be called twice
        /// </summary>
        /// <returns></returns>
        IMethodOptions<T> Twice();

        /// <summary>
        /// Expectation will be called exactly
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IMethodOptions<T> Times(int expected);

        /// <summary>
        /// Expectation will be called at least
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IMethodOptions<T> AtLeast(int expected);

        /// <summary>
        /// Expectation could be called as many
        /// times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IMethodOptions<T> AtMost(int expected);

        /// <summary>
        /// Expectation may be called
        /// </summary>
        /// <returns></returns>
        IMethodOptions<T> Any();

        /// <summary>
        /// Expectation will not be called
        /// </summary>
        /// <returns></returns>
        IMethodOptions<T> Never();
    }
}
