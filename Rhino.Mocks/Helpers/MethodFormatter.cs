using System;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Helpers
{
    /// <summary>
    /// Helper class to produce a string representation
    /// of a method
    /// </summary>
    public static class MethodFormatter
    {
        /// <summary>
        /// Returns a string representation of a method
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string ToString(IInvocation invocation, MethodInfo method)
        {
            return ToString(invocation, method, new object[0], FormatArgument);
        }

        /// <summary>
        /// Returns a string representation of a method providing a 
        /// callback to format constraints
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="method"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string ToString(IInvocation invocation, MethodInfo method, Func<Array, int, string> func)
        {
            return ToString(invocation, method, new object[0], func);
        }

        /// <summary>
        /// Returns a string representation of a method and it's
        /// arguments
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string ToString(IInvocation invocation, MethodInfo method, object[] args)
        {
            return ToString(invocation, method, args, FormatArgument);
        }

        /// <summary>
        /// Returns a string representation of a method and it's
        /// arguments providing a callback to format constraints
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string ToString(IInvocation invocation, MethodInfo method, object[] args, Func<Array, int, string> func)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (args == null)
                throw new ArgumentNullException("args");

            if (func == null)
                throw new ArgumentNullException("func");

            var buffer = new StringBuilder()
                .AppendFormat("{0}.{1}", method.DeclaringType.Name, method.Name);

            if (invocation != null && method.IsGenericMethod)
            {
                buffer.Append("<");

                var generics = invocation.GenericArguments;
                for (int index = 0; index < generics.Length; index++)
                {
                    buffer.Append(generics[index])
                        .Append(", ");
                }

                buffer
                    .Remove(buffer.Length - 2, 2)
                    .Append(">");
            }

            buffer.Append("(");

            var count = method.GetParameters().Length;
            if (count != 0)
            {
                for (int index = 0; index < count; index++)
                {
                    buffer.Append(func(args, index))
                        .Append(", ");
                }

                buffer.Remove(buffer.Length - 2, 2);
            }

            return buffer
                .Append(");")
                .ToString();
        }

        private static string FormatArgument(Array arguments, int currentIndex)
        {
            if (arguments.Length <= currentIndex)
                return "missing parameter";

            var argument = arguments.GetValue(currentIndex);
            var array = argument as Array;
            if (array != null)
            {
                var buffer = new StringBuilder()
                    .Append("[");

                for (int index = 0; index < array.Length; index++)
                {
                    buffer.Append(FormatArgument(array, index))
                        .Append(", ");
                }

                return buffer
                    .Remove(buffer.Length - 2, 2)
                    .Append("]")
                    .ToString();
            }

            if (argument is string)
                return string.Format("\"{0}\"", argument);

            if (argument == null)
                return "null";

            var mockArgument = argument as IMockInstance;
            if (mockArgument != null)
                return mockArgument.GetType().BaseType.FullName;

            return argument.ToString();
        }
    }
}
