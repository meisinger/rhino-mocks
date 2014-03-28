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
using System.Data;
using Xunit;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Helpers;

namespace Rhino.Mocks.Tests.Constraints
{
	public class ConstraintTests
	{
		private IDemo demo;
		
		public ConstraintTests()
		{
			demo = MockRepository.Mock<IDemo>();
		}

        [Fact]
		public void UsingPredicate()
		{
            demo.Expect(x => x.VoidStringArg(Arg<string>.Matches(s => (s.Length == 2 && s.EndsWith("b")))));

			demo.VoidStringArg("ab");
            demo.VerifyExpectations();
		}

        //[Fact]
        //public void UsingPredicateConstraintWhenTypesNotMatching()
        //{
        //    demo.Expect(x => x.VoidStringArg(null))
        //        .Constraints(Is.Matching<DataSet>(delegate(DataSet s) { return false; }));
			
        //    Assert.Throws<InvalidOperationException>(
        //        "Predicate accept System.Data.DataSet but parameter is System.String which is not compatible",
        //        () => demo.VoidStringArg("ab"));
        //}

        [Fact]
        public void UsingPredicateConstraintWithSubtype()
        {
            demo.Expect(x => x.VoidStringArg(Arg<string>.Matches(Is.Matching<object>(delegate(object o) { return o.Equals("ab"); }))));
            
            demo.VoidStringArg("ab");
            demo.VerifyExpectations();
        }

		[Fact]
		public void UsingPredicateWhenExpectationViolated()
		{
            demo.Expect(x => x.VoidStringArg(Arg<string>.Matches(Is.Matching<string>(JustPredicate))));
            demo.VoidStringArg("cc");

			Assert.Throws<ExpectationViolationException>(
				() => demo.VerifyExpectations(true));
		}
		
		public bool JustPredicate(string s)
		{
			return false;
		}

        [Fact]
        public void AndSeveralConstraings()
        {
            AbstractConstraint all = Is.NotEqual("bar") & Is.TypeOf(typeof(string)) & Is.NotNull();
            Assert.True(all.Eval("foo"));
            Assert.Equal("not equal to bar and type of {System.String} and not equal to null", all.Message);
        }

		 [Fact]
        public void AndSeveralConstraings_WithGenerics()
        {
            AbstractConstraint all = Is.NotEqual("bar") && Is.TypeOf<string>() && Is.NotNull();
            Assert.True(all.Eval("foo"));
            Assert.Equal("not equal to bar and type of {System.String} and not equal to null", all.Message);
        }

		[Fact]
		public void AndConstraints()
		{
			AbstractConstraint start = Text.StartsWith("Ayende"), end = Text.EndsWith("Rahien");
			AbstractConstraint combine = start & end;
			Assert.True(combine.Eval("Ayende Rahien"));
			Assert.Equal("starts with \"Ayende\" and ends with \"Rahien\"", combine.Message);
		}

		[Fact]
		public void NotConstraint()
		{
			AbstractConstraint start = Text.StartsWith("Ayende");
			AbstractConstraint negate = !start;
			Assert.True(negate.Eval("Rahien"));
			Assert.Equal("not starts with \"Ayende\"", negate.Message);
		}

		[Fact]
		public void OrConstraints()
		{
			AbstractConstraint start = Text.StartsWith("Ayende"), end = Text.EndsWith("Rahien");
			AbstractConstraint combine = start | end;
			Assert.True(combine.Eval("Ayende"));
			Assert.True(combine.Eval("Rahien"));
			Assert.Equal("starts with \"Ayende\" or ends with \"Rahien\"", combine.Message);
		}

		[Fact]
		public void SettingConstraintOnAMock()
		{
            demo.Expect(x => x.VoidStringArg(Arg.Text.Contains("World")));
			
			demo.VoidStringArg("Hello, World");
            demo.VerifyExpectations();
		}

		[Fact]
		public void ConstraintFailingThrows()
		{
            demo.Expect(x => x.VoidStringArg(Arg.Text.Contains("World")));
            demo.VoidStringArg("Hello, world");

			Assert.Throws<ExpectationViolationException>(
				() => demo.VerifyExpectations(true));
		}

        [Fact]
		public void ConstraintsThatWerentCallCauseVerifyFailure()
		{
            demo.Expect(x => x.VoidStringArg(Arg.Text.Contains("World")));

            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations());
		}
	}
}
