using System;
using System.Collections;
using System.Text;

namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Constraint to determine if an object
    /// exists in a collection
    /// </summary>
    public class OneOf : AbstractConstraint
    {
        private readonly ICollection arg1;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get
            {
                var buffer = new StringBuilder()
                    .Append("one of [");

                var argEnumerator = arg1.GetEnumerator();
                while (argEnumerator.MoveNext())
                {
                    var current = argEnumerator.Current;
                    buffer.Append(current)
                        .Append(", ");
                }

                return buffer
                    .Remove(buffer.Length - 2, 2)
                    .Append("]")
                    .ToString();
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="arg1"></param>
        public OneOf(ICollection arg1)
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
            var expectedEnumerator = arg1.GetEnumerator();
            while (expectedEnumerator.MoveNext())
            {
                var expectedItem = expectedEnumerator.Current;
                if (arg.Equals(expectedItem))
                    return true;
            }

            return false;
        }
    }
}
