
namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Logical negate operator on constraint
    /// </summary>
    public class OperatorNot : AbstractConstraint
    {
        private readonly AbstractConstraint arg1;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return string.Format("not {0}", arg1.Message); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="arg1"></param>
        public OperatorNot(AbstractConstraint arg1)
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
            return !(arg1.Eval(arg));
        }
    }
}
