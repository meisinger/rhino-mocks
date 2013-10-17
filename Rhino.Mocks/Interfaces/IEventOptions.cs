using System;

namespace Rhino.Mocks.Interfaces
{
    /// <summary>
    /// Access to various options that can
    /// be applied to expectations for events
    /// </summary>
    public interface IEventOptions
    {
        /// <summary>
        /// Ignores all arguments removing any existing argument constraints
        /// </summary>
        /// <returns></returns>
        IEventOptions IgnoreArguments();

        /// <summary>
        /// Throw exception of the given type when the property is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IEventOptions Throws<TException>()
            where TException : Exception, new();

        /// <summary>
        /// Throw exception of the given type when the property is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="exception"></param>
        /// <returns>Fluid Interface</returns>
        IEventOptions Throws<TException>(TException exception)
            where TException : Exception;
    }
}
