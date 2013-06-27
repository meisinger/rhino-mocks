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
using System.Collections;
using System.ServiceModel;
using Xunit;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Tests.Callbacks;

namespace Rhino.Mocks.Tests
{
	public class RhinoMockTests
	{
		private IDemo demo;

		public RhinoMockTests()
		{
			demo = MockRepository.GenerateStrictMock(typeof (IDemo)) as IDemo;
		}

		[Fact]
		public void CallsAreNotOrderDependant()
		{
            demo.Expect(x => x.ReturnStringNoArgs())
                .Return(null);

            demo.Expect(x => x.VoidStringArg("Hello"));

			demo.VoidStringArg("Hello");
			demo.ReturnStringNoArgs();
			
            demo.VerifyAllExpectations();
		}

		[Fact]
		public void OrderedCallsTrackingAsExpected()
		{
			RecordOrdered(demo);

			demo.ReturnStringNoArgs();
			demo.VoidNoArgs();
			demo.VoidNoArgs();
			demo.VoidStringArg("Hello");
			demo.VoidStringArg("World");

            demo.VerifyAllExpectations();
		}

        [Fact(Skip = "Test No Longer Valid (Ordering Removed)")]
		public void GetDocumentationMessageWhenExpectationNotMet()
		{
			RecordOrdered(demo);
			
			demo.ReturnStringNoArgs();
			demo.VoidNoArgs();
			
            Assert.Throws<ExpectationViolationException>(
				"Unordered method call! The expected call is: 'Ordered: { IDemo.VoidNoArgs(); }' but was: 'IDemo.VoidStringArg(\"Hello\");'",
				() => demo.VoidStringArg("Hello"));
		}

		[Fact]
		public void WillDisplayDocumentationMessageIfNotCalled()
		{
            demo.Expect(x => x.VoidNoArgs())
                .IgnoreArguments()
                .Message("Called to prefar foo for bar");

			Assert.Throws<ExpectationViolationException>(
				"Message: Called to prefar foo for bar\nIDemo.VoidNoArgs(); Expected #1, Actual #0.",
				() => demo.VerifyAllExpectations());
		}

		[Fact]
		public void WillDiplayDocumentationMessageIfCalledTooMuch()
		{
            demo.Expect(x => x.VoidNoArgs())
                .Message("Should be called only once");

			demo.VoidNoArgs();
			
			Assert.Throws<ExpectationViolationException>(
				@"IDemo.VoidNoArgs(); Expected #1, Actual #2.
Message: Should be called only once",
				() => demo.VoidNoArgs());
		}

		[Fact(Skip = "Test No Longer Valid")]
		public void LastMockedObjectIsNullAfterDisposingMockRepository()
		{
            //MockRepository mocks = new MockRepository();
            //mocks.ReplayAll();				
            //mocks.VerifyAll();
			
			Assert.Throws<InvalidOperationException>(
				"Invalid call, the last call has been used or no call has been made (make sure that you are calling a virtual (C#) / Overridable (VB) method).",
				() => LastCall.IgnoreArguments());
		}

		[Fact]
		public void MixOrderedAndUnorderedBehaviour()
		{
            demo.Expect(x => x.EnumNoArgs())
                .Return(EnumDemo.Dozen)
                .Repeat.Twice();

            demo.Expect(x => x.VoidStringArg("Ayende"));
            demo.Expect(x => x.VoidStringArg("Rahien"));
            demo.Expect(x => x.VoidThreeStringArgs("1", "2", "3"));
            demo.Expect(x => x.StringArgString("Hello"))
                .Return("World");

			Assert.Equal(EnumDemo.Dozen, demo.EnumNoArgs());
			Assert.Equal(EnumDemo.Dozen, demo.EnumNoArgs());
			demo.VoidStringArg("Ayende");
			demo.VoidThreeStringArgs("1", "2", "3");
			demo.VoidStringArg("Rahien");
			Assert.Equal("World", demo.StringArgString("Hello"));

            demo.VerifyAllExpectations();
		}

		[Fact]
		public void ChangingRecordersWhenReplayingDoesNotInterruptVerification()
		{
            demo.Expect(x => x.VoidStringArg("ayende"));

            demo.VoidStringArg("ayende");
            demo.VerifyAllExpectations();
		}

		[Fact(Skip = "Test No Longer Valid")]
		public void CallingReplayInOrderringThrows()
		{
			demo.VoidStringArg("ayende");
            //Assert.Throws<InvalidOperationException>(
            //    "Can't start replaying because Ordered or Unordered properties were call and not yet disposed.",
            //    () => {
            //        using (mocks.Ordered())
            //        {
            //            mocks.Replay(demo);
            //        }
            //    });
		}

		[Fact]
		public void UsingSeveralObjectAndMixingOrderAndUnorder()
		{
            IList second = MockRepository.GenerateStrictMock(typeof(IList)) as IList;

            demo.Expect(x => x.EnumNoArgs())
                .Return(EnumDemo.Dozen)
                .Repeat.Twice();

            second.Expect(x => x.Clear());

            demo.Expect(x => x.VoidStringArg("Ayende"));

            second.Expect(x => x.Count)
                .Return(3)
                .Repeat.Twice();

            demo.Expect(x => x.VoidStringArg("Rahien"));
            demo.Expect(x => x.VoidThreeStringArgs("1", "2", "3"));

            demo.Expect(x => x.StringArgString("Hello"))
                .Return("World");

            second.Expect(x => x.IndexOf(null))
                .Return(2);

			Assert.Equal(EnumDemo.Dozen, demo.EnumNoArgs());
			Assert.Equal(EnumDemo.Dozen, demo.EnumNoArgs());
			second.Clear();
			demo.VoidStringArg("Ayende");
			Assert.Equal(3, second.Count);
			demo.VoidThreeStringArgs("1", "2", "3");
			Assert.Equal(3, second.Count);
			demo.VoidStringArg("Rahien");
			Assert.Equal("World", demo.StringArgString("Hello"));
			second.IndexOf(null);


            demo.VerifyAllExpectations();
            second.VerifyAllExpectations();
		}

		[Fact]
		public void SeveralMocksUsingOrdered()
		{
            IList second = MockRepository.GenerateStrictMock(typeof(IList)) as IList;

            demo.Expect(x => x.EnumNoArgs())
                .Return(EnumDemo.Dozen)
                .Repeat.Twice();

            demo.Expect(x => x.VoidStringArg("Ayende"));

            second.Expect(x => x.Count)
                .Return(3)
                .Repeat.Twice();

            demo.Expect(x => x.VoidStringArg("Rahien"));
            demo.Expect(x => x.VoidThreeStringArgs("1", "2", "3"));
            demo.Expect(x => x.StringArgString("Hello"))
                .Return("World");

            second.Expect(x => x.IndexOf(null))
                .Return(2);

			demo.EnumNoArgs();
			Assert.Throws<ExpectationViolationException>(
				"Unordered method call! The expected call is: 'Ordered: { IDemo.EnumNoArgs(); }' but was: 'IList.Clear();'",
				() => second.Clear());
		}

		[Fact]
		public void RecursiveExpectationsOnUnordered()
		{
            demo = (IDemo)MockRepository.GenerateStrictMock(typeof(IDemo));

            demo.Expect(x => x.VoidNoArgs())
                .Callback(new DelegateDefinations.NoArgsDelegate(CallMethodOnDemo));

            demo.Expect(x => x.VoidStringArg("Ayende"));
			
			demo.VoidNoArgs();
            demo.VerifyAllExpectations();
		}

        [Fact(Skip = "Test No Longer Valid (Ordering Removed)")]
		public void RecursiveExpectationsOnOrdered()
		{
            demo = (IDemo)MockRepository.GenerateStrictMock(typeof(IDemo));

            demo.Expect(x => x.VoidNoArgs())
                .Callback(CallMethodOnDemo);

            demo.Expect(x => x.VoidStringArg("Ayende"));

			Assert.Throws<ExpectationViolationException>(
				"Unordered method call! The expected call is: 'Ordered: { IDemo.VoidNoArgs(callback method: RhinoMockTests.CallMethodOnDemo); }' but was: 'IDemo.VoidStringArg(\"Ayende\");'",
				() => demo.VoidNoArgs());
		}
        
		[Fact]
		public void GetArgsOfEpectedAndActualMethodCallOnException()
		{
            demo = (IDemo)MockRepository.GenerateStrictMock(typeof(IDemo));

            demo.Expect(x => x.VoidThreeStringArgs("a", "b", "c"));

			Assert.Throws<ExpectationViolationException>(
				"IDemo.VoidThreeStringArgs(\"c\", \"b\", \"a\"); Expected #0, Actual #1.\r\nIDemo.VoidThreeStringArgs(\"a\", \"b\", \"c\"); Expected #1, Actual #0.",
				() => demo.VoidThreeStringArgs("c", "b", "a"));
		}
        
		[Fact(Skip = "Test No Longer Valid (Ordering Removed)")]
		public void SteppingFromInnerOrderringToOuterWithoutFullifingAllOrderringInInnerThrows()
		{
			demo = (IDemo) MockRepository.GenerateStrictMock(typeof (IDemo));

            demo.Expect(x => x.VoidThreeStringArgs("", "", ""));
            demo.Expect(x => x.VoidNoArgs());
            demo.Expect(x => x.VoidStringArg("Ayende"));

			demo.VoidNoArgs();

			Assert.Throws<ExpectationViolationException>(
				"Unordered method call! The expected call is: 'Ordered: { IDemo.VoidStringArg(\"Ayende\"); }' but was: 'IDemo.VoidThreeStringArgs(\"\", \"\", \"\");'",
				() => demo.VoidThreeStringArgs("", "", ""));
		}

		[Fact]
		public void Overrideing_ToString()
		{
			ObjectThatOverrideToString oid = (ObjectThatOverrideToString)
				MockRepository.GenerateStrictMock(typeof (ObjectThatOverrideToString));

            oid.Expect(x => x.ToString())
                .Return("bla");

			Assert.Equal("bla", oid.ToString());

            oid.VerifyAllExpectations();
		}

		[Fact]
		public void CallbackThatThrows()
		{
			demo = (IDemo) MockRepository.GenerateStrictMock(typeof (IDemo));

            demo.Expect(x => x.VoidNoArgs())
                .Callback(new DelegateDefinations.NoArgsDelegate(ThrowFromCallback));

			Assert.Throws<AddressAlreadyInUseException>(demo.VoidNoArgs);
		}

		private bool CallMethodOnDemo()
		{
			demo.VoidStringArg("Ayende");
			return true;
		}

		private bool ThrowFromCallback()
		{
			throw new AddressAlreadyInUseException();
		}

        private static void RecordOrdered(IDemo demo)
        {
            demo.Expect(x => x.ReturnStringNoArgs())
                .Return(null);

            demo.Expect(x => x.VoidNoArgs())
                .Repeat.Twice();

            demo.Expect(x => x.VoidStringArg("Hello"));
            demo.Expect(x => x.VoidStringArg("World"));
        }

		public class ObjectThatOverrideToString
		{
			public override string ToString()
			{
				return base.ToString ();
			}
		}
	}
}