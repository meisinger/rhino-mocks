using System;

namespace Rhino.Mocks.Core.Constraints
{
    /// <summary>
    /// Constraint to determine if given string
    /// can be found in the object
    /// </summary>
    public class Contains : AbstractConstraint
    {
        private readonly string arg1;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return string.Format("contains \"{0}\"", arg1); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="arg1"></param>
        public Contains(string arg1)
        {
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
            var index = actual.IndexOf(arg1, StringComparison.Ordinal);
            return (index != -1);
        }
    }
}
