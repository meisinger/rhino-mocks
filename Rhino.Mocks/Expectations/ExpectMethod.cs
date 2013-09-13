using System;
using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Helpers;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Expectations
{
    /// <summary>
    /// Represents an expectation against a method call.
    /// Allows access to various options that can be applied to an expectation
    /// </summary>
    public class ExpectMethod : Expectation, IMethodOptions
    {
        private readonly IRepeatOptions repeatOptions;

        internal override ExpectationType Type
        {
            get { return ExpectationType.Method; }
        }

        /// <summary>
        /// Access to repeat options on expectation
        /// </summary>
        IRepeatOptions IMethodOptions.Repeat
        {
            get { return repeatOptions; }
        }
        
        /// <summary>
        /// Constraints against argument of the expectation
        /// </summary>
        public AbstractConstraint[] Arguments { get; set; }

        /// <summary>
        /// Method of the expectation
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Constructs a new method expectation
        /// </summary>
        public ExpectMethod()
            : this(new Range(1, 1))
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="range"></param>
        protected ExpectMethod(Range range)
            : base(range)
        {
            repeatOptions = new RepeatOptions(this);
        }

        /// <summary>
        /// Returns the string representation of the expectation
        /// </summary>
        /// <returns>string</returns>
        public override string GetDisplayName(IInvocation invocation)
        {
            return MethodFormatter.ToString(invocation, Method, Arguments,
                (x, i) => Arguments[i].Message);
        }

        /// <summary>
        /// Handles the method
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        public override void HandleMethodCall(MethodInfo method, object[] arguments)
        {
            Method = method;

            if (ArgumentManager.HasBeenUsed)
            {
                ArgumentManager.ValidateMethodSignature(method);
                Arguments = ArgumentManager.GetConstraints();
                ReturnArguments = ArgumentManager.GetReturnValues();
                SetExpectedCount(new Range(1, null));
                ArgumentManager.Clear();

                return;
            }

            var parameters = method.GetParameters();
            var constraints = new AbstractConstraint[parameters.Length];
            for (int index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                constraints[index] = (parameter.IsOut)
                    ? Is.Anything()
                    : Is.Equal(arguments[index]);
            }

            Arguments = constraints;
        }

        /// <summary>
        /// Checks that the given method and arguments
        /// match the expectation and argument constraints
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public override bool MatchesCall(MethodInfo method, object[] arguments)
        {
            if (!Method.Equals(method))
                return false;

            return MatchesCallArguments(arguments);
        }

        /// <summary>
        /// Checks that the given arguments match the argument constraints
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool MatchesCallArguments(object[] arguments)
        {
            if (Arguments == null && arguments == null)
                return true;

            if (Arguments == null || arguments == null)
                return false;

            if (Arguments.Length != arguments.Length)
                return false;

            for (int index = 0; index < Arguments.Length; index++)
            {
                var argument = arguments[index];
                var constraint = Arguments[index];

                if (!constraint.Eval(argument))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Call original method
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IMethodOptions IMethodOptions.CallOriginalMethod()
        {
            if (Method.IsAbstract)
            {
                var message = string.Format(
                    "Can't use CallOriginalMethod on method {0} because the method is abstract.",
                        Method.Name);

                throw new InvalidOperationException(message);
            }

            ForceProceed = true;
            return this;
        }

        /// <summary>
        /// Ignores all arguments removing any existing argument constraints
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IMethodOptions IMethodOptions.IgnoreArguments()
        {
            var constraints = new AbstractConstraint[Arguments.Length];
            for (int index = 0; index < Arguments.Length; index++)
                constraints[index] = Is.Anything();

            Arguments = constraints;
            return this;
        }

        /// <summary>
        /// Throw exception of the given type when
        /// the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IMethodOptions IMethodOptions.Throws<TException>()
        {
            ThrowsException = true;
            ExceptionToThrow = new TException();
            return this;
        }

        /// <summary>
        /// Throw exception of the given type when
        /// the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="exception"></param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions IMethodOptions.Throws<TException>(TException exception)
        {
            ThrowsException = true;
            ExceptionToThrow = exception;
            return this;
        }
    }

    /// <summary>
    /// Represents an expectation against a method call.
    /// Allows access to various options that can be applied to an expectation
    /// </summary>
    public class ExpectMethod<T> : Expectation<T>, IMethodOptions<T>
    {
        private readonly IRepeatOptions<T> repeatOptions;

        internal override ExpectationType Type
        {
            get { return ExpectationType.Method; }
        }

        /// <summary>
        /// Access to repeat options on expectation
        /// </summary>
        IRepeatOptions<T> IMethodOptions<T>.Repeat
        {
            get { return repeatOptions; }
        }

        /// <summary>
        /// Constraints against argument of the expectation
        /// </summary>
        public AbstractConstraint[] Arguments { get; set; }

        /// <summary>
        /// Method of the expectation
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Constructs a new method expectation
        /// </summary>
        public ExpectMethod()
            : this(new Range(1, 1))
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="range"></param>
        protected ExpectMethod(Range range)
            : base(range)
        {
            repeatOptions = new RepeatOptions<T>(this);
        }

        /// <summary>
        /// Returns the string representation of the expectation
        /// </summary>
        /// <returns>string</returns>
        public override string GetDisplayName(IInvocation invocation)
        {
            return MethodFormatter.ToString(invocation, Method, Arguments,
                (x, i) => Arguments[i].Message);
        }

        /// <summary>
        /// Handles the method
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        public override void HandleMethodCall(MethodInfo method, object[] arguments)
        {
            Method = method;

            if (ArgumentManager.HasBeenUsed)
            {
                ArgumentManager.ValidateMethodSignature(method);
                Arguments = ArgumentManager.GetConstraints();
                ReturnArguments = ArgumentManager.GetReturnValues();
                SetExpectedCount(new Range(1, null));
                ArgumentManager.Clear();

                return;
            }

            var parameters = method.GetParameters();
            var constraints = new AbstractConstraint[parameters.Length];
            for (int index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                constraints[index] = (parameter.IsOut)
                    ? Is.Anything()
                    : Is.Equal(arguments[index]);
            }

            Arguments = constraints;
        }

        /// <summary>
        /// Checks that the given method and arguments
        /// match the expectation and argument constraints
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public override bool MatchesCall(MethodInfo method, object[] arguments)
        {
            if (!Method.Equals(method))
                return false;

            return MatchesCallArguments(arguments);
        }

        /// <summary>
        /// Checks that the given arguments match the argument constraints
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool MatchesCallArguments(object[] arguments)
        {
            if (Arguments == null && arguments == null)
                return true;

            if (Arguments == null || arguments == null)
                return false;

            if (Arguments.Length != arguments.Length)
                return false;

            for (int index = 0; index < Arguments.Length; index++)
            {
                var argument = arguments[index];
                var constraint = Arguments[index];

                if (!constraint.Eval(argument))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Call original method
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> IMethodOptions<T>.CallOriginalMethod()
        {
            if (Method.IsAbstract)
            {
                var message = string.Format(
                    "Can't use CallOriginalMethod on method {0} because the method is abstract.",
                        Method.Name);

                throw new InvalidOperationException(message);
            }

            ForceProceed = true;
            return this;
        }

        /// <summary>
        /// Ignores all arguments removing any existing argument constraints
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> IMethodOptions<T>.IgnoreArguments()
        {
            var constraints = new AbstractConstraint[Arguments.Length];
            for (int index = 0; index < Arguments.Length; index++)
                constraints[index] = Is.Anything();

            Arguments = constraints;
            return this;
        }

        /// <summary>
        /// Define the return value of a method call (non-void)
        /// </summary>
        /// <param name="value">Value to return when method is called</param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> IMethodOptions<T>.Return(T value)
        {
            SetReturnValue(value);
            return this;
        }

        /// <summary>
        /// Define the return value of a method call (non-void)
        /// </summary>
        /// <param name="func">Value to return when method is called</param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> IMethodOptions<T>.Returns(Func<T> func)
        {
            SetReturnValue(func());
            return this;
        }
        
        /// <summary>
        /// Throw exception of the given type when
        /// the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> IMethodOptions<T>.Throws<TException>()
        {
            ThrowsException = true;
            ExceptionToThrow = new TException();
            return this;
        }

        /// <summary>
        /// Throw exception of the given type when
        /// the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="exception"></param>
        /// <returns>Fluid Interface</returns>
        IMethodOptions<T> IMethodOptions<T>.Throws<TException>(TException exception)
        {
            ThrowsException = true;
            ExceptionToThrow = exception;
            return this;
        }
    }
}
