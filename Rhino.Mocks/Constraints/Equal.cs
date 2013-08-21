using System.Collections;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Constraint comparing equality
    /// </summary>
    public class Equal : AbstractConstraint
    {
        private readonly object arg1;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get
            {
                return (arg1 == null)
                    ? "equal to null"
                    : string.Format("equal to {0}", arg1);
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="arg1"></param>
        public Equal(object arg1)
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
                return (arg1 == null);

            if (arg1 == null)
                return false;

            var expected = arg1 as IMockInstance;
            var actual = arg as IMockInstance;
            if (expected != null && actual != null)
                return MockInstanceEquality.Instance.AreEqual(expected, actual);

            if (arg1 is ICollection)
                return CollectionsAreEqual(arg1 as ICollection, arg as ICollection);

            return arg1.Equals(arg);
        }

        /// <summary>
        /// Determines if two collections are equal
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static bool CollectionsAreEqual(ICollection expected, ICollection actual)
        {
            if (expected == null && actual == null)
                return true;

            if (expected == null || actual == null)
                return false;

            if (expected.Count != actual.Count)
                return false;

            var expectedEnumerator = expected.GetEnumerator();
            var actualEnumerator = actual.GetEnumerator();
            while (expectedEnumerator.MoveNext() && actualEnumerator.MoveNext())
            {
                var expectedItem = expectedEnumerator.Current;
                var actualItem = actualEnumerator.Current;

                if (expectedItem == null)
                {
                    if (actualItem == null)
                        continue;

                    return false;
                }

                if (expectedItem is ICollection)
                {
                    if (CollectionsAreEqual(expectedItem as ICollection, actualItem as ICollection))
                        continue;

                    return false;
                }

                if (!expectedItem.Equals(actualItem))
                    return false;
            }

            return true;
        }
    }
}
