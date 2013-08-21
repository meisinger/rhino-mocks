
namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Logical OR operator over two constraints
    /// </summary>
    public class OperatorOr : AbstractConstraint
    {
        private readonly AbstractConstraint arg1;
        private readonly AbstractConstraint arg2;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return string.Format("{0} or {1}", arg1.Message, arg2.Message); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public OperatorOr(AbstractConstraint arg1, AbstractConstraint arg2)
        {
            this.arg1 = arg1;
            this.arg2 = arg2;
        }

        /// <summary>
        /// Determines if the give object passes
        /// the constraint
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override bool Eval(object arg)
        {
            return (arg1.Eval(arg) || arg2.Eval(arg));
        }
    }
}
