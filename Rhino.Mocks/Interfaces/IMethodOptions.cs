using System;

namespace Rhino.Mocks.Interfaces
{
    /// <summary>
    /// Access to various options that can be applied to a method
    /// </summary>
    public interface IMethodOptions
    {
        /// <summary>
        /// Access to repeat options on expectation
        /// </summary>
        IRepeatOptions Repeat { get; }

        /// <summary>
        /// Call the original method
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IMethodOptions CallOriginalMethod();

        /// <summary>
        /// Ignores all arguments removing any existing argument constraints
        /// </summary>
        /// <returns></returns>
        IMethodOptions IgnoreArguments();

        /// <summary>
        /// Throw exception of the given type when the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IMethodOptions Throws<TException>()
            where TException : Exception, new();

        /// <summary>
        /// Throw exception of the given type when the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="exception"></param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions Throws<TException>(TException exception)
            where TException : Exception;
    }

    /// <summary>
    /// Access to various options that can be applied to a method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMethodOptions<T>
    {
        /// <summary>
        /// Access to repeat options on expectation
        /// </summary>
        IRepeatOptions<T> Repeat { get; }

        /// <summary>
        /// Call the original method
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> CallOriginalMethod();

        /// <summary>
        /// Ignores all arguments removing any existing argument constraints
        /// </summary>
        /// <returns></returns>
        IMethodOptions<T> IgnoreArguments();

        /// <summary>
        /// Define the return value of a method call (non-void)
        /// </summary>
        /// <param name="value">Value to return when method is called</param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> Return(T value);

        /// <summary>
        /// Define the return value of a method call (non-void)
        /// </summary>
        /// <param name="func">Value to return when method is called</param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> Returns(Func<T> func);

        /// <summary>
        /// Throw exception of the given type when the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> Throws<TException>()
            where TException : Exception, new();

        /// <summary>
        /// Throw exception of the given type when the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="exception"></param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> Throws<TException>(TException exception)
            where TException : Exception;
    }
}
