using System.Collections;

namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Constraint on the given object is
    /// found in a collection
    /// </summary>
    public class IsIn : AbstractConstraint
    {
        private readonly object arg1;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return string.Format("list contains [{0}]", arg1); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="arg1"></param>
        public IsIn(object arg1)
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
            var actual = arg as IEnumerable;
            if (actual == null)
                return false;

            var actualEnumerator = actual.GetEnumerator();
            while (actualEnumerator.MoveNext())
            {
                var actualItem = actualEnumerator.Current;
                if (arg1.Equals(actualItem))
                    return true;
            }

            return false;
        }
    }
}
