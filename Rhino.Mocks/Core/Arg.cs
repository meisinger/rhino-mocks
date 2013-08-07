using System;
using System.Linq.Expressions;
using Rhino.Mocks.Core.Constraints;
using Rhino.Mocks.Core.Helpers;

namespace Rhino.Mocks.Core
{
    /// <summary>
    /// Provides access to create constraints
    /// against arguments of a mock
    /// </summary>
    public static class Arg
    {
        /// <summary>
        /// Access to constraints against
        /// string arguments
        /// </summary>
        public static TextArg Text
        {
            get { return new TextArg(); }
        }

        /// <summary>
        /// Constraint that argument is equal
        /// to the given source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Is<T>(T source)
        {
            return Arg<T>.Is.Equal(source);
        }
    }

    /// <summary>
    /// Provides access to create constraints
    /// against arguments of a mock
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Arg<T>
    {
        /// <summary>
        /// Provides access to create simple
        /// constraints against arguments of a mock
        /// </summary>
        public static IsArg<T> Is
        {
            get { return new IsArg<T>(); }
        }

        /// <summary>
        /// Provides access to create 
        /// constraints against <see cref="System.Collections.ICollection"/>
        /// arguments of a mock
        /// </summary>
        public static ListArg<T> List
        {
            get { return new ListArg<T>(); }
        }

        /// <summary>
        /// Provides ability to create complex
        /// constraints against arguments of a mock
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static T Matches(AbstractConstraint constraint)
        {
            ArgumentManager.AddArgument(constraint);
            return default(T);
        }

        /// <summary>
        /// Provides ability to create a constraint
        /// using an <see cref="System.Linq.Expressions.LambdaExpression"/>
        /// for evaluation
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T Matches(Expression<Func<T, bool>> expression)
        {
            ArgumentManager.AddArgument(new LambdaConstraint(expression));
            return default(T);
        }

        /// <summary>
        /// Add constraint for an "out" argument
        /// of a mock
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ByRefDummy<T> Out(T value)
        {
            ArgumentManager.AddOutArgument(value);
            return new ByRefDummy<T>();
        }

        /// <summary>
        /// Add constraint for a "ref" argument
        /// of a mock
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ByRefDummy<T> Ref(AbstractConstraint constraint, T value)
        {
            ArgumentManager.AddRefArgument(constraint, value);
            return new ByRefDummy<T>();
        }
    }
}
