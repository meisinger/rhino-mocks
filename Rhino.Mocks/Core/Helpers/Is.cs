using System;
using Rhino.Mocks.Core.Constraints;

namespace Rhino.Mocks.Core.Helpers
{
    /// <summary>
    /// Provides access to common constraints
    /// </summary>
    public static class Is
    {
        /// <summary>
        /// Constraint that argument is greater than the
        /// given object
        /// </summary>
        /// <param name="objToCompare"></param>
        /// <returns></returns>
        public static AbstractConstraint GreaterThan(IComparable objToCompare)
        {
            return new Comparison(objToCompare, true, false);
        }

        /// <summary>
        /// Constraint that argument is greater than or 
        /// equal to the given object
        /// </summary>
        /// <param name="objToCompare"></param>
        /// <returns></returns>
        public static AbstractConstraint GreaterThanOrEqual(IComparable objToCompare)
        {
            return new Comparison(objToCompare, true, true);
        }

        /// <summary>
        /// Constraint that argument is less than the
        /// given object
        /// </summary>
        /// <param name="objToCompare"></param>
        /// <returns></returns>
        public static AbstractConstraint LessThan(IComparable objToCompare)
        {
            return new Comparison(objToCompare, false, false);
        }

        /// <summary>
        /// Constraint that argument is less than or
        /// equal to the given object
        /// </summary>
        /// <param name="objToCompare"></param>
        /// <returns></returns>
        public static AbstractConstraint LessThanOrEqual(IComparable objToCompare)
        {
            return new Comparison(objToCompare, false, true);
        }

        /// <summary>
        /// Constraint that argument is equal to 
        /// the given object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static AbstractConstraint Equal(object obj)
        {
            return new Equal(obj);
        }

        /// <summary>
        /// Constraint that argument is not equal to 
        /// the given object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static AbstractConstraint NotEqual(object obj)
        {
            return !(new Equal(obj));
        }

        /// <summary>
        /// Constraint that argument is the same instance
        /// as the given object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static AbstractConstraint Same(object obj)
        {
            return new Same(obj);
        }

        /// <summary>
        /// Constraint that argument is not the same
        /// instance as the given object 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static AbstractConstraint NotSame(object obj)
        {
            return !(new Same(obj));
        }

        /// <summary>
        /// Constraint allowing argument to be any value
        /// </summary>
        /// <returns></returns>
        public static AbstractConstraint Anything()
        {
            return new Anything();
        }

        /// <summary>
        /// Constraint that argument is null
        /// </summary>
        /// <returns></returns>
        public static AbstractConstraint Null()
        {
            return new Equal(null);
        }

        /// <summary>
        /// Constraint that argument is not null
        /// </summary>
        /// <returns></returns>
        public static AbstractConstraint NotNull()
        {
            return !(new Equal(null));
        }

        /// <summary>
        /// Constraint that argument is an instance
        /// of the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AbstractConstraint TypeOf(Type type)
        {
            return new TypeOf(type);
        }

        /// <summary>
        /// Constraint that argument is an instance
        /// of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AbstractConstraint TypeOf<T>()
        {
            return new TypeOf(typeof(T));
        }

        /// <summary>
        /// Constraint that delegate returns true
        /// given the argument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static AbstractConstraint Matching<T>(Func<T, bool> predicate)
        {
            return new PredicateConstraint<T>(predicate);
        }
    }
}
