using System;

namespace Rhino.Mocks.Core.Constraints
{
    /// <summary>
    /// Constraint to determine if the given
    /// object is an instance of a specific type
    /// </summary>
    public class TypeOf : AbstractConstraint
    {
        private readonly Type type;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return string.Format("type of {{{0}}}", type.FullName); }
        }
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="type"></param>
        public TypeOf(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Determines if the give object passes
        /// the constraint
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override bool Eval(object arg)
        {
            return type.IsInstanceOfType(arg);
        }
    }
}
