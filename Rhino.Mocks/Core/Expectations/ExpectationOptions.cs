using System;
using System.Reflection;
using Rhino.Mocks.Core.Interfaces;
using Rhino.Mocks.Core.Helpers;
using Rhino.Mocks.Core.Constraints;

namespace Rhino.Mocks.Core.Expectations
{
    /// <summary>
    /// Access to various options that can 
    /// be applied to an expectation
    /// </summary>
    public class ExpectationOptions : IMockExpectation, IExpectationOptions
    {
        private readonly IRepeatOptions repeatOptions;

        private bool proceedIsForced;
        private bool throwsException;
        private int actualCount;
        private int expectedCount;

        /// <summary>
        /// Access to repeat options on expectation
        /// </summary>
        IRepeatOptions IExpectationOptions.Repeat
        {
            get { return repeatOptions; }
        }

        /// <summary>
        /// The number of times the expectation
        /// was actually called
        /// </summary>
        public int ActualCount
        {
            get { return actualCount; }
        }

        /// <summary>
        /// The number of times the expectation
        /// is expected to be called
        /// </summary>
        public int ExpectedCount
        {
            get { return expectedCount; }
        }

        /// <summary>
        /// Indicates whether or not the
        /// expectation has been satisfied
        /// </summary>
        public virtual bool ExpectationMet
        {
            get { return (actualCount >= expectedCount); }
        }

        /// <summary>
        /// Indicates whether or not the
        /// mocked method is executed
        /// </summary>
        public virtual bool ForceProceed
        {
            get { return proceedIsForced; }
        }

        /// <summary>
        /// Indicates whether or not an
        /// exception should be thrown
        /// </summary>
        public virtual bool ThrowsException
        {
            get { return throwsException; }
        }

        /// <summary>
        /// Indicates whether or not the
        /// expectation has a return type
        /// </summary>
        public virtual bool HasReturnValue
        {
            get { return false; }
        }

        /// <summary>
        /// Method of the expectation
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Constraints against argument of the 
        /// expectation
        /// </summary>
        public AbstractConstraint[] Arguments { get; set; }

        /// <summary>
        /// Expectation to throw if method is called
        /// </summary>
        public Exception ExceptionToThrow { get; set; }

        /// <summary>
        /// Collection of "out" and "ref" arguments
        /// </summary>
        public object[] ReturnArguments { get; set; }
        
        /// <summary>
        /// Return value for the expectation
        /// </summary>
        public virtual object ReturnValue
        {
            get { return null; }
        }
        
        /// <summary>
        /// constructor
        /// </summary>
        public ExpectationOptions()
            : this(1)
        {
            repeatOptions = new RepeatOptions(this);
        }

        /// <summary>
        /// protected constructor
        /// </summary>
        protected ExpectationOptions(int expected)
        {
            expectedCount = expected;
            actualCount = 0;
        }

        /// <summary>
        /// Increments actual call counter
        /// </summary>
        public void IncrementActual()
        {
            actualCount += 1;
        }

        /// <summary>
        /// Sets expected call counter
        /// </summary>
        /// <param name="expected"></param>
        public void SetExpectedCount(int expected)
        {
            expectedCount = expected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        public void HandleMethodCall(MethodInfo method, object[] arguments)
        {
            Method = method;

            if (ArgumentManager.HasBeenUsed)
            {
                ArgumentManager.ValidateMethodSignature(method);
                Arguments = ArgumentManager.GetConstraints();
                ReturnArguments = ArgumentManager.GetReturnValues();
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
        public bool MatchesCall(MethodInfo method, object[] arguments)
        {
            if (!Method.Equals(method))
                return false;

            return MatchesCallArguments(arguments);
        }

        /// <summary>
        /// Checks that the given arguments match the
        /// argument constraints
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
        IExpectationOptions IExpectationOptions.CallOriginalMethod()
        {
            proceedIsForced = true;
            return this;
        }
        
        /// <summary>
        /// Ignores all arguments removing any
        /// existing argument constraints
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions IExpectationOptions.IgnoreArguments()
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
        IExpectationOptions IExpectationOptions.Throws<TException>()
        {
            throwsException = true;

            ExceptionToThrow = new TException();
            return this;
        }
    }

    /// <summary>
    /// Access to various options that can 
    /// be applied to an expectation
    /// </summary>
    public class ExpectationOptions<T> : ExpectationOptions, IExpectationOptions<T>
    {
        private readonly IRepeatOptions<T> repeatOptions;

        private bool proceedIsForced;
        private bool throwsException;
        private bool returnValueSet;
        private T returnValue;

        /// <summary>
        /// Access to repeat options on expectation
        /// </summary>
        IRepeatOptions<T> IExpectationOptions<T>.Repeat
        {
            get { return repeatOptions; }
        }
        
        /// <summary>
        /// Indicates whether or not the
        /// mocked method is executed
        /// </summary>
        public override bool ForceProceed
        {
            get { return proceedIsForced; }
        }

        /// <summary>
        /// Indicates whether or not an
        /// exception should be thrown
        /// </summary>
        public override bool ThrowsException
        {
            get { return throwsException; }
        }

        /// <summary>
        /// Indicates whether or not a return value
        /// has been set
        /// </summary>
        public override bool HasReturnValue
        {
            get { return returnValueSet; }
        }

        /// <summary>
        /// Return value for the expectation
        /// </summary>
        public override object ReturnValue
        {
            get
            {
                if (returnValueSet)
                    return returnValue;

                var returnType = Method.ReturnType;
                if (!returnType.IsValueType || returnType == typeof(void))
                    return null;

                return Activator.CreateInstance(returnType);
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ExpectationOptions()
            : base(1)
        {
            repeatOptions = new RepeatOptions<T>(this);
        }

        /// <summary>
        /// Call original method
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> IExpectationOptions<T>.CallOriginalMethod()
        {
            proceedIsForced = true;
            return this;
        }

        /// <summary>
        /// Ignores all arguments removing any
        /// existing argument constraints
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> IExpectationOptions<T>.IgnoreArguments()
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
        public IExpectationOptions<T> Return(T value)
        {
            returnValue = value;
            returnValueSet = true;
            return this;
        }

        /// <summary>
        /// Define the return value of a method call (non-void)
        /// </summary>
        /// <param name="func">Value to return when method is called</param>
        /// <returns>Fluid Interface</returns>
        public IExpectationOptions<T> Return(Func<T> func)
        {
            returnValue = func();
            returnValueSet = true;
            return this;
        }

        /// <summary>
        /// Throw exception of the given type when
        /// the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> IExpectationOptions<T>.Throws<TException>()
        {
            throwsException = true;

            ExceptionToThrow = new TException();
            return this;
        }
    }
}
