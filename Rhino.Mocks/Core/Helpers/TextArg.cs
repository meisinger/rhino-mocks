using System;

namespace Rhino.Mocks.Core.Helpers
{
    /// <summary>
    /// Provides access to constraints defined in <see cref="List"/>
    /// to be used in context with the <see cref="Arg"/> syntax
    /// </summary>
    public class TextArg
    {
        /// <summary>
        /// constraint
        /// </summary>
        internal TextArg()
        {
        }

        /// <summary>
        /// Constraint that argument starts with the
        /// given value (ordinal)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string StartsWith(string value)
        {
            ArgumentManager.AddArgument(Text.StartsWith(value));
            return null;
        }

        /// <summary>
        /// Constraint that argument ends with the
        /// given value (ordinal)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string EndsWith(string value)
        {
            ArgumentManager.AddArgument(Text.EndsWith(value));
            return null;
        }

        /// <summary>
        /// Constraint that argument contains the
        /// given value (ordinal)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Contains(string value)
        {
            ArgumentManager.AddArgument(Text.Contains(value));
            return null;
        }

        /// <summary>
        /// Constraint that argument matches the
        /// given regular expression
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string Like(string pattern)
        {
            ArgumentManager.AddArgument(Text.Like(pattern));
            return null;
        }

        /// <summary>
        /// Throws InvalidOperationException.
        /// "Equal" method should be used instead.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            throw new InvalidOperationException(
                "\"Equals\" is not a supported constraint. \"Equal\" should be used instead.");
        }

        /// <summary>
        /// Serves as a hash function for a particular type
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
