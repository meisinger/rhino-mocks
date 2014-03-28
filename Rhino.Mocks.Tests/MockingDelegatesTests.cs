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
using System.IO;
using System.Reflection;
using Xunit;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
    public delegate object ObjectDelegateWithNoParams();
        
    public class MockingDelegatesTests
    {
        private delegate object ObjectDelegateWithNoParams();
        private delegate void VoidDelegateWithParams(string a);
        private delegate string StringDelegateWithParams(int a, string b);
        private delegate int IntDelegateWithRefAndOutParams(ref int a, out string b);

		public MockingDelegatesTests()
        {
        }

        [Fact]
        public void CallingMockedDelegatesWithoutOn()
        {
            ObjectDelegateWithNoParams d1 = MockRepository.Mock<ObjectDelegateWithNoParams>();

            d1.Expect(x => x())
                .Return(1);
            
            Assert.Equal(1, d1());
        }

        [Fact]
        public void MockTwoDelegatesWithTheSameName()
        {
            ObjectDelegateWithNoParams d1 = MockRepository.Mock<ObjectDelegateWithNoParams>();

            Tests.ObjectDelegateWithNoParams d2 = MockRepository.Mock<Tests.ObjectDelegateWithNoParams>();

            d1.Expect(x => x())
                .Return(1);

            d2.Expect(x => x())
                .Return(2);

            Assert.Equal(1, d1());
            Assert.Equal(2, d2());

            d1.VerifyAllExpectations();
            d2.VerifyAllExpectations();
        }

        [Fact]
        public void MockObjectDelegateWithNoParams()
        {
            ObjectDelegateWithNoParams d = MockRepository.Mock<ObjectDelegateWithNoParams>();

            d.Expect(x => x())
                .Return("abc");

            d.Expect(x => x())
                .Return("def");

            Assert.Equal("abc", d());
            Assert.Equal("def", d());
            Assert.Null(d());

            Assert.Throws<ExpectationViolationException>(
                () => d.VerifyExpectations(true));
        }

        [Fact]
        public void MockVoidDelegateWithNoParams()
        {
            VoidDelegateWithParams d = MockRepository.Mock<VoidDelegateWithParams>();

            d.Expect(x => x("abc"));
            d.Expect(x => x("efg"));

            d("abc");
            d("efg");
            d("hij");

            Assert.Throws<ExpectationViolationException>(
                () => d.VerifyExpectations(true));
        }

        [Fact]
        public void MockStringDelegateWithParams()
        {
            StringDelegateWithParams d = MockRepository.Mock<StringDelegateWithParams>();

            d.Expect(x => x(1, "111"))
                .Return("abc");

            d.Expect(x => x(2, "222"))
                .Return("def");

            Assert.Equal("abc", d(1, "111"));
            Assert.Equal("def", d(2, "222"));
            d(3, "333");

            Assert.Throws<ExpectationViolationException>(
                () => d.VerifyExpectations(true));
        }

        [Fact]
        public void DelegateBaseTypeCannotBeMocked()
        {
            Assert.Throws<InvalidOperationException>(
                () => MockRepository.Mock<Delegate>());
        }

        [Fact]
        public void GenericDelegate()
        {
            Action<int> action = MockRepository.Mock<Action<int>>();

            action.Expect(x =>
            {
                for (int i = 0; i < 10; i++)
                    x(i);
            });

            ForEachFromZeroToNine(action);

            action.VerifyAllExpectations();
        }

        private int Return1_Plus2_A(ref int a, out string b)
        {
            a += 2;
            b = "A";
            return 1;
        }

        private void ForEachFromZeroToNine(Action<int> act)
        {
            for (int i = 0; i < 10; i++)
            {
                act(i);
            }
        }
    }
}
