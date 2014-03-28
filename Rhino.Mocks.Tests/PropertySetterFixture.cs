using Xunit;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	public class PropertySetterFixture
	{
		[Fact]
		public void Setter_Expectation_With_Custom_Ignore_Arguments()
		{
            IBar bar = MockRepository.Mock<IBar>();
            bar.ExpectProperty(x => x.Foo = Arg<int>.Is.Anything);

            bar.Foo = 2;
            bar.VerifyAllExpectations();
		}

		[Fact]
		public void Setter_Expectation_Not_Fullfilled()
		{
            IBar bar = MockRepository.Mock<IBar>();
            bar.ExpectProperty(x => x.Foo = Arg<int>.Is.Anything);

            Assert.Throws<ExpectationViolationException>(
                () => bar.VerifyAllExpectations());
		}

		[Fact]
		public void Setter_Expectation_With_Correct_Argument()
		{
            IBar bar = MockRepository.Mock<IBar>();
            bar.ExpectProperty(x => x.Foo = Arg<int>.Is.Anything);

			bar.Foo = 1;
            bar.VerifyAllExpectations();
		}

		[Fact]
		public void Setter_Expectation_With_Wrong_Argument()
		{
            IBar bar = MockRepository.Mock<IBar>();
            bar.ExpectProperty(x => x.Foo = 1);

            bar.Foo = 0;

            Assert.Throws<ExpectationViolationException>(
                () => bar.VerifyExpectations(true));
		}
	}

	public interface IBar
	{
		int Foo { get; set; }
	}
}