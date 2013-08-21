using System;

namespace Rhino.Mocks.Helpers
{
    /// <summary>
    /// Provides access to constraints defined in <see cref="Is"/>
    /// to be used in context with the <see cref="Arg&lt;T&gt;"/> syntax
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IsArg<T>
    {
        /// <summary>
        /// Constraint allowing argument to be
        /// any value
        /// </summary>
        public T Anything
        {
            get
            {
                ArgumentManager.AddArgument(Is.Anything());
                return default(T);
            }
        }

        /// <summary>
        /// Constraint that argument is null
        /// </summary>
        public T Null
        {
            get
            {
                ArgumentManager.AddArgument(Is.Null());
                return default(T);
            }
        }

        /// <summary>
        /// Constraint that argument is not null
        /// </summary>
        public T NotNull
        {
            get
            {
                ArgumentManager.AddArgument(Is.NotNull());
                return default(T);
            }
        }

        /// <summary>
        /// Constraint that argument is an instance 
        /// of the specified type
        /// </summary>
        public T TypeOf
        {
            get
            {
                ArgumentManager.AddArgument(Is.TypeOf<T>());
                return default(T);
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        internal IsArg()
        {
        }

        /// <summary>
        /// Constraint that argument is greater than
        /// the given source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public T GreaterThan(IComparable source)
        {
            source = ConvertObjectType(source);

            ArgumentManager.AddArgument(Is.GreaterThan(source));
            return default(T);
        }

        /// <summary>
        /// Constraint that argument is greater than
        /// or equal to the given source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public T GreaterThanOrEqual(IComparable source)
        {
            source = ConvertObjectType(source);

            ArgumentManager.AddArgument(Is.GreaterThanOrEqual(source));
            return default(T);
        }

        /// <summary>
        /// Constraint that argument is less than
        /// the given source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public T LessThan(IComparable source)
        {
            source = ConvertObjectType(source);

            ArgumentManager.AddArgument(Is.LessThan(source));
            return default(T);
        }

        /// <summary>
        /// Constraint that argument is less than
        /// or equal to the given source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public T LessThanOrEqual(IComparable source)
        {
            source = ConvertObjectType(source);

            ArgumentManager.AddArgument(Is.LessThanOrEqual(source));
            return default(T);
        }

        /// <summary>
        /// Constraint that argument is equal
        /// to the given source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public T Equal(object source)
        {
            source = ConvertObjectType(source);

            ArgumentManager.AddArgument(Is.Equal(source));
            return default(T);
        }

        /// <summary>
        /// Constraint that argument is not equal
        /// to the given source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public T NotEqual(object source)
        {
            source = ConvertObjectType(source);

            ArgumentManager.AddArgument(Is.NotEqual(source));
            return default(T);
        }

        /// <summary>
        /// Constraint that argument is the 
        /// same instance of the given source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public T Same(object source)
        {
            source = ConvertObjectType(source);

            ArgumentManager.AddArgument(Is.Same(source));
            return default(T);
        }

        /// <summary>
        /// Constraint that argument is not the 
        /// same instance of the given source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public T NotSame(object source)
        {
            source = ConvertObjectType(source);

            ArgumentManager.AddArgument(Is.NotSame(source));
            return default(T);
        }

        /// <summary>
        /// Throws InvalidOperationException.
        /// "Equal" method should be used instead.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            throw new InvalidOperationException(
                "\"Equals\" is not a supported constraint. \"Equal\" should be used instead.");
        }

        /// <summary>
        /// Serves as a hash function for a particular type
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private IComparable ConvertObjectType(IComparable obj)
        {
            var actual = ConvertObjectType(obj as object);
            if (actual is IComparable)
                obj = actual as IComparable;

            return obj;
        }
        
        private object ConvertObjectType(object obj)
        {
            var type = typeof(T);
            if (!type.IsPrimitive || (type == obj.GetType()))
                return obj;

            try
            {
                return Convert.ChangeType(obj, type);
            }
            catch (InvalidCastException)
            {
                return obj;
            }
            catch (FormatException)
            {
                return obj;
            }
        }
    }
}
