using System.Collections;

namespace Rhino.Mocks.Core.Constraints
{
    /// <summary>
    /// Constraint allowing a specific item in 
    /// a collection to be applied to another
    /// constraint
    /// </summary>
    public class ListElement : AbstractConstraint
    {
        private readonly int index;
        private readonly AbstractConstraint arg1;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return string.Format("element at index {0} {1}", index, arg1.Message); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="index"></param>
        /// <param name="arg1"></param>
        public ListElement(int index, AbstractConstraint arg1)
        {
            this.index = index;
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
            var actual = arg as IList;
            if (actual == null)
                return false;

            if (index >= 0 && index < actual.Count)
            {
                var item = actual[index];
                return arg1.Eval(item);
            }

            return false;
        }
    }
}
