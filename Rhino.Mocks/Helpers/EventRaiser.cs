using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rhino.Mocks.Helpers
{
    /// <summary>
    /// Container for raising events for a given object
    /// </summary>
    public class EventRaiser
    {
        private readonly object instance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instance"></param>
        public EventRaiser(object instance)
        {
            this.instance = instance;
        }

        /// <summary>
        /// Invoke the event for the given object
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="args"></param>
        public void Raise(Delegate subscription, params object[] args)
        {
            AssertParametersMatch(subscription.Method, args);

            try
            {
                subscription.DynamicInvoke(args);
            }
            catch (TargetInvocationException ex)
            {
                PreserveStack(ex);
                throw ex.InnerException;
            }
        }

        private void AssertParametersMatch(MethodInfo method, object[] args)
        {
            var parameters = method.GetParameters();
            if (args == null || (parameters.Length != args.Length))
            {
                var expected = parameters.Length;
                var count = (args != null) ? args.Length : 0;

                var message = 
                    string.Format("An attempt was made to raise an event with the wrong number of arguments. Expected {0} but was {1}",
                        expected, count);

                throw new InvalidOperationException(message);
            }

            var errors = new List<string>();
            for (int index = 0; index < parameters.Length; index++)
            {
                var argument = args[index];
                var parameter = parameters[index];
                if ((argument == null && parameter.ParameterType.IsValueType) ||
                    (argument != null && !parameter.ParameterType.IsInstanceOfType(argument)))
                {
                    var type = "null";
                    if (argument != null)
                        type = argument.GetType().FullName;

                    errors.Add(string.Format("Parameter #{0} is of type \"{1}\" but should be of type \"{2}\".",
                        index + 1, type, parameter.ParameterType));
                }
            }

            if (errors.Count == 0)
                return;

            var errorMessage = string.Join(Environment.NewLine, errors.ToArray());
            throw new InvalidOperationException(errorMessage);
        }

        private void PreserveStack(Exception exception)
        {
            var method = typeof(Exception).GetMethod("InternalPreserveStackTrace", 
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(exception, null);
        }
    }
}
