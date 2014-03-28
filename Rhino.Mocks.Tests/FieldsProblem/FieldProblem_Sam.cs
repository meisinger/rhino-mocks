
using Rhino.Mocks.Exceptions;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_Sam
	{
		[Fact]
		public void Test()
		{
            SimpleOperations myMock = MockRepository.Partial<SimpleOperations>();

            myMock.Expect(x => x.AddTwoValues(1, 2))
                .Return(3);

			Assert.Equal(3, myMock.AddTwoValues(1, 2));
            myMock.VerifyExpectations();
		}

		[Fact]
		public void WillRememberExceptionInsideOrderRecorderEvenIfInsideCatchBlock()
		{
            IInterfaceWithThreeMethods interfaceWithThreeMethods = MockRepository.Mock<IInterfaceWithThreeMethods>();

            interfaceWithThreeMethods.Expect(x => x.A());
            interfaceWithThreeMethods.Expect(x => x.C());

			interfaceWithThreeMethods.A();
            interfaceWithThreeMethods.B();
            interfaceWithThreeMethods.C();

			Assert.Throws<ExpectationViolationException>(() => 
                interfaceWithThreeMethods.VerifyExpectations(true));
		}
	}

	public interface IInterfaceWithThreeMethods
	{
		void A();
		void B();
		void C();
	}

	public class SimpleOperations
	{
		public virtual int AddTwoValues(int x, int y)
		{
			return x + y;
		}
	}
}