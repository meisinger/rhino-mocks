using System.Linq.Expressions;

namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Constraint allowing a <see cref="System.Linq.Expressions.LambdaExpression"/>
    /// to be used against objects
    /// </summary>
    public class LambdaConstraint : AbstractConstraint
    {
        private readonly LambdaExpression expression;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return expression.ToString(); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="expression"></param>
        public LambdaConstraint(LambdaExpression expression)
        {
            this.expression = expression;
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
                return false;

            var actualType = arg.GetType();
            if (!expression.Parameters[0].Type.IsAssignableFrom(actualType))
                return false;

            var func = expression.Compile();
            return (bool)func.DynamicInvoke(arg);
        }
    }
}
