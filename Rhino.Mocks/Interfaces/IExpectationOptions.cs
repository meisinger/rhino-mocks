using System;

namespace Rhino.Mocks.Interfaces
{
    /// <summary>
    /// Access to various options that can be applied to an expectation
    /// </summary>
    public interface IExpectationOptions
    {
        /// <summary>
        /// Throw exception of the given type when the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions Throws<TException>()
            where TException : Exception, new();

        /// <summary>
        /// Throw exception of the given type when the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="exception"></param>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions Throws<TException>(TException exception)
            where TException : Exception;
    }

    /// <summary>
    /// Access to various options that can be applied to an expectation
    /// </summary>
    public interface IExpectationOptions<T>
    {
        /// <summary>
        /// Throw exception of the given type when the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> Throws<TException>()
            where TException : Exception, new();

        /// <summary>
        /// Throw exception of the given type when the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="exception"></param>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> Throws<TException>(TException exception)
            where TException : Exception;
    }
}
