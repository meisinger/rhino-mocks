using System;

namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Constraint to determine if given string
    /// can be found at the end of the object
    /// </summary>
    public class EndsWith : AbstractConstraint
    {
        private readonly string arg1;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return string.Format("ends with \"{0}\"", arg1); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="arg1"></param>
        public EndsWith(string arg1)
        {
            if (string.IsNullOrEmpty(arg1))
                throw new ArgumentNullException("arg1");

            this.arg1 = arg1;
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

            var actual = arg.ToString();
            return actual.EndsWith(arg1, StringComparison.Ordinal);
        }
    }
}
