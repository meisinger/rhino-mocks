using Rhino.Mocks.Core.Interfaces;

namespace Rhino.Mocks.Core.Expectations
{
    /// <summary>
    /// Access to repeatable options
    /// on expectation
    /// </summary>
    public class RepeatOptions : IRepeatOptions
    {
        private readonly ExpectationOptions expectation;

        /// <summary>
        /// constructor
        /// </summary>
        public RepeatOptions(ExpectationOptions expectation)
        {
            this.expectation = expectation;
        }

        /// <summary>
        /// Expectation will be called once
        /// </summary>
        /// <returns></returns>
        public IExpectationOptions Once()
        {
            expectation.SetExpectedCount(1);
            return expectation;
        }

        /// <summary>
        /// Expectation will be called twice
        /// </summary>
        /// <returns></returns>
        public IExpectationOptions Twice()
        {
            expectation.SetExpectedCount(2);
            return expectation;
        }

        /// <summary>
        /// Expectation will be called exactly
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IExpectationOptions Times(int expected)
        {
            expectation.SetExpectedCount(expected);
            return expectation;
        }

        /// <summary>
        /// Expectation will be called at least
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IExpectationOptions AtLeast(int expected)
        {
            expectation.SetExpectedCount(expected);
            return expectation;
        }

        /// <summary>
        /// Expectation could be called as many
        /// times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IExpectationOptions AtMost(int expected)
        {
            expectation.SetExpectedCount(expected);
            return expectation;
        }

        /// <summary>
        /// Expectation may be called
        /// </summary>
        /// <returns></returns>
        public IExpectationOptions Any()
        {
            expectation.SetExpectedCount(int.MaxValue);
            return expectation;
        }

        /// <summary>
        /// Expectation will not be called
        /// </summary>
        /// <returns></returns>
        public IExpectationOptions Never()
        {
            expectation.SetExpectedCount(0);
            return expectation;
        }
    }

    /// <summary>
    /// Access to repeatable options
    /// on expectation
    /// </summary>
    public class RepeatOptions<T> : IRepeatOptions<T>
    {
        private readonly ExpectationOptions<T> expectation;

        /// <summary>
        /// constructor
        /// </summary>
        public RepeatOptions(ExpectationOptions<T> expectation)
        {
            this.expectation = expectation;
        }

        /// <summary>
        /// Expectation will be called once
        /// </summary>
        /// <returns></returns>
        public IExpectationOptions<T> Once()
        {
            expectation.SetExpectedCount(1);
            return expectation;
        }

        /// <summary>
        /// Expectation will be called twice
        /// </summary>
        /// <returns></returns>
        public IExpectationOptions<T> Twice()
        {
            expectation.SetExpectedCount(2);
            return expectation;
        }

        /// <summary>
        /// Expectation will be called exactly
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IExpectationOptions<T> Times(int expected)
        {
            expectation.SetExpectedCount(expected);
            return expectation;
        }

        /// <summary>
        /// Expectation will be called at least
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IExpectationOptions<T> AtLeast(int expected)
        {
            expectation.SetExpectedCount(expected);
            return expectation;
        }

        /// <summary>
        /// Expectation could be called as many
        /// times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IExpectationOptions<T> AtMost(int expected)
        {
            expectation.SetExpectedCount(expected);
            return expectation;
        }

        /// <summary>
        /// Expectation may be called
        /// </summary>
        /// <returns></returns>
        public IExpectationOptions<T> Any()
        {
            expectation.SetExpectedCount(int.MaxValue);
            return expectation;
        }

        /// <summary>
        /// Expectation will not be called
        /// </summary>
        /// <returns></returns>
        public IExpectationOptions<T> Never()
        {
            expectation.SetExpectedCount(0);
            return expectation;
        }
    }
}
