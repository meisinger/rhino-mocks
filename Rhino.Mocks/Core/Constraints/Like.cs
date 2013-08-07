using System;
using System.Text.RegularExpressions;

namespace Rhino.Mocks.Core.Constraints
{
    /// <summary>
    /// Constraint to determine if object 
    /// matches the given regular expression
    /// </summary>
    public class Like : AbstractConstraint
    {
        private readonly string pattern;
        private readonly Regex expression;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return string.Format("like \"{0}\"", pattern); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pattern"></param>
        public Like(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException("pattern");

            this.pattern = pattern;

            expression = new Regex(pattern);
        }

        /// <summary>
        /// Determines if the give object passes
        /// the constraint
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override bool Eval(object arg)
        {
            if (arg == null)
                return false;

            return expression.IsMatch(arg.ToString());
        }
    }
}
