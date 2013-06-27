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
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Utilities
{
    /// <summary>
    /// Utiliy class for dealing with Generics
    /// </summary>
	public static class GenericsUtil
	{
		/// <summary>
        /// Determines if the type is an open generic type.
        /// </summary>
        /// <remarks>
        /// There are issues with trying to get this to work correctly 
        /// with open generic types, since this is an edge case, 
        /// I am letting the runtime handle it.
        /// </remarks>
        /// <param name="type">type to check</param>
        /// <returns>true if type is open generic; otherwise false</returns>
        public static bool HasOpenGenericParam(Type type)
		{
			// not bound to particular type, only way I know of 
            // doing this, since IsGeneric and IsGenericTypeDefination 
            // will both lie when used with generic method parameters
			if (type.FullName == null)
				return true;

			foreach (Type argument in type.GetGenericArguments())
			{
				if (argument.FullName == null)
					return true;

				if (argument.IsGenericType)
				{
					if (HasOpenGenericParam(argument))
						return true;
				}
			}

			return false;
		}

		/// <summary>
        /// Identifies the actual type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="invocation">The invocation.</param>
		/// <returns>actual type</returns>
		public static Type GetRealType(Type type, IInvocation invocation)
		{
			if (!HasOpenGenericParam(type))
				return type;

            // if the AssemblyQualifiedName is null, we have an open type
			string name = type.AssemblyQualifiedName ?? type.Name; 

			Dictionary<string, Type> names = CreateTypesTableFromInvocation(invocation);
			if (names.ContainsKey(name))
				return names[name];

			return ReconstructGenericType(type, names);
		}

		/// <summary>
		/// Because we need to support complex types here we
		/// need to be aware of the following scenarios:
		/// List[T] and List[Foo[T]]
		/// </summary>
		private static Type ReconstructGenericType(Type type, Dictionary<string, Type> names)
		{
			List<Type> collection = new List<Type>();
			foreach (Type argument in type.GetGenericArguments())
			{
                Type argumentType;
                if (!names.TryGetValue(argument.Name, out argumentType))
                    argumentType = ReconstructGenericType(argument, names);

                collection.Add(argumentType);
			}

			Type definition = type.GetGenericTypeDefinition();
			return definition.MakeGenericType(collection.ToArray());
		}

		private static Dictionary<string, Type> CreateTypesTableFromInvocation(IInvocation invocation)
		{
			Type genericType = GetTypeWithGenericArgumentsForMethodParameters(invocation);

			Type[] types = genericType.GetGenericArguments();
			Type[] arguments = genericType.GetGenericTypeDefinition().GetGenericArguments();

            Dictionary<string, Type> names = new Dictionary<string, Type>();
			for (int i = 0; i < arguments.Length; i++)
			{
				string name = arguments[i].Name;
				names[name] = types[i];
			}

			return names;
		}

		private static Type GetTypeWithGenericArgumentsForMethodParameters(IInvocation invocation)
		{
			Type invocationType = invocation.GetType();
			if (invocationType.IsGenericType)
				return invocationType;

			Type type = MockRepository
                .GetMockedObject(invocation.Proxy)
                .GetDeclaringType(invocation.Method);

			if (type != null)
                return type;

            throw new InvalidOperationException("BUG: Could not find a declaring type for method " + invocation.Method);
		}
	}
}
