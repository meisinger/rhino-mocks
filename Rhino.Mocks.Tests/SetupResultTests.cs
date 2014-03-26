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
	public class SetupResultTests
	{
		private IDemo demo;

		public SetupResultTests()
		{
            demo = Repository.Mock<IDemo>();
		}

        [Fact]
        public void CanSetupResultForMethodAndIgnoreArgs()
        {
            demo.Expect(x => x.StringArgString(null))
                .IgnoreArguments()
                .Return("Ayende")
                .Repeat.Any();

            Assert.Equal("Ayende", demo.StringArgString("a"));
            Assert.Equal("Ayende", demo.StringArgString("b"));

            demo.VerifyAllExpectations();
        }
	    
		[Fact]
		public void CanSetupResult()
		{
            demo.Expect(x => x.Prop)
                .Return("Ayende");

			Assert.Equal("Ayende", demo.Prop);

            demo.VerifyAllExpectations();
		}

        [Fact]
		public void SetupResultCanRepeatAsManyTimeAsItWant()
		{
            demo.Expect(x => x.Prop)
                .Return("Ayende")
                .Repeat.Any();

			for (int i = 0; i < 30; i++)
			{
				Assert.Equal("Ayende", demo.Prop);
			}

            demo.VerifyAllExpectations();
		}

		[Fact]
		public void SetupResultUsingOn()
		{
            demo.Expect(x => x.Prop)
                .Return("Ayende")
                .Repeat.Any();

			for (int i = 0; i < 30; i++)
			{
				Assert.Equal("Ayende", demo.Prop);
			}

            demo.VerifyAllExpectations();
		}

		[Fact]
		public void SetupResultUsingOrdered()
		{
            demo.Expect(x => x.Prop)
                .Return("Ayende")
                .Repeat.Any();

            demo.Expect(x => x.VoidNoArgs())
                .Repeat.Twice();

			demo.VoidNoArgs();

			for (int i = 0; i < 30; i++)
			{
				Assert.Equal("Ayende", demo.Prop);
			}

			demo.VoidNoArgs();

            demo.VerifyAllExpectations();
		}

		[Fact]
		public void ExpectNever()
		{
            demo.Expect(x => x.ReturnStringNoArgs())
                .Repeat.Never();

            demo.ReturnIntNoArgs();

            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations(true));
		}

		[Fact(Skip = "Test Does Not Make Sense")]
		public void ExpectNeverSetupTwiceThrows()
		{
            demo.Expect(x => x.ReturnStringNoArgs())
                .Repeat.Never();

            Assert.Throws<InvalidOperationException>(
                () => demo.Expect(x => x.ReturnStringNoArgs())
                    .Repeat.Never());
		}
	}
}