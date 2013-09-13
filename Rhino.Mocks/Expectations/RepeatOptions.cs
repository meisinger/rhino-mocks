using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Expectations
{
    /// <summary>
    /// Access to repeatable options on expectation
    /// </summary>
    public class RepeatOptions : IRepeatOptions
    {
        private readonly ExpectMethod expectation;

        /// <summary>
        /// constructor
        /// </summary>
        public RepeatOptions(ExpectMethod expectation)
        {
            this.expectation = expectation;
        }

        /// <summary>
        /// Expectation will be called once
        /// </summary>
        /// <returns></returns>
        public IMethodOptions Once()
        {
            expectation.SetExpectedCount(new Range(1, 1));
            return expectation;
        }

        /// <summary>
        /// Expectation will be called twice
        /// </summary>
        /// <returns></returns>
        public IMethodOptions Twice()
        {
            expectation.SetExpectedCount(new Range(2, 2));
            return expectation;
        }

        /// <summary>
        /// Expectation will be called exactly
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IMethodOptions Times(int expected)
        {
            expectation.SetExpectedCount(new Range(expected, expected));
            return expectation;
        }

        /// <summary>
        /// Expectation will be called at least
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IMethodOptions AtLeast(int expected)
        {
            expectation.SetExpectedCount(new Range(expected, null));
            return expectation;
        }

        /// <summary>
        /// Expectation could be called as many
        /// times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IMethodOptions AtMost(int expected)
        {
            expectation.SetExpectedCount(new Range(1, expected));
            return expectation;
        }

        /// <summary>
        /// Expectation may be called
        /// </summary>
        /// <returns></returns>
        public IMethodOptions Any()
        {
            expectation.SetExpectedCount(new Range(int.MaxValue, int.MaxValue));
            return expectation;
        }

        /// <summary>
        /// Expectation will not be called
        /// </summary>
        /// <returns></returns>
        public IMethodOptions Never()
        {
            expectation.SetExpectedCount(new Range(0, 0));
            return expectation;
        }
    }

    /// <summary>
    /// Access to repeatable options on expectation
    /// </summary>
    public class RepeatOptions<T> : IRepeatOptions<T>
    {
        private readonly ExpectMethod<T> expectation;

        /// <summary>
        /// constructor
        /// </summary>
        public RepeatOptions(ExpectMethod<T> expectation)
        {
            this.expectation = expectation;
        }

        /// <summary>
        /// Expectation will be called once
        /// </summary>
        /// <returns></returns>
        public IMethodOptions<T> Once()
        {
            expectation.SetExpectedCount(new Range(1, 1));
            return expectation;
        }

        /// <summary>
        /// Expectation will be called twice
        /// </summary>
        /// <returns></returns>
        public IMethodOptions<T> Twice()
        {
            expectation.SetExpectedCount(new Range(2, 2));
            return expectation;
        }

        /// <summary>
        /// Expectation will be called exactly
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IMethodOptions<T> Times(int expected)
        {
            expectation.SetExpectedCount(new Range(expected, expected));
            return expectation;
        }

        /// <summary>
        /// Expectation will be called at least
        /// as many times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IMethodOptions<T> AtLeast(int expected)
        {
            expectation.SetExpectedCount(new Range(expected, null));
            return expectation;
        }

        /// <summary>
        /// Expectation could be called as many
        /// times as the given number
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public IMethodOptions<T> AtMost(int expected)
        {
            expectation.SetExpectedCount(new Range(1, expected));
            return expectation;
        }

        /// <summary>
        /// Expectation may be called
        /// </summary>
        /// <returns></returns>
        public IMethodOptions<T> Any()
        {
            expectation.SetExpectedCount(new Range(int.MaxValue, int.MaxValue));
            return expectation;
        }

        /// <summary>
        /// Expectation will not be called
        /// </summary>
        /// <returns></returns>
        public IMethodOptions<T> Never()
        {
            expectation.SetExpectedCount(new Range(0, 0));
            return expectation;
        }
    }
}
