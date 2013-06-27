#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Utilities
{
    /// <summary>
    /// Utility class for dealing with methods
    /// </summary>
	public static class MethodCallUtil
	{
		/// <summary>
		/// Delegate to format the argument for the string representation of
		/// the method call.
		/// </summary>
		public delegate string FormatArgumnet(Array args, int index);

		/// <summary>
		/// Return the string representation of a method call and its arguments.
		/// </summary>
		/// <param name="method">The method</param>
		/// <param name="args">The method arguments</param>
		/// <param name="invocation">Invocation of the method, used to get the generics arguments</param>
		/// <param name="format">Delegate to format the parameter</param>
		/// <returns>The string representation of this method call</returns>
		public static string StringPresentation(IInvocation invocation, FormatArgumnet format, MethodInfo method, object[] args)
		{
			Validate.IsNotNull(format, "format");
			Validate.IsNotNull(method, "method");
			Validate.IsNotNull(args, "args");

			StringBuilder buffer = new StringBuilder()
                .Append(method.DeclaringType.Name)
                .Append(".")
                .Append(method.Name);

			if (invocation != null && method.IsGenericMethod)
            {
                Type[] arguments = invocation.GenericArguments;

                buffer.Append("<");
                int argumentLength = arguments.Length;
                for (int i = 0; i < argumentLength; i++)
                {
                    buffer.Append(arguments[i]);
                    if (i < (argumentLength - 1))
                        buffer.Append(", ");
                }
                buffer.Append(">");
            }
            
			buffer.Append("(");
			int parameterLength = method.GetParameters().Length;
			for (int i = 0; i < parameterLength; i++)
			{
				buffer.Append(format(args, i));
				if (i < parameterLength - 1)
					buffer.Append(", ");
			}
			buffer.Append(");");

			return buffer.ToString();
		}

		/// <summary>
		/// Return the string representation of a method call and its arguments.
		/// </summary>
		/// <param name="invocation">The invocation of the method, used to get the generic parameters</param>
		/// <param name="method">The method</param>
		/// <param name="args">The method arguments</param>
		/// <returns>The string representation of this method call</returns>
		public static string StringPresentation(IInvocation invocation, MethodInfo method, object[] args)
		{
			return StringPresentation(invocation, new FormatArgumnet(DefaultFormatArgument), method, args);
		}

		private static string DefaultFormatArgument(Array args, int index)
		{
			if (args.Length <= index)
				return "missing parameter";

			object arg = args.GetValue(index);
			if (arg is Array)
			{
				Array arr = (Array) arg;
				StringBuilder buffer = new StringBuilder();
				buffer.Append('[');
				for (int j = 0; j < arr.Length; j++)
				{
					buffer.Append(DefaultFormatArgument(arr, j));
					if (j < arr.Length - 1)
						buffer.Append(", ");
				}
				buffer.Append("]");

				return buffer.ToString();
			}

			if (arg is string)
				return string.Format("\"{0}\"", arg);
			else if (arg == null)
				return "null";
			else
				return MockingSafeToString(arg);
		}

        /// <summary>
        /// We need to ensure that we won't reentrant into the
        /// repository if the parameter is a mock object
        /// </summary>
		private static string MockingSafeToString(object arg)
		{
			IMockedObject mock = arg as IMockedObject;
			if (mock == null)
				return arg.ToString();

			return mock.GetType().BaseType.FullName;
		}
	}
}
