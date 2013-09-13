using System;
using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Helpers;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Expectations
{
    /// <summary>
    /// Represents an expectation against a method call.
    /// Allows access to various options that can be applied to an expectation
    /// </summary>
    public class ExpectProperty<T> : Expectation<T>, IPropertyOptions<T>
    {
        internal override ExpectationType Type
        {
            get { return ExpectationType.Property; }
        }

        /// <summary>
        /// Constraints against argument of the expectation
        /// </summary>
        public AbstractConstraint[] Arguments { get; set; }

        /// <summary>
        /// Get Method of the expectation
        /// </summary>
        public MethodInfo MethodGet { get; set; }

        /// <summary>
        /// Set Method of the expectation
        /// </summary>
        public MethodInfo MethodSet { get; set; }

        /// <summary>
        /// Property of the expectation
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public ExpectProperty()
            : this(new Range(1, null))
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="range"></param>
        protected ExpectProperty(Range range)
            : base(range)
        {
        }

        /// <summary>
        /// Returns the string representation of the expectation
        /// </summary>
        /// <returns>string</returns>
        public override string GetDisplayName(IInvocation invocation)
        {
            return MethodFormatter.ToString(invocation, MethodGet, Arguments,
                (x, i) => Arguments[i].Message);
        }

        /// <summary>
        /// Handles the call
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        public override void HandleMethodCall(MethodInfo method, object[] arguments)
        {
            if (!method.IsSpecialName)
                throw new InvalidOperationException("Property expectations can only be set for properties.");

            var methodName = method.Name;
            if (!methodName.StartsWith("get_") && !methodName.StartsWith("set_"))
                throw new InvalidOperationException("Property expectations can only be set for properties.");

            var propertyName = method.Name.Substring(4);
            var property = method.DeclaringType.GetProperty(propertyName);

            MethodGet = property.GetGetMethod(true);
            MethodSet = property.GetSetMethod(true);

            if (ArgumentManager.HasBeenUsed)
            {
                ArgumentManager.ValidateMethodSignature(method);
                Arguments = ArgumentManager.GetConstraints();
                ReturnArguments = ArgumentManager.GetReturnValues();
                SetExpectedCount(new Range(1, null));
                ArgumentManager.Clear();

                return;
            }

            var parameters = method.GetParameters();
            var constraints = new AbstractConstraint[parameters.Length];
            for (int index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                constraints[index] = (parameter.IsOut)
                    ? Is.Anything()
                    : Is.Equal(arguments[index]);
            }

            Arguments = constraints;
        }

        /// <summary>
        /// Checks that the given method and arguments
        /// match the expectation and argument constraints
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public override bool MatchesCall(MethodInfo method, object[] arguments)
        {
            var argumentsMatch = MatchesCallArguments(arguments);
            if (MethodGet != null && MethodGet.Equals(method))
                return argumentsMatch;

            if (MethodSet != null && MethodSet.Equals(method))
                return argumentsMatch;

            return false;
        }

        /// <summary>
        /// Checks that the given arguments match the argument constraints
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool MatchesCallArguments(object[] arguments)
        {
            if (Arguments == null && arguments == null)
                return true;

            if (Arguments == null || arguments == null)
                return false;

            if (Arguments.Length != arguments.Length)
                return false;

            for (int index = 0; index < Arguments.Length; index++)
            {
                var argument = arguments[index];
                var constraint = Arguments[index];

                if (!constraint.Eval(argument))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Call original property
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> IPropertyOptions<T>.CallOriginalProperty()
        {
            //if (Method.IsAbstract)
            //{
            //    var message = string.Format(
            //        "Can't use CallOriginalMethod on method {0} because the method is abstract.",
            //            Method.Name);

            //    throw new InvalidOperationException(message);
            //}

            ForceProceed = true;
            return this;
        }

        /// <summary>
        /// Ignores all arguments removing any existing argument constraints
        /// </summary>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> IPropertyOptions<T>.IgnoreArguments()
        {
            var constraints = new AbstractConstraint[Arguments.Length];
            for (int index = 0; index < Arguments.Length; index++)
                constraints[index] = Is.Anything();

            Arguments = constraints;
            return this;
        }

        /// <summary>
        /// Define the return value of a property (non write-only)
        /// </summary>
        /// <param name="value">Value to return when "get" is called</param>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> IPropertyOptions<T>.Return(T value)
        {
            if (MethodGet == null)
                throw new InvalidOperationException("Return value cannot be set for a write-only property.");

            SetReturnValue(value);
            return this;
        }

        /// <summary>
        /// Define the return value of a property (non write-only)
        /// </summary>
        /// <param name="func">Value to return when "get" is called</param>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> IPropertyOptions<T>.Returns(Func<T> func)
        {
            if (MethodGet == null)
                throw new InvalidOperationException("Return value cannot be set for a write-only property.");

            SetReturnValue(func());
            return this;
        }

        /// <summary>
        /// Throw exception of the given type when the property is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> IPropertyOptions<T>.Throws<TException>()
        {
            ThrowsException = true;
            ExceptionToThrow = new TException();
            return this;
        }

        /// <summary>
        /// Throw exception of the given type when the property is called
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="exception"></param>
        /// <returns>Fluid Interface</returns>
        IPropertyOptions<T> IPropertyOptions<T>.Throws<TException>(TException exception)
        {
            ThrowsException = true;
            ExceptionToThrow = exception;
            return this;
        }
    }
}
