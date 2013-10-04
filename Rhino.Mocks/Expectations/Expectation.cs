using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Expectations
{
    /// <summary>
    /// Access to various options that can be applied to an expectation
    /// </summary>
    public abstract class Expectation
    {
        private readonly List<Actuals> actuals;
        private Range range;
        private bool consider;

        /// <summary>
        /// Identifies the type of expectation
        /// </summary>
        internal abstract ExpectationType Type { get; }
        
        /// <summary>
        /// The number of times the expectation was actually called
        /// </summary>
        public int ActualCount
        {
            get { return actuals.Count; }
        }

        /// <summary>
        /// The number of times the expectation is expected to be called
        /// </summary>
        public Range ExpectedCount
        {
            get { return range; }
        }
        
        /// <summary>
        /// Indicates whether or not the expectation have been met
        /// </summary>
        public bool ExpectationMet
        {
            get
            {
                var actualCount = actuals.Count;
                var minimum = range.Minimum;
                var maximum = range.Maximum;

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
        /// Indicates whether or not the expectation have been satisfied
        /// </summary>
        public bool ExpectationSatisfied
        {
            get
            {
                if (!consider)
                    return false;

                var actualCount = actuals.Count;
                var minimum = range.Minimum;
                var maximum = range.Maximum;
                if (actualCount < minimum)
                    return false;

                if (!maximum.HasValue)
                    return false;

                return (actualCount >= maximum.Value);
            }
        }
        
        /// <summary>
        /// Indicates whether or not actual calls have been made against
        /// this expectation
        /// </summary>
        public bool HasActuals
        {
            get { return (actuals.Count != 0); }
        }

        /// <summary>
        /// Indicates whether or not a delegate has been set against
        /// this expectation
        /// </summary>
        public bool HasDelegateToInvoke
        {
            get { return (DelegateToInvoke != null); }
        }

        /// <summary>
        /// Indicates whether or not the expectation has a return type
        /// </summary>
        public virtual bool HasReturnValue
        {
            get { return false; }
        }

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
        /// Delegate to invoke when expectation has been called
        /// </summary>
        public Delegate DelegateToInvoke { get; set; }

        /// <summary>
        /// Expectation to throw if method is called
        /// </summary>
        public Exception ExceptionToThrow { get; set; }

        /// <summary>
        /// Indicates whether or not the mocked method is executed
        /// </summary>
        public bool ForceProceed { get; set; }

        /// <summary>
        /// Collection of "out" and "ref" arguments
        /// </summary>
        public object[] ReturnArguments { get; set; }

        /// <summary>
        /// Indicates whether or not an
        /// exception should be thrown
        /// </summary>
        public bool ThrowsException { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="range"></param>
        protected Expectation(Range range)
        {
            this.range = range;

            consider = true;
            actuals = new List<Actuals>();
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
        /// Indicates whether this expectation handled the actual method call
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool HandledActual(Actuals item)
        {
            return actuals.Any(x => x.HashCode == item.HashCode);
        }

        /// <summary>
        /// Returns the string representation of the expectation
        /// </summary>
        /// <returns>string</returns>
        public abstract string GetDisplayName(IInvocation invocation);

        /// <summary>
        /// Handles the method
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        public abstract void HandleMethodCall(MethodInfo method, object[] arguments);
        
        /// <summary>
        /// Checks that the given method and arguments
        /// match the expectation and argument constraints
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public abstract bool MatchesCall(MethodInfo method, object[] arguments);

        /// <summary>
        /// Sets expected call counter
        /// </summary>
        /// <param name="expected"></param>
        public void SetExpectedCount(Range expected)
        {
            range = expected;

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
    }

    /// <summary>
    /// Access to various options that can be applied to an expectation
    /// </summary>
    public abstract class Expectation<T> : Expectation
    {
        private T returnValue;
        private bool returnValueIsSet;
        
        /// <summary>
        /// Indicates whether or not the expectation has a return type
        /// </summary>
        public override bool HasReturnValue
        {
            get { return returnValueIsSet; }
        }

        /// <summary>
        /// Return value for the expectation
        /// </summary>
        public override object ReturnValue
        {
            get { return returnValue; }
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
        /// <param name="range"></param>
        protected Expectation(Range range)
            : base(range)
        {
        }

        /// <summary>
        /// Sets expected return value
        /// </summary>
        /// <param name="value"></param>
        public override void SetReturnValue(object value)
        {
            returnValue = (T)value;
            returnValueIsSet = true;
        }
    }
}
