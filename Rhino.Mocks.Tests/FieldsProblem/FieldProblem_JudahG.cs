using System;
using Xunit;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_JudahG
	{
		public interface IView
		{
			int? Foo { get; set; }
		}

		[Fact]
		public void IsMatching()
		{
            Predicate<int> alwaysReturnsTrue = delegate(int input)
            {
                return true;
            };

			IView view = MockRepository.GenerateStrictMock<IView>();

            view.Expect(x => x.Foo = null)
                .Constraints(Is.Matching(alwaysReturnsTrue));

            view.Foo = 1;
            view.VerifyAllExpectations();
		}
	}
}