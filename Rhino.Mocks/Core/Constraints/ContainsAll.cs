using System;
using System.Collections;
using System.Text;

namespace Rhino.Mocks.Core.Constraints
{
    /// <summary>
    /// Constraint comparing items in one collection
    /// exist entirely in another collection
    /// </summary>
    public class ContainsAll : AbstractConstraint
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
                    .Append("list missing [");

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
        public ContainsAll(ICollection arg1)
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

            var expectedEnumerator = arg1.GetEnumerator();
            var actualEnumerator = actual.GetEnumerator();
            while (expectedEnumerator.MoveNext())
            {
                var expectedItem = expectedEnumerator.Current;
                var expectedFound = false;

                actualEnumerator.Reset();
                while (actualEnumerator.MoveNext())
                {
                    var actualItem = actualEnumerator.Current;
                    if (expectedItem.Equals(actualItem))
                    {
                        expectedFound = true;
                        break;
                    }
                }

                if (!expectedFound)
                    return false;
            }

            return true;
        }
    }
}
