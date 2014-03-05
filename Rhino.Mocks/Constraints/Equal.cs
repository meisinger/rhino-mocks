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
            return CollectionsAreEqual(new[] { arg1 }, new[] { arg });
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

                if (SafeEquals(expectedItem, actualItem))
                    continue;

                if (expectedItem is ICollection)
                {
                    if (!CollectionsAreEqual(expectedItem as ICollection, actualItem as ICollection))
                        return false;

                    continue;
                }

                return false;
            }

            return true;
        }

        private static bool SafeEquals(object expected, object actual)
        {
            var expcetedMock = expected as IMockInstance;
            var actualMock = actual as IMockInstance;

            if (expcetedMock == null && actualMock == null)
                return expected.Equals(actual);

            return MockInstanceEquality.Instance.AreEqual(expected, actual);
        }
    }
}
