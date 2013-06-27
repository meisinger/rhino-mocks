using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks.Constraints
{
    /// <summary>
    /// Provides a dummy field to pass as out or ref argument
    /// </summary>
    /// <typeparam name="T">The type of the argument</typeparam>
	public class OutRefArgDummy<T>
	{
		/// <summary>
		/// Dummy field to satisfy the compiler. Used for out and ref arguments.
		/// </summary>
		public T Dummy;
	}

}
