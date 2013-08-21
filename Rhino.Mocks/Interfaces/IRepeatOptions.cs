
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
        IExpectationOptions Once();

        /// <summary>
        /// Expectation will be called twice
        /// </summary>
        /// <returns></returns>
        IExpectationOptions Twice();

        /// <summary>
        /// Expectation will be called exactly
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IExpectationOptions Times(int expected);
        
        /// <summary>
        /// Expectation will be called at least
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IExpectationOptions AtLeast(int expected);
        
        /// <summary>
        /// Expectation could be called as many
        /// times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IExpectationOptions AtMost(int expected);
        
        /// <summary>
        /// Expectation may be called
        /// </summary>
        /// <returns></returns>
        IExpectationOptions Any();

        /// <summary>
        /// Expectation will not be called
        /// </summary>
        /// <returns></returns>
        IExpectationOptions Never();
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
        IExpectationOptions<T> Once();

        /// <summary>
        /// Expectation will be called twice
        /// </summary>
        /// <returns></returns>
        IExpectationOptions<T> Twice();

        /// <summary>
        /// Expectation will be called exactly
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IExpectationOptions<T> Times(int expected);

        /// <summary>
        /// Expectation will be called at least
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IExpectationOptions<T> AtLeast(int expected);

        /// <summary>
        /// Expectation could be called as many
        /// times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        IExpectationOptions<T> AtMost(int expected);

        /// <summary>
        /// Expectation may be called
        /// </summary>
        /// <returns></returns>
        IExpectationOptions<T> Any();

        /// <summary>
        /// Expectation will not be called
        /// </summary>
        /// <returns></returns>
        IExpectationOptions<T> Never();
    }
}
