using System;

namespace Rhino.Mocks.Interfaces
{
    /// <summary>
    /// Access to various options that can
    /// be applied only to "get" properties
    /// </summary>
    public interface IPropertyOptions<T>
    {
        /// <summary>
        /// Call the original property
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> CallOriginalProperty();

        /// <summary>
        /// Ignores all arguments removing any existing argument constraints
        /// </summary>
        /// <returns></returns>
        IPropertyOptions<T> IgnoreArguments();

        /// <summary>
        /// Define the return value of a property (non write-only)
        /// </summary>
        /// <param name="value">Value to return when "get" is called</param>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> Return(T value);

        /// <summary>
        /// Define the return value of a property (non write-only)
        /// </summary>
        /// <param name="func">Value to return when "get" is called</param>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> Returns(Func<T> func);

        /// <summary>
        /// Throw exception of the given type when the property is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> Throws<TException>()
            where TException : Exception, new();

        /// <summary>
        /// Throw exception of the given type when the property is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="exception"></param>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> Throws<TException>(TException exception)
            where TException : Exception;
    }
}
