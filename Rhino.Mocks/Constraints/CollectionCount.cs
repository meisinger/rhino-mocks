using System;
using System.Collections;

namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Constraint allowing the count of a collection
    /// to be applied to another constraint
    /// </summary>
    public class CollectionCount : AbstractConstraint
    {
        private readonly AbstractConstraint arg1;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return string.Format("collection count {0}", arg1.Message); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="arg1"></param>
        public CollectionCount(AbstractConstraint arg1)
        {
            if (arg1 == null)
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
            var actual = arg as ICollection;
            if (actual == null)
                return false;

            return arg1.Eval(actual.Count);
        }
    }
}
