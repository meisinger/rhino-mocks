
namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Constraint allowing all values (always
    /// returns true)
    /// </summary>
    public class Anything : AbstractConstraint
    {
        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return "anything"; }
        }

        /// <summary>
        /// Determines if the give object passes
        /// the constraint
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override bool Eval(object arg)
        {
            return true;
        }
    }
}
