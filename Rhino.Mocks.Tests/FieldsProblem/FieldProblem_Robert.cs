using Xunit;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_Robert
	{
		public interface IView
		{
			void RedrawDisplay(object something);
		}

		[Fact]
		public void CorrectResultForExpectedWhenUsingTimes()
		{
            IView view = MockRepository.GenerateStrictMock<IView>();

            view.Expect(x => x.RedrawDisplay(null))
                .IgnoreArguments()
                .Repeat.Times(4);

            Assert.Throws<ExpectationViolationException>(
                "IView.RedrawDisplay(\"blah\"); Expected #4, Actual #5.",
                () =>
                {
                    for (int i = 0; i < 5; i++)
                        view.RedrawDisplay("blah");
                });
		}

		[Fact]
		public void CorrectResultForExpectedWhenUsingTimesWithRange()
		{
			IView view = MockRepository.GenerateStrictMock<IView>();

            view.Expect(x => x.RedrawDisplay(null))
                .IgnoreArguments()
                .Repeat.Times(3, 4);

            Assert.Throws<ExpectationViolationException>(
                "IView.RedrawDisplay(\"blah\"); Expected #3 - 4, Actual #5.",
                () =>
                {
                    for (int i = 0; i < 5; i++)
                        view.RedrawDisplay("blah");
                });
		}
	}
}