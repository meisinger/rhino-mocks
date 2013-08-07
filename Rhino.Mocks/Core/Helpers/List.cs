using System.Collections;
using Rhino.Mocks.Core.Constraints;

namespace Rhino.Mocks.Core.Helpers
{
    /// <summary>
    /// Provides access to common <see cref="System.Collections.ICollection"/> constraints
    /// </summary>
    public static class List
    {
        /// <summary>
        /// Constraint that argument contains the given
        /// object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static AbstractConstraint IsIn(object item)
        {
            return new IsIn(item);
        }

        /// <summary>
        /// Constraint that argument contains all of the
        /// items in the given collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static AbstractConstraint ContainsAll(ICollection collection)
        {
            return new ContainsAll(collection);
        }

        /// <summary>
        /// Constraint that argument is equal to one of 
        /// the items in the given collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static AbstractConstraint OneOf(ICollection collection)
        {
            return new OneOf(collection);
        }

        /// <summary>
        /// Constraint that argument is equal to the given
        /// collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static AbstractConstraint Equal(ICollection collection)
        {
            return new CollectionEqual(collection);
        }

        /// <summary>
        /// Constraint that arguments count passes the
        /// given constraint
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static AbstractConstraint Count(AbstractConstraint constraint)
        {
            return new CollectionCount(constraint);
        }

        /// <summary>
        /// Constraint that the item at the given index
        /// of the argument passes the given constraint
        /// </summary>
        /// <param name="index"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static AbstractConstraint Element(int index, AbstractConstraint constraint)
        {
            return new ListElement(index, constraint);
        }

        /// <summary>
        /// Constraint that the item of the given key
        /// of the argument passes the given constraint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static AbstractConstraint Element<T>(T key, AbstractConstraint constraint)
        {
            return new KeyedListElement<T>(key, constraint);
        }
    }
}
