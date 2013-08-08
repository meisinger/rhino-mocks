using System;

namespace Rhino.Mocks.Core.Interfaces
{
    /// <summary>
    /// Access to varios options that can
    /// be applied to an expectation
    /// </summary>
    public interface IExpectationOptions
    {
        /// <summary>
        /// Access to repeat options on expectation
        /// </summary>
        IRepeatOptions Repeat { get; }

        /// <summary>
        /// Call the original method
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions CallOriginalMethod();

        /// <summary>
        /// Throw exception of the given type when
        /// the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions Throws<TException>()
            where TException : Exception, new();
    }

    /// <summary>
    /// Access to various options that can 
    /// be applied to an expectation
    /// </summary>
    public interface IExpectationOptions<T>
    {
        /// <summary>
        /// Access to repeat options on expectation
        /// </summary>
        IRepeatOptions<T> Repeat { get; }

        /// <summary>
        /// Define the return value of a method call (non-void)
        /// </summary>
        /// <param name="value">Value to return when method is called</param>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> Return(T value);

        /// <summary>
        /// Define the return value of a method call (non-void)
        /// </summary>
        /// <param name="func">Value to return when method is called</param>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> Return(Func<T> func);

        /// <summary>
        /// Call the original method
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> CallOriginalMethod();

        /// <summary>
        /// Throw exception of the given type when
        /// the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> Throws<TException>()
            where TException : Exception, new();
    }
}
