
namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Abstract class for constraint evaluation
    /// </summary>
    public abstract class AbstractConstraint
    {
        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public abstract string Message { get; }

        /// <summary>
        /// Determines if the give object passes
        /// the constraint
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public abstract bool Eval(object arg);

        /// <summary>
        /// Logical AND operator
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static AbstractConstraint operator &(AbstractConstraint arg1, AbstractConstraint arg2)
        {
            return new OperatorAnd(arg1, arg2);
        }

        /// <summary>
        /// Logical OR operator
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static AbstractConstraint operator |(AbstractConstraint arg1, AbstractConstraint arg2)
        {
            return new OperatorOr(arg1, arg2);
        }

        /// <summary>
        /// Logical negation operator
        /// </summary>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static AbstractConstraint operator !(AbstractConstraint arg1)
        {
            return new OperatorNot(arg1);
        }

        /// <summary>
        /// Definitely false operator
        /// </summary>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static bool operator false(AbstractConstraint arg1)
        {
            return false;
        }

        /// <summary>
        /// Definitely true operator
        /// </summary>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static bool operator true(AbstractConstraint arg1)
        {
            return false;
        }
    }
}
