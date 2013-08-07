using System;
using Rhino.Mocks.Core.Helpers;

namespace Rhino.Mocks.Core.Constraints
{
    /// <summary>
    /// Constraint allowing comparison through
    /// a delegate
    /// </summary>
    public class PredicateConstraint<T> : AbstractConstraint
    {
        private readonly Func<T, bool> predicate;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format("predicate ({0}})",
                    MethodFormatter.ToString(null, predicate.Method, (arr, i) => "obj"));
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="predicate"></param>
        public PredicateConstraint(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            this.predicate = predicate;
        }

        /// <summary>
        /// Determines if the give object passes
        /// the constraint
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override bool Eval(object arg)
        {
            if (arg != null && !typeof(T).IsAssignableFrom(arg.GetType()))
                return false;

            return predicate((T)arg);
        }
    }
}
