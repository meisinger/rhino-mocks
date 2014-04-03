using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_SO_user815512
    {
        public interface IFuncTest
        {
            Func<int> Generate(int num);
        }

        [Fact]
        public void Can_Create_Two_Expectations_With_No_Exception()
        {
            var myMock = MockRepository.Mock<IFuncTest>();

            myMock.Expect(x => x.Generate(42))
                .Return(() => 6);

            myMock.Expect(x => x.Generate(4000))
                .Return(() => 9);

            Assert.Equal(6, myMock.Generate(42)());
            Assert.Equal(9, myMock.Generate(4000)());

            myMock.VerifyExpectations(true);
        }
    }
}
