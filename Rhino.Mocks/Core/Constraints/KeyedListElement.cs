using System.Reflection;

namespace Rhino.Mocks.Core.Constraints
{
    /// <summary>
    /// Constraint allowing a specific item in
    /// a generic collection to be applied to 
    /// another constraint
    /// </summary>
    public class KeyedListElement<T> : AbstractConstraint
    {
        private const BindingFlags GetFlags = 
            BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.DeclaredOnly | BindingFlags.Instance;

        private readonly T key;
        private readonly AbstractConstraint arg1;

        /// <summary>
        /// Returns the message of the constraint
        /// </summary>
        public override string Message
        {
            get { return string.Format("element at key {0} {1}", key, arg1.Message); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="arg1"></param>
        public KeyedListElement(T key, AbstractConstraint arg1)
        {
            this.key = key;
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
            var method = FindGetItemMethod(arg);
            if (method == null)
                return false;

            object actual;

            try
            {
                actual = method.Invoke(arg, new object[] { key });
            }
            catch (TargetInvocationException)
            {
                return false;
            }

            return arg1.Eval(actual);
        }

        private MethodInfo FindGetItemMethod(object arg)
        {
            if (arg == null)
                return null;

            var types = new[] { typeof(T) };
            var argType = arg.GetType();

            return argType.GetMethod("get_Item", GetFlags, null, types, null);
        }
    }
}
