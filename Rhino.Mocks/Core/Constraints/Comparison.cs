using System;
using System.Text;

namespace Rhino.Mocks.Core.Constraints
{
    /// <summary>
    /// Constraint for IComparable objects
    /// </summary>
    public class Comparison : AbstractConstraint
    {
        private readonly IComparable arg1;
        private readonly bool largerThan;
        private readonly bool andEqualTo;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get 
            {
                var buffer = new StringBuilder();
                if (largerThan)
                    buffer.Append("greater than ");
                else
                    buffer.Append("less than ");

                if (andEqualTo)
                    buffer.Append("or equal to ");

                return buffer.Append(arg1)
                    .ToString();
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="largerThan"></param>
        /// <param name="andEqualTo"></param>
        public Comparison(IComparable arg1, bool largerThan, bool andEqualTo)
        {
            this.arg1 = arg1;
            this.largerThan = largerThan;
            this.andEqualTo = andEqualTo;
        }

        /// <summary>
        /// Determines if the give object passes
        /// the constraint
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override bool Eval(object arg)
        {
            var actual = arg as IComparable;
            if (actual == null)
                return false;

            var result = actual.CompareTo(arg1);
            if (result == 0 && andEqualTo)
                return true;

            return (largerThan)
                ? (result > 0)
                : (result < 0);
        }
    }
}
