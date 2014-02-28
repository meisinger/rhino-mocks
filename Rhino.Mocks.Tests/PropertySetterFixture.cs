using Xunit;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	public class PropertySetterFixture
	{
		[Fact]
		public void Setter_Expectation_With_Custom_Ignore_Arguments()
		{
            IBar bar = Repository.Mock<IBar>();
            bar.ExpectProperty(x => x.Foo = Arg<int>.Is.Anything);

            bar.Foo = 2;
            bar.VerifyAllExpectations();
		}

		[Fact]
		public void Setter_Expectation_Not_Fullfilled()
		{
            IBar bar = Repository.Mock<IBar>();
            bar.ExpectProperty(x => x.Foo = Arg<int>.Is.Anything);

            Assert.Throws<ExpectationViolationException>(
                "IBar.set_Foo(any); Expected #1, Actual #0.",
                () => bar.VerifyAllExpectations());
		}

		[Fact]
		public void Setter_Expectation_With_Correct_Argument()
		{
            IBar bar = Repository.Mock<IBar>();
            bar.ExpectProperty(x => x.Foo = Arg<int>.Is.Anything);

			bar.Foo = 1;
            bar.VerifyAllExpectations();
		}

		[Fact]
		public void Setter_Expectation_With_Wrong_Argument()
		{
            IBar bar = Repository.Mock<IBar>();
            bar.ExpectProperty(x => x.Foo = 1);

            bar.Foo = 0;

            Assert.Throws<ExpectationViolationException>(
                "IBar.set_Foo(0); Expected #0, Actual #1.\r\nIBar.set_Foo(1); Expected #1, Actual #0.",
                () => bar.VerifyExpectations(true));
		}
	}

	public interface IBar
	{
		int Foo { get; set; }
	}
}