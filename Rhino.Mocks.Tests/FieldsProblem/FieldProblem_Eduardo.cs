
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_Eduardo
	{
		[Fact]
		public void CanSetExpectationOnReadWritePropertyUsingRecordPlaybackSyntax()
		{
			var demo = MockRepository.Mock<IDemo>();

            demo.Expect(x => x.Prop = "Eduardo");
            
            demo.Prop = "Eduardo";
            demo.VerifyAllExpectations();
		}

		[Fact]
		public void CanSetExpectationOnReadWritePropertyUsingAAASyntax()
		{
			var demo = MockRepository.Mock<IDemo>();

            demo.Expect(x => x.Prop = "Eduardo");
            
			demo.Prop = "Eduardo";
			demo.VerifyAllExpectations();
		}
	}
}