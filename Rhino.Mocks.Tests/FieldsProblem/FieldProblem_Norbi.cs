using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_Norbi
    {
        public interface IFuncTest<T>
        {
            Func<T> Get(string text);
        }

        public interface IParam
        {
            string Text { get; }
        }

        public class Param : IParam
        {
            public string Text { get; set; }
        }

        [Fact]
        public void Can_Create_Two_Expectations_Against_Func()
        {
            var mock = MockRepository.Mock<IFuncTest<IParam>>();
            mock.Expect(x => x.Get("Test1"))
                .Return(() => new Param { Text = "ParamWithText1" });

            mock.Expect(x => x.Get("Test2"))
                .Return(() => new Param { Text = "ParamWithText2" });

            Assert.Equal("ParamWithText1", mock.Get("Test1")().Text);
            Assert.Equal("ParamWithText2", mock.Get("Test2")().Text);

            mock.VerifyExpectations();
        }
    }
}
