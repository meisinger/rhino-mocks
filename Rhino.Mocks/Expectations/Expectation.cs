using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Helpers;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Expectations
{
    /// <summary>
    /// Access to various options that can 
    /// be applied to an expectation
    /// </summary>
    public class Expectation : IMockExpectation, IExpectationOptions
    {
        private readonly IRepeatOptions repeatOptions;
        private readonly List<Actuals> actuals;

        private bool consider;
        private bool proceedIsForced;
        private bool throwsException;
        private Range expectedCount;

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
            get { return (actuals.Count); }
        }

        /// <summary>
        /// The number of times the expectation
        /// is expected to be called
        /// </summary>
        public Range ExpectedCount
        {
            get { return expectedCount; }
        }

        /// <summary>
        /// Indicates whether or not the
        /// expectation have been met
        /// </summary>
        public bool ExpectationMet
        {
            get
            {
                var actualCount = actuals.Count;
                var minimum = expectedCount.Minimum;
                var maximum = expectedCount.Maximum;

                if (minimum == int.MaxValue)
                    if (maximum.HasValue && maximum.Value == int.MaxValue)
                        return true;

                if (actualCount < minimum)
                    return false;

                if (!maximum.HasValue)
                    return true;

                return (actualCount <= maximum.Value);
            }
        }

        /// <summary>
        /// Indicates whether or not the
        /// expectation have been satisfied
        /// </summary>
        public bool ExpectationSatisfied
        {
            get
            {
                if (!consider)
                    return false;

                var actualCount = actuals.Count;
                var minimum = expectedCount.Minimum;
                var maximum = expectedCount.Maximum;
                if (actualCount < minimum)
                    return false;

                if (!maximum.HasValue)
                    return false;

                return (actualCount >= maximum.Value);
            }
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
        /// Indicates whether or not actual
        /// calls have been made against
        /// this expectation
        /// </summary>
        public bool HasActuals
        {
            get { return (actuals.Count != 0); }
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
        /// Return type for the expectation
        /// </summary>
        public virtual Type ReturnType
        {
            get { return typeof(void); }
        }

        /// <summary>
        /// Indicates whether or not the
        /// expectation should be considered
        /// </summary>
        /// <remarks>
        /// Utilized for when <see cref="RepeatOptions.Never()"/>
        /// is called to no longer consider the expectation.
        /// This is horrible
        /// </remarks>
        public bool ShouldConsider
        {
            get { return consider; }
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
        /// constructor
        /// </summary>
        public Expectation()
            : this(new Range(1, 1))
        {
            repeatOptions = new RepeatOptions(this);
        }

        /// <summary>
        /// protected constructor
        /// </summary>
        protected Expectation(Range expected)
        {
            actuals = new List<Actuals>();
            expectedCount = expected;

            consider = true;
        }

        /// <summary>
        /// Increments actual call counter
        /// </summary>
        /// <param name="item"></param>
        public void AddActualCall(Actuals item)
        {
            actuals.Add(item);
        }

        /// <summary>
        /// Indicates whether this expectation
        /// handled the actual method call
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool HandledActual(Actuals item)
        {
            return actuals.Any(x => x.HashCode == item.HashCode);
        }

        /// <summary>
        /// Handles the method
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
        /// Sets expected call counter
        /// </summary>
        /// <param name="expected"></param>
        public void SetExpectedCount(Range expected)
        {
            expectedCount = expected;

            var minimum = expected.Minimum;
            var maximum = expected.Maximum;

            if (minimum == 0)
            {
                if (maximum.HasValue && maximum.Value == 0)
                    consider = false;
            }
        }

        /// <summary>
        /// Sets expected return value
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetReturnValue(object value)
        {
        }

        /// <summary>
        /// Call original method
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions IExpectationOptions.CallOriginalMethod()
        {
            if (Method.IsAbstract)
            {
                var message = string.Format(
                    "Can't use CallOriginalMethod on method {0} because the method is abstract.",
                        Method.Name);

                throw new InvalidOperationException(message);
            }

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

        /// <summary>
        /// Throw exception of the given type when
        /// the method is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="exception"></param>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions IExpectationOptions.Throws<TException>(TException exception)
        {
            throwsException = true;

            ExceptionToThrow = exception;
            return this;
        }
    }

    /// <summary>
    /// Access to various options that can 
    /// be applied to an expectation
    /// </summary>
    public class Expectation<T> : Expectation, IExpectationOptions<T>
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
        /// Return type for the expectation
        /// </summary>
        public override Type ReturnType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public Expectation()
            : base(new Range(1, 1))
        {
            repeatOptions = new RepeatOptions<T>(this);
        }

        /// <summary>
        /// Sets expected return value
        /// </summary>
        /// <param name="value"></param>
        public override void SetReturnValue(object value)
        {
            returnValue = (T)value;
            returnValueSet = true;
        }

        /// <summary>
        /// Call original method
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> IExpectationOptions<T>.CallOriginalMethod()
        {
            if (Method.IsAbstract)
            {
                var message = string.Format(
                    "Can't use CallOriginalMethod on method {0} because the method is abstract.",
                        Method.Name);

                throw new InvalidOperationException(message);
            }

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
        IExpectationOptions<T> IExpectationOptions<T>.Return(T value)
        {
            SetReturnValue(value);
            return this;
        }

        /// <summary>
        /// Define the return value of a method call (non-void)
        /// </summary>
        /// <param name="func">Value to return when method is called</param>
        /// <returns>Fluid Interface</returns>
        IExpectationOptions<T> IExpectationOptions<T>.Returns(Func<T> func)
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
        IExpectationOptions<T> IExpectationOptions<T>.Throws<TException>()
        {
            throwsException = true;

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
        IExpectationOptions IExpectationOptions<T>.Throws<TException>(TException exception)
        {
            throwsException = true;

            ExceptionToThrow = exception;
            return this;
        }
    }
}
