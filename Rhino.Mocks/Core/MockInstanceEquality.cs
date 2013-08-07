using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Rhino.Mocks.Core.Interfaces;

namespace Rhino.Mocks.Core
{
    /// <summary>
    /// Generates hash codes for mocked instances and 
    /// provides methods for comparison between them
    /// </summary>
    internal class MockInstanceEquality : IComparer, IEqualityComparer, IEqualityComparer<object>
    {
        private static readonly MockInstanceEquality instance = new MockInstanceEquality();
        private static int hashcode;

        /// <summary>
        /// Singleton
        /// </summary>
        internal static MockInstanceEquality Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Returns the next hash code for a mock instance
        /// </summary>
        internal static int NextHash
        {
            get { return Interlocked.Increment(ref hashcode); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private MockInstanceEquality()
        {
        }

        /// <summary>
        /// Determines if two mock instances are
        /// equal (same hash code)
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        internal bool AreEqual(object arg1, object arg2)
        {
            var result = Compare(arg1, arg2);
            return (result == 0);
        }

        /// <summary>
        /// Gets the mock instance hash code when
        /// the object is a mock; otherwise returns
        /// the hash code of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(object obj)
        {
            var mock = GetMockOrNull(obj);
            if (mock == null)
                return obj.GetHashCode();

            return mock.HashCode;
        }

        /// <summary>
        /// Compares two mock instances
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return 1;

            if (y == null)
                return -1;

            var mockX = GetMockOrNull(x);
            var mockY = GetMockOrNull(y);

            if (mockX == null && mockY == null)
                return -2;

            if (mockX == null)
                return 1;

            if (mockY == null)
                return -1;

            return (mockX.HashCode - mockY.HashCode);
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            return AreEqual(x, y);
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return AreEqual(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return GetHashCode(obj);
        }

        private static IMockInstance GetMockOrNull(object obj)
        {
            var mockDelegate = obj as Delegate;
            if (mockDelegate != null)
                obj = mockDelegate.Target;

            var mock = obj as IMockInstance;
            return mock;
        }
    }
}
