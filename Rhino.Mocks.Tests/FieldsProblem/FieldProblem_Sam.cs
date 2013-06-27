
using Rhino.Mocks.Exceptions;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_Sam
	{
		[Fact]
		public void Test()
		{
            SimpleOperations myMock = MockRepository.GenerateStrictMock<SimpleOperations>();

            myMock.Expect(x => x.AddTwoValues(1, 2))
                .Return(3);

			Assert.Equal(3, myMock.AddTwoValues(1, 2));
            myMock.VerifyAllExpectations();
		}

		[Fact]
		public void WillRememberExceptionInsideOrderRecorderEvenIfInsideCatchBlock()
		{
			IInterfaceWithThreeMethods interfaceWithThreeMethods = MockRepository
                .GenerateStrictMock<IInterfaceWithThreeMethods>();

            interfaceWithThreeMethods.Expect(x => x.A());
            interfaceWithThreeMethods.Expect(x => x.C());

			interfaceWithThreeMethods.A();
			try
			{
				interfaceWithThreeMethods.B();
			}
			catch { /* valid for code under test to catch all */ }

			interfaceWithThreeMethods.C();

            Assert.Throws<ExpectationViolationException>(
                "Unordered method call! The expected call is: 'Ordered: { IInterfaceWithThreeMethods.C(); }' but was: 'IInterfaceWithThreeMethods.B();'",
                () => interfaceWithThreeMethods.VerifyAllExpectations());
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