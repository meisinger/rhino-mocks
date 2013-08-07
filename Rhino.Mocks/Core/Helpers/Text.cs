using Rhino.Mocks.Core.Constraints;

namespace Rhino.Mocks.Core.Helpers
{
    /// <summary>
    /// Provides access to common string constraints
    /// </summary>
    public static class Text
    {
        /// <summary>
        /// Constraint that argument starts with the
        /// given value (ordinal)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static AbstractConstraint StartsWith(string value)
        {
            return new StartsWith(value);
        }

        /// <summary>
        /// Constraint that argument ends with the
        /// given value (ordinal)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static AbstractConstraint EndsWith(string value)
        {
            return new EndsWith(value);
        }

        /// <summary>
        /// Constraint that argument contains the
        /// given value (ordinal)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static AbstractConstraint Contains(string value)
        {
            return new Contains(value);
        }

        /// <summary>
        /// Constraint that argument matches the
        /// given regular expression
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static AbstractConstraint Like(string pattern)
        {
            return new Like(pattern);
        }
    }
}
