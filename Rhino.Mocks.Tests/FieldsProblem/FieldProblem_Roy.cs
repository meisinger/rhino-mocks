using System;
using Xunit;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_Roy
    {
        [Fact]
        public void StubNeverFailsTheTest()
        {
            IGetResults resultGetter = MockRepository.Mock<IGetResults>();

            resultGetter.Stub(x => x.GetSomeNumber("a"))
                .Return(1);

            int result = resultGetter.GetSomeNumber("b");

            Assert.Equal(0, result);
            resultGetter.VerifyExpectations(); //<- should not fail the test methinks
        }

        [Fact]
        public void CanGetSetupResultFromStub()
        {
            IGetResults resultGetter = MockRepository.Mock<IGetResults>();

            resultGetter.Stub(x => x.GetSomeNumber("a"))
                .Return(1);

            int result = resultGetter.GetSomeNumber("a");

            Assert.Equal(1, result);
            resultGetter.VerifyExpectations();
        }
    }

    public interface IGetResults
    {
        int GetSomeNumber(string s);
    }
}