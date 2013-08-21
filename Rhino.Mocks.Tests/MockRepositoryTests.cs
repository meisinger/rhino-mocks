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
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
	public class MockRepositoryTests
	{
		private IDemo demo;

		public MockRepositoryTests()
		{
            demo = Repository.Mock<IDemo>();
		}

		[Fact]
		public void CreatesNewMockObject()
		{
			Assert.NotNull(demo);
		}

		[Fact]
		public void CallMethodOnMockObject()
		{
            demo.Expect(x => x.ReturnStringNoArgs());
		}

		[Fact]
		public void RecordWithBadReplayCauseException()
		{
            demo.Expect(x => x.ReturnStringNoArgs())
                .Return(string.Empty);

            Assert.Throws<ExpectationViolationException>(
                "IDemo.ReturnStringNoArgs(); Expected #1, Actual #0.",
                () => demo.VerifyAllExpectations());
		}

		[Fact]
		public void RecordTwoMethodsButReplayOneCauseException()
		{
            demo.Expect(x => x.ReturnStringNoArgs())
                .Return(string.Empty)
                .Repeat.Twice();

			demo.ReturnStringNoArgs();

			Assert.Throws<ExpectationViolationException>(
				"IDemo.ReturnStringNoArgs(); Expected #2, Actual #1.",
				() => demo.VerifyAllExpectations());
		}

		[Fact(Skip = "Test No Longer Valid (State No Longer Exists)")]
		public void UsingVerifiedObjectThrows()
		{
            demo.VerifyAllExpectations();

			Assert.Throws<InvalidOperationException>(
				"This action is invalid when the mock object is in verified state.",
				() => demo.ReturnIntNoArgs());
		}

		[Fact]
		public void SetReturnValue()
		{
            string retVal = "Ayende";

            demo.Expect(x => x.ReturnStringNoArgs())
                .Return(retVal);

			Assert.Equal(retVal, demo.ReturnStringNoArgs());
			demo.VerifyAllExpectations();
		}

		[Fact]
		public void SetReturnValueAndNumberOfRepeats()
		{
            string retVal = "Ayende";

            demo.Expect(x => x.ReturnStringNoArgs())
                .Return(retVal)
                .Repeat.Twice();
			
			Assert.Equal(retVal, demo.ReturnStringNoArgs());
			Assert.Equal(retVal, demo.ReturnStringNoArgs());
            demo.VerifyAllExpectations();
		}

		[Fact]
		public void SetMethodToThrow()
		{
            demo.Expect(x => x.VoidStringArg("test"))
                .Throws<ArgumentException>();

			Assert.Throws<ArgumentException>(
                "Reserved value, must be zero",
                () => demo.VoidStringArg("test"));
		}

		[Fact]
		public void SettingMethodToThrowTwice()
		{
            string exceptionMessage = "Reserved value, must be zero";

            demo.Expect(x => x.VoidStringArg("test"))
                .Throws(new ArgumentException(exceptionMessage))
                .Repeat.Twice();

			for (int i = 0; i < 2; i++)
			{
				try
				{
					demo.VoidStringArg("test");
					Assert.False(true, "Expected exception");
				}
				catch (ArgumentException e)
				{
					Assert.Equal(exceptionMessage, e.Message);
				}
			}
		}

		[Fact]
		public void ReturnningValueType()
		{
            demo.Expect(x => x.ReturnIntNoArgs())
                .Return(2);

			Assert.Equal(2, demo.ReturnIntNoArgs());
		}

		[Fact(Skip = "Test No Longer Valid")]
		public void CallingSecondMethodWithoutSetupRequiredInfoOnFirstOne()
		{
            demo.Expect(x => x.ReturnIntNoArgs());

            Assert.Throws<InvalidOperationException>(
                "Previous method 'IDemo.ReturnIntNoArgs();' requires a return value or an exception to throw.",
                () => demo.ReturnIntNoArgs());
		}

		[Fact]
		public void ReturnDerivedType()
		{
            demo.Expect(x => x.EnumNoArgs())
                .Return(DemoEnum.Demo);
		}

		[Fact]
		public void SetExceptionAndThenSetReturn()
		{
            demo.Expect(x => x.EnumNoArgs())
                .Throws<Exception>();

            demo.Expect(x => x.EnumNoArgs())
                .Return(DemoEnum.Demo);

            Assert.Throws<Exception>(() => demo.EnumNoArgs());

			DemoEnum d = DemoEnum.NonDemo;
			d = (DemoEnum)demo.EnumNoArgs();

			Assert.Equal(d, DemoEnum.Demo);
		}

        [Fact(Skip = "Test Passes - Wrong Message")]
		public void ExpectMethodOnce()
		{
            demo.Expect(x => x.EnumNoArgs())
                .Return(DemoEnum.NonDemo);

			DemoEnum d = (DemoEnum)demo.EnumNoArgs();
			Assert.Equal(d, DemoEnum.NonDemo);

            demo.EnumNoArgs();

            Assert.Throws<ExpectationViolationException>(
                "IDemo.EnumNoArgs(); Expected #1, Actual #2.",
                () => demo.VerifyExpectations(true));
		}

		[Fact]
		public void ExpectMethodAlways()
		{
            demo.Expect(x => x.EnumNoArgs())
                .Return(DemoEnum.NonDemo)
                .Repeat.Any();

			demo.EnumNoArgs();
			demo.EnumNoArgs();
			demo.EnumNoArgs();

            demo.VerifyAllExpectations();
		}

		[Fact]
		public void DifferentArgumentsCauseException()
		{
            demo.Expect(x => x.VoidStringArg("Hello"));
            demo.VoidStringArg("World");

			Assert.Throws<ExpectationViolationException>(
				"IDemo.VoidStringArg(\"World\"); Expected #0, Actual #1.\r\nIDemo.VoidStringArg(\"Hello\"); Expected #1, Actual #0.",
				() => demo.VerifyExpectations(true));
		}

		[Fact]
		public void VerifyingArguments()
		{
            demo.Expect(x => x.VoidStringArg("Hello"));

            demo.VoidStringArg("Hello");
            demo.VerifyAllExpectations();
		}

		[Fact]
		public void IgnoreArgument()
		{
            demo.Expect(x => x.VoidStringArg("Hello"))
                .IgnoreArguments();

			demo.VoidStringArg("World");
            demo.VerifyAllExpectations();
		}

		[Fact]
		public void IgnoreArgsAndReturnValue()
		{
            string objToReturn = "World";

            demo.Expect(x => x.StringArgString("Hello"))
                .IgnoreArguments()
                .Return(objToReturn)
                .Repeat.Twice();

			Assert.Equal(objToReturn, demo.StringArgString("foo"));
			Assert.Equal(objToReturn, demo.StringArgString("bar"));

            demo.VerifyAllExpectations();
		}

		[Fact]
		public void RepeatThreeTimes()
		{
            string objToReturn = "World";

            demo.Expect(x => x.StringArgString("Hello"))
                .IgnoreArguments()
                .Return(objToReturn)
                .Repeat.Times(3);

			Assert.Equal(objToReturn, demo.StringArgString("foo"));
			Assert.Equal(objToReturn, demo.StringArgString("bar"));
			Assert.Equal(objToReturn, demo.StringArgString("bar"));

            demo.VerifyAllExpectations();
		}

		[Fact]
		public void RepeatOneToThreeTimes()
		{
            string objToReturn = "World";

            demo.Expect(x => x.StringArgString("Hello"))
                .IgnoreArguments()
                .Return(objToReturn)
                .Repeat.AtMost(3);

			Assert.Equal(objToReturn, demo.StringArgString("foo"));
			Assert.Equal(objToReturn, demo.StringArgString("bar"));

            demo.VerifyAllExpectations();
		}

		[Fact]
		public void ThrowingExceptions()
		{
            demo.Expect(x => x.StringArgString("Ayende"))
                .IgnoreArguments()
                .Throws<Exception>();
			
			Assert.Throws<Exception>(
                "Ugh! It's alive!",
                () => demo.StringArgString(null));
		}

		[Fact]
		public void ThrowingExceptionsWhenOrdered()
		{
            demo.Expect(x => x.StringArgString("Ayende"))
                .IgnoreArguments()
                .Throws<Exception>();

            Assert.Throws<Exception>(
                "Ugh! It's alive!",
                () => demo.StringArgString(null));
		}

		[Fact]
		public void ExpectationExceptionWhileUsingDisposableThrowTheCorrectExpectation()
		{
            demo.VoidNoArgs();

			Assert.Throws<ExpectationViolationException>(
				"IDemo.VoidNoArgs(); Expected #0, Actual #1.",
				() => demo.VerifyExpectations(true));
		}

		[Fact]
		public void MockObjectThrowsForUnexpectedCall()
		{
			IDemo demo = Repository.Mock<IDemo>();

            demo.VoidNoArgs();

            Assert.Throws<ExpectationViolationException>(
                "IDemo.VoidNoArgs(); Expected #0, Actual #1.",
                () => demo.VerifyExpectations(true));
		}
        
		[Fact]
		public void MockObjectThrowsForUnexpectedCall_WhenVerified_IfFirstExceptionWasCaught()
		{
            IDemo demo = Repository.Mock<IDemo>();
			
			try
			{
				demo.VoidNoArgs();
			}
			catch (Exception) { }

			Assert.Throws<ExpectationViolationException>(
				"IDemo.VoidNoArgs(); Expected #0, Actual #1.",
				() => demo.VerifyExpectations(true));
		}

		[Fact]
		public void DyamicMockAcceptUnexpectedCall()
		{
            IDemo demo = Repository.Mock<IDemo>();
			
			demo.VoidNoArgs();
            demo.VerifyAllExpectations();
		}

		[Fact]
		public void RepositoryThrowsWithConstructorArgsForMockInterface()
		{
            Assert.Throws<ArgumentException>(() =>
            {
                IDemo demo = Repository.Mock<IDemo>("Foo");
            });
		}

		[Fact]
		public void RepositoryThrowsWithConstructorArgsForMockDelegate()
		{
            Assert.Throws<ArgumentException>(() =>
            {
                EventHandler handler = Repository.Mock<EventHandler>("Foo");
            });
		}

		[Fact]
		public void RepositoryThrowsWithWrongConstructorArgsForMockClass()
		{
			try
            {
                object o = Repository.Mock<object>("Foo");
   
                Assert.False(true, "The above call should have failed");
            }
            catch(ArgumentException argEx)
            {
				Assert.Contains("Can not instantiate proxy of class: System.Object.", argEx.Message);
            }
		}

        [Fact]
        public void GenerateMockForClassWithNoDefaultConstructor() 
        {
            Assert.NotNull(Repository.Partial<ClassWithNonDefaultConstructor>(null, 0));            
        }

        [Fact]
        public void GenerateMockForClassWithDefaultConstructor() 
        {
            Assert.NotNull(Repository.Partial<ClassWithDefaultConstructor>());
        }

        [Fact]
        public void GenerateMockForInterface() 
        {
            Assert.NotNull(Repository.Mock<IDemo>());
        }

		[Fact]
		public void GenerateStrictMockWithRemoting()
		{
            IDemo mock = Repository.Mock<IDemo>();
			Assert.NotNull(mock);
		}

		[Fact]
		public void GenerateDynamicMockWithRemoting()
		{
            IDemo mock = Repository.Mock<IDemo>();
			Assert.NotNull(mock);
        }

		public class ClassWithNonDefaultConstructor 
        {
            public ClassWithNonDefaultConstructor(string someString, int someInt) {}
        }

        public class ClassWithDefaultConstructor {}        

		private enum DemoEnum
		{
			Demo,
			NonDemo
		}
	}
}
