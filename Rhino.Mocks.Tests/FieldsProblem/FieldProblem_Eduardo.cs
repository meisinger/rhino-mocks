
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_Eduardo
	{
		[Fact]
		public void CanSetExpectationOnReadWritePropertyUsingRecordPlaybackSyntax()
		{
			var demo = MockRepository.GenerateDynamicMock<IDemo>();

			demo.Expect(x => x.Prop)
                .SetPropertyWithArgument("Eduardo");

            demo.Prop = "Eduardo";
            demo.VerifyAllExpectations();
		}

		[Fact]
		public void CanSetExpectationOnReadWritePropertyUsingAAASyntax()
		{
			var demo = MockRepository.GenerateMock<IDemo>();

			demo.Expect(x => x.Prop)
                .SetPropertyWithArgument("Eduardo");

			demo.Prop = "Eduardo";
			demo.VerifyAllExpectations();
		}
	}
}