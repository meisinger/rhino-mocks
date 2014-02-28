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
        ///
        /// </summary>
        /// <param name="action"></param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions DoInstead(Delegate action);

        /// <summary>
        /// Ignores all arguments removing any existing argument constraints
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IMethodOptions IgnoreArguments();

        /// <summary>
        ///
        /// </summary>
        /// <param name="action"></param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions Intercept(Action<MethodInvocation> action);

        /// <summary>
        /// Set the parameter values for [out] and [ref] parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions OutRef(params object[] parameters);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IMethodOptions WhenCalled(Action action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        IMethodOptions WhenCalled<TArg>(Action<TArg> action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        IMethodOptions WhenCalled<TArg1, TArg2>(Action<TArg1, TArg2> action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        IMethodOptions WhenCalled<TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        IMethodOptions WhenCalled<TArg1, TArg2, TArg3, TArg4>(Action<TArg1, TArg2, TArg3, TArg4> action);
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
        ///
        /// </summary>
        /// <param name="action"></param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> DoInstead(Delegate action);

        /// <summary>
        /// Ignores all arguments removing any existing argument constraints
        /// </summary>
        /// <returns></returns>
        IMethodOptions<T> IgnoreArguments();

        /// <summary>
        ///
        /// </summary>
        /// <param name="action"></param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> Intercept(Action<MethodInvocation> action);

        /// <summary>
        /// Set the parameter values for [out] and [ref] parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> OutRef(params object[] parameters);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IMethodOptions<T> WhenCalled(Action action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        IMethodOptions<T> WhenCalled<TArg>(Action<TArg> action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        IMethodOptions<T> WhenCalled<TArg1, TArg2>(Action<TArg1, TArg2> action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        IMethodOptions<T> WhenCalled<TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        IMethodOptions<T> WhenCalled<TArg1, TArg2, TArg3, TArg4>(Action<TArg1, TArg2, TArg3, TArg4> action);
    }
}
