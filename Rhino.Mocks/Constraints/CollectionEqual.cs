using System;
using System.Collections;
using System.Text;

namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Constraint comparing items in a collection
    /// </summary>
    public class CollectionEqual : AbstractConstraint
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
                    .Append("equal to collection [");

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
        public CollectionEqual(ICollection arg1)
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
            return Equal.CollectionsAreEqual(arg1, arg as ICollection);
        }
    }
}
