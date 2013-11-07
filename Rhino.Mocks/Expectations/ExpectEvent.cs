using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Helpers;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Expectations
{
    /// <summary>
    /// Represents an expectation against an event call.
    /// Allows access to various options that can be applied to an expectation
    /// </summary>
    public class ExpectEvent : Expectation, IEventOptions
    {
        internal override ExpectationType Type
        {
            get { return ExpectationType.Event; }
        }

        /// <summary>
        /// Constraints against argument of the expectation
        /// </summary>
        public AbstractConstraint[] Arguments { get; set; }

        /// <summary>
        /// Get Method of the expectation
        /// </summary>
        public MethodInfo Method { get; set; }
        
        /// <summary>
        /// constructor
        /// </summary>
        public ExpectEvent()
            : this(new Range(1, null))
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="range"></param>
        protected ExpectEvent(Range range)
            : base(range)
        {
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
        /// Handles the call
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

            var argumentNullCount = 0;
            var parameters = method.GetParameters();
            var constraints = new AbstractConstraint[parameters.Length];
            for (int index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.IsOut)
                    constraints[index] = Is.Anything();
                else
                {
                    var argument = arguments[index];
                    if (argument == null)
                        argumentNullCount++;

                    constraints[index] = Is.Equal(argument);
                }
            }

            if (argumentNullCount == parameters.Length)
            {
                for (int index = 0; index < parameters.Length; index++)
                    constraints[index] = Is.Anything();
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
        /// Ignores all arguments removing any existing argument constraints
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IEventOptions IEventOptions.IgnoreArguments()
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
        IEventOptions IEventOptions.Throws<TException>()
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
        IEventOptions IEventOptions.Throws<TException>(TException exception)
        {
            ThrowsException = true;
            ExceptionToThrow = exception;
            return this;
        }
    }
}
