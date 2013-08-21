using System;
using Xunit;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	public class DotNet35Tests
	{
		[Fact]
		public void NaturalSyntaxForCallingMethods()
		{
            IDemo demo = Repository.Mock<IDemo>();

            demo.Expect(x => x.VoidNoArgs());
            demo.VoidNoArgs();

            demo.VerifyExpectations();
		}

		[Fact]
		public void NaturalSyntaxForCallingMethods_WithArguments()
		{
            IDemo demo = Repository.Mock<IDemo>();

            demo.Expect(x => x.VoidStringArg("blah"));
            demo.VoidStringArg("blah");

            demo.VerifyExpectations();
		}

		[Fact]
		public void NaturalSyntaxForCallingMethods_WithArguments_WhenNotCalled_WouldFailVerification()
		{
            IDemo demo = Repository.Mock<IDemo>();

			demo.Expect(x => x.VoidStringArg("blah"));
			
            Assert.Throws<ExpectationViolationException>(
                "IDemo.VoidStringArg(\"blah\"); Expected #1, Actual #0.",
                () => demo.VerifyExpectations());
		}

		[Fact]
		public void NaturalSyntaxForCallingMethods_WithArguments_WhenCalledWithDifferentArgument()
		{
            IDemo demo = Repository.Mock<IDemo>();
			
            demo.Expect(x => x.VoidStringArg("blah"));
            demo.VoidStringArg("arg");
            
            Assert.Throws<ExpectationViolationException>(
                @"IDemo.VoidStringArg(""arg""); Expected #0, Actual #1.
IDemo.VoidStringArg(""blah""); Expected #1, Actual #0.",
               () => demo.VerifyExpectations(true));
		}

		[Fact]
		public void CanCallMethodWithParameters_WithoutSpecifyingParameters_WillAcceptAnyParameter()
		{
            IDemo demo = Repository.Mock<IDemo>();
			
            demo.Expect(x => x.VoidStringArg("blah"))
                .IgnoreArguments();
			
            demo.VoidStringArg("asd");

            demo.VerifyExpectations();
		}
	}
}