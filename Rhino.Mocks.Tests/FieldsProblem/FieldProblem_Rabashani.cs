using Xunit;
using Rhino.Mocks.Tests.Model;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_Rabashani
	{
		[Fact]
		public void CanMockInternalInterface()
		{
            IInternal mock = MockRepository.Mock<IInternal>();
            mock.Expect(x => x.Foo());

			mock.Foo();
            mock.VerifyExpectations();
		}

		[Fact]
		public void CanMockInternalClass()
		{
            Internal mock = MockRepository.Partial<Internal>();

            mock.Expect(x => x.Bar())
                .Return("blah");

			Assert.Equal("blah", mock.Bar());
            mock.VerifyExpectations();
		}

		[Fact]
		public void CanPartialMockInternalClass()
		{
            Internal mock = MockRepository.Partial<Internal>();

            mock.Expect(x => x.Foo())
                .Return("blah");

			Assert.Equal("blah", mock.Foo());
			Assert.Equal("abc", mock.Bar());

            mock.VerifyExpectations();
		}
	}
}
