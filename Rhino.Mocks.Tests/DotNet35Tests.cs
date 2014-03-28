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
            IDemo demo = MockRepository.Mock<IDemo>();

            demo.Expect(x => x.VoidNoArgs());
            demo.VoidNoArgs();

            demo.VerifyExpectations();
		}

		[Fact]
		public void NaturalSyntaxForCallingMethods_WithArguments()
		{
            IDemo demo = MockRepository.Mock<IDemo>();

            demo.Expect(x => x.VoidStringArg("blah"));
            demo.VoidStringArg("blah");

            demo.VerifyExpectations();
		}

		[Fact]
		public void NaturalSyntaxForCallingMethods_WithArguments_WhenNotCalled_WouldFailVerification()
		{
            IDemo demo = MockRepository.Mock<IDemo>();

			demo.Expect(x => x.VoidStringArg("blah"));
			
            Assert.Throws<ExpectationViolationException>(
                () => demo.VerifyExpectations());
		}

		[Fact]
		public void NaturalSyntaxForCallingMethods_WithArguments_WhenCalledWithDifferentArgument()
		{
            IDemo demo = MockRepository.Mock<IDemo>();
			
            demo.Expect(x => x.VoidStringArg("blah"));
            demo.VoidStringArg("arg");
            
            Assert.Throws<ExpectationViolationException>(
               () => demo.VerifyExpectations(true));
		}

		[Fact]
		public void CanCallMethodWithParameters_WithoutSpecifyingParameters_WillAcceptAnyParameter()
		{
            IDemo demo = MockRepository.Mock<IDemo>();
			
            demo.Expect(x => x.VoidStringArg("blah"))
                .IgnoreArguments();
			
            demo.VoidStringArg("asd");

            demo.VerifyExpectations();
		}
	}
}