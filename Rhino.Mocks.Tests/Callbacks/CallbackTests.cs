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
using System.Runtime.InteropServices;
using Xunit;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.Callbacks
{
	public class CallbackTests
	{
		private IDemo demo;
		private bool callbackCalled;

		public CallbackTests()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            demo = Repository.Mock<IDemo>();
			callbackCalled = false;
		}

		[Fact]
		public void CallbackIsCalled()
		{
            demo.Expect(x => x.VoidStringArg("Ayende"))
                .IgnoreArguments()
                .WhenCalled<string>(x => StringMethod(x));

			demo.VoidStringArg("");
            demo.VerifyExpectations(true);
			Assert.True(callbackCalled);
		}

		[Fact]
		public void GetSameArgumentsAsMethod()
		{
            demo.Expect(x => x.VoidThreeArgs(0, "", 0f))
                .IgnoreArguments()
                .WhenCalled<int, string, float>((x, y, z) => ThreeArgsAreSame(x, y, z));

            demo.VoidThreeArgs(1, "Ayende", 3.14f);
            demo.VerifyExpectations(true);
            Assert.True(callbackCalled);
		}

		[Fact]
		public void DifferentArgumentsFromMethodThrows()
		{
            Assert.Throws<InvalidOperationException>(
                "Callback arguments didn't match the method arguments",
                () => demo.Expect(x => x.VoidThreeArgs(0, "", 0f))
                        .WhenCalled<int, string, string>((x, y, z) => OtherThreeArgs(x, y, z)));
		}

		[Fact]
		public void IgnoreArgsWhenUsingCallbacks()
		{
            demo.Expect(x => x.VoidThreeArgs(0, "", 0f))
                .IgnoreArguments()
                .WhenCalled<int, string, float>((x, y, z) => ThreeArgsAreSame(x, y, z));

            demo.VoidThreeArgs(1, "Ayende", 3.14f);
            demo.VerifyExpectations(true);
		}

		[Fact]
		public void SetReturnValueOnMethodWithCallback()
		{
            demo.Expect(x => x.ReturnIntNoArgs())
                .WhenCalled(() => NoArgsMethod())
                .Return(5);

            Assert.Equal(5, demo.ReturnIntNoArgs());
            demo.VerifyExpectations(true);
		}

		[Fact]
		public void CallbackWithDifferentSignatureFails()
		{
            Assert.Throws<InvalidOperationException>(
                "Callback arguments didn't match the method arguments",
                () => demo.Expect(x => x.VoidThreeArgs(0, "", 0f))
                        .WhenCalled<string>(x => StringMethod(x)));
		}

		[Fact]
		public void GetMessageFromCallbackWhenNotReplaying()
		{
            demo.Expect(x => x.VoidThreeArgs(0, "", 0f))
                .WhenCalled<int, string, float>((x, y, z) => ThreeArgsAreSame(x, y, z));

            Assert.Throws<ExpectationViolationException>(
                "IDemo.VoidThreeArgs(callback method: CallbackTests.ThreeArgsAreSame); Expected #1, Actual #0.",
                () => demo.VerifyExpectations(true));
		}

		[Fact]
		public void GetMessageFromCallbackWhenCalledTooMuch()
		{
            demo.Expect(x => x.VoidThreeArgs(0, "", 0f))
                .WhenCalled<int, string, float>((x, y, z) => ThreeArgsAreSame(x, y, z))
                .Repeat.Once();

            demo.VoidThreeArgs(1, "Ayende", 3.14f);
            demo.VoidThreeArgs(1, "Ayende", 3.14f);

            Assert.Throws<ExpectationViolationException>(
                "IDemo.VoidThreeArgs(1, \"Ayende\", 3.14); Expected #1, Actual #2.",
                () => demo.VerifyExpectations(true));
		}
        
		[Fact(Skip = "Test No Longer Valid")]
		public void CallbackWhenMethodHasReturnValue()
		{
            demo.Expect(x => x.ReturnIntNoArgs())
                .WhenCalled(() => NoArgsMethod());

            Assert.Throws<InvalidOperationException>(
                "Previous method 'IDemo.ReturnIntNoArgs(callback method: CallbackTests.NoArgsMethod);' requires a return value or an exception to throw.",
                () => demo.ReturnIntNoArgs());
		}
        
		[Fact(Skip = "Test No Longer Valid (Constraint Removed)")]
		public void CallbackAndConstraintsOnSameMethod()
		{
            Assert.Throws<InvalidOperationException>(
                "This method has already been set to CallbackExpectation.",
                () => demo.Expect(x => x.StringArgString(Arg<string>.Is.Anything))
                    .WhenCalled<string>(x => StringMethod(x)));
		}

		[Fact]
		public void ExceptionInCallback()
		{
            demo.Expect(x => x.ReturnIntNoArgs())
                .WhenCalled(() => NoArgsThrowing())
                .Return(5);

			Assert.Throws<ExternalException>(
                "I'm not guilty, is was /him/",
                () => Assert.Equal(5, demo.ReturnIntNoArgs()));
		}

		[Fact]
		public void CallbackCanFailExpectationByReturningFalse()
		{
            demo.Expect(x => x.VoidNoArgs())
                .WhenCalled(() => NoArgsMethodFalse());

            demo.VoidThreeArgs(1, "Ayende", 3.14f);

			Assert.Throws<ExpectationViolationException>(
                "IDemo.VoidThreeArgs(1, \"Ayende\", 3.14); Expected #0, Actual #1.",
                () => demo.VerifyExpectations(true));
		}

		private bool StringMethod(string s)
		{
			callbackCalled = true;
			return true;
		}

		private bool OtherThreeArgs(int i, string s, string s2)
		{
			return true;
		}

		private bool ThreeArgsAreSame(int i, string s, float f)
		{
			Assert.Equal(1, i);
			Assert.Equal("Ayende", s);
			Assert.Equal(3.14f, f);

			callbackCalled = true;
			return true;
		}

		private bool NoArgsMethod()
		{
			return true;
		}

		private bool NoArgsMethodFalse()
		{
			return false;
		}

		private bool NoArgsThrowing()
		{
			throw new ExternalException("I'm not guilty, is was /him/");
		}
	}

	public class DelegateDefinations
	{
		public delegate void VoidThreeArgsDelegate(int i, string s, float f);

		public delegate bool StringDelegate(string s);

		public delegate bool ThreeArgsDelegate(int i, string s, float f);
		public delegate bool OtherThreeArgsDelegate(int i, string s, string s2);

		public delegate bool NoArgsDelegate();
		public delegate bool IntArgDelegate(int i);

	}
}
