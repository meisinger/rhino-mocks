using System;
using System.Collections;
using Rhino.Mocks.Core.Constraints;

namespace Rhino.Mocks.Core.Helpers
{
    /// <summary>
    /// Provides access to constraints defined in <see cref="List"/>
    /// to be used in context with the <see cref="Arg&lt;T&gt;"/> syntax
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListArg<T>
    {
        /// <summary>
        /// constructor
        /// </summary>
        internal ListArg()
        {
        }

        /// <summary>
        /// Constraint that argument contains the
        /// give object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T IsIn(object item)
        {
            ArgumentManager.AddArgument(List.IsIn(item));
            return default(T);
        }

        /// <summary>
        /// Constraint that each item in the argument
        /// exists in the given collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public T ContainsAll(ICollection collection)
        {
            ArgumentManager.AddArgument(List.ContainsAll(collection));
            return default(T);
        }

        /// <summary>
        /// Constraint that argument is equal to one
        /// of the items in the given collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public T OneOf(ICollection collection)
        {
            ArgumentManager.AddArgument(List.OneOf(collection));
            return default(T);
        }

        /// <summary>
        /// Constraint that argument is equal to
        /// the given collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public T Equal(ICollection collection)
        {
            ArgumentManager.AddArgument(List.Equal(collection));
            return default(T);
        }

        /// <summary>
        /// Constraint that argument count passes
        /// the given constraint
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public T Count(AbstractConstraint constraint)
        {
            ArgumentManager.AddArgument(List.Count(constraint));
            return default(T);
        }

        /// <summary>
        /// Constraint that an item in the argument at
        /// the given index passes the given constraint
        /// </summary>
        /// <param name="index"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public T Element(int index, AbstractConstraint constraint)
        {
            ArgumentManager.AddArgument(List.Element(index, constraint));
            return default(T);
        }

        /// <summary>
        /// Constraint that an item in the argument
        /// matching to the given item passes the given 
        /// constraint
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="item"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public T Element<TItem>(TItem item, AbstractConstraint constraint)
        {
            ArgumentManager.AddArgument(List.Element<TItem>(item, constraint));
            return default(T);
        }

        /// <summary>
        /// Throws InvalidOperationException.
        /// "Equal" method should be used instead.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            throw new InvalidOperationException(
                "\"Equals\" is not a supported constraint. \"Equal\" should be used instead.");
        }

        /// <summary>
        /// Serves as a hash function for a particular type
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
