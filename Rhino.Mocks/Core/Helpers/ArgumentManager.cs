using System;
using System.Collections.Generic;
using System.Reflection;
using Rhino.Mocks.Core.Constraints;

namespace Rhino.Mocks.Core.Helpers
{
    /// <summary>
    /// Responsible for managing <see cref=" Arg&lt;T&gt;"/> class
    /// </summary>
    internal class ArgumentManager
    {
        [ThreadStatic]
        private static List<ArgumentInstance> arguments;

        /// <summary>
        /// Indicates whether the <see cref="Arg&lt;T&gt;"/> class
        /// has been used
        /// </summary>
        internal static bool HasBeenUsed
        {
            get { return (arguments != null && (arguments.Count > 0)); }
        }

        /// <summary>
        /// Resets the internal state of stored arguments
        /// </summary>
        internal static void Clear()
        {
            if (arguments != null)
                arguments.Clear();
        }

        /// <summary>
        /// Adds a constraint for an argument
        /// </summary>
        /// <param name="constraint"></param>
        internal static void AddArgument(AbstractConstraint constraint)
        {
            Initialize();
            arguments.Add(new ArgumentInstance(constraint));
        }

        /// <summary>
        /// Adds a constraint for an "out" argument
        /// </summary>
        /// <param name="value"></param>
        internal static void AddOutArgument(object value)
        {
            Initialize();
            arguments.Add(new ArgumentInstance(value));
        }

        /// <summary>
        /// Adds a constraint for a "ref" argument
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="value"></param>
        internal static void AddRefArgument(AbstractConstraint constraint, object value)
        {
            Initialize();
            arguments.Add(new ArgumentInstance(constraint, value));
        }

        /// <summary>
        /// Returns the constraints for all arguments
        /// </summary>
        /// <returns></returns>
        internal static AbstractConstraint[] GetConstraints()
        {
            Initialize();

            var constraints = new List<AbstractConstraint>();
            for (int index = 0; index < arguments.Count; index++)
            {
                var argument = arguments[index];
                constraints.Add(argument.Constraint);
            }

            return constraints.ToArray();
        }

        /// <summary>
        /// Returns the return value for "out" and "ref" arguments
        /// </summary>
        /// <returns></returns>
        internal static object[] GetReturnValues()
        {
            Initialize();

            var values = new List<object>();
            for (int index = 0; index < arguments.Count; index++)
            {
                var argument = arguments[index];
                if (argument.Type == ArgumentType.ArgumentIn)
                    continue;

                values.Add(argument.ReturnValue);
            }

            return values.ToArray();
        }

        /// <summary>
        /// Validates method parameters against the arguments set
        /// </summary>
        /// <param name="method"></param>
        internal static void ValidateMethodSignature(MethodInfo method)
        {
            Initialize();

            var parameters = method.GetParameters();
            if (arguments.Count < parameters.Length)
            {
                throw new InvalidOperationException(
                    string.Format("When using Arg<T>, all arguments must be defined using Arg<T>.Is, Arg<T>.Text, Arg<T>.List, Arg<T>.Ref or Arg<T>.Out. {0} arguments expected, {1} have been defined.",
                        parameters.Length, arguments.Count));
            }

            if (arguments.Count > parameters.Length)
            {
                throw new InvalidOperationException(
                    string.Format("Use Arg<T> only while creating an expectation. {0} arguments expected, {1} have been defined.",
                        parameters.Length, arguments.Count));
            }

            for (int index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                var argument = arguments[index];

                if (parameter.IsOut)
                {
                    if (argument.Type == ArgumentType.ArgumentOut)
                        continue;

                    throw new InvalidOperationException(
                        string.Format("Argument at index {0} must be defined as: out Arg<T>.Out(returnvalue).Dummy", index));
                }

                if (parameter.ParameterType.IsByRef)
                {
                    if (argument.Type == ArgumentType.ArgumentRef)
                        continue;

                    throw new InvalidOperationException(
                        string.Format("Argument at index {0} must be defined as: ref Arg<T>.Ref(constraint, returnvalue).Dummy", index));
                }

                if (argument.Type != ArgumentType.ArgumentIn)
                {
                    throw new InvalidOperationException(
                        string.Format("Argument at index {0} must be defined using: Arg<T>.Is, Arg<T>.Text or Arg<T>.List", index));
                }
            }
        }

        private static void Initialize()
        {
            if (arguments == null)
                arguments = new List<ArgumentInstance>();
        }

        private enum ArgumentType
        {
            ArgumentIn,
            ArgumentOut,
            ArgumentRef
        }

        private struct ArgumentInstance
        {
            public AbstractConstraint Constraint;
            public ArgumentType Type;
            public object ReturnValue;

            public ArgumentInstance(AbstractConstraint constraint)
            {
                Constraint = constraint;
                Type = ArgumentType.ArgumentIn;
                ReturnValue = null;
            }

            public ArgumentInstance(AbstractConstraint constraint, object value)
            {
                Constraint = constraint;
                Type = ArgumentType.ArgumentRef;
                ReturnValue = value;
            }

            public ArgumentInstance(object value)
            {
                Constraint = Is.Anything();
                Type = ArgumentType.ArgumentOut;
                ReturnValue = value;
            }
        }
    }
}
