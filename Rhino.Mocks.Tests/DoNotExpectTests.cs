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
using Xunit;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
    public class DoNotExpectTests
    {
        private IDemo demo;

		public DoNotExpectTests()
        {
            demo = MockRepository.Mock<IDemo>();
        }

        [Fact]
        public void ShouldNotExpect()
        {
            demo.Expect(x => x.StringArgString("Ayende"))
                .Repeat.Never();

            demo.StringArgString("Ayende");

            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations());
        }

        [Fact]
        public void CanUseAnonymousDelegatesToCallVoidMethods()
        {
            demo.Expect(x => x.VoidNoArgs())
                .Repeat.Never();

            demo.VoidNoArgs();

            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations());
        }

        [Fact]
        public void CanUseAnonymousDelegatesToCallVoidMethods_WithStringArg()
        {
            demo.Expect(x => x.VoidStringArg("Ayende"))
                .Repeat.Never();

            demo.VoidStringArg("Ayende");

            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations());
        }

        [Fact]
        public void CanUseAnonymousDelegatesToCallVoidMethods_WithoutAnonymousDelegate()
        {
            demo.Expect(x => x.VoidNoArgs())
                .Repeat.Never();

            demo.VoidNoArgs();

            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations());
        }

        [Fact]
        public void CanUseAnonymousDelegatesToCallStringMethods_WithoutAnonymousDelegate()
        {
            demo.Expect(x => x.StringArgString("Ayende"))
                .Repeat.Never();

            demo.StringArgString("Ayende");

            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations());
        }

        [Fact]
        public void DoNotExpectCallRespectsArguments()
        {
            demo.Expect(x => x.StringArgString("Ayende"))
                .Repeat.Never();

            demo.StringArgString("Ayende");
            demo.StringArgString("Sneal");

            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations());
        }

        [Fact]
        public void CanUseDoNotCallOnPropertySet()
        {
            demo.Expect(x => x.Prop = "Ayende")
                .Repeat.Never();

            demo.Prop = "Ayende";

            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations());
        }

        [Fact]
        public void CanUseDoNotCallOnPropertyGet()
        {
            demo.Expect(x => x.Prop)
                .Repeat.Never();

            var soItCompiles = demo.Prop;

            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations());
        }
    }
}
