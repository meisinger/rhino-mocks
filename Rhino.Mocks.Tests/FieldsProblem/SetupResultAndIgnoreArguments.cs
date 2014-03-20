
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class SetupResultAndIgnoreArguments
	{
		[Fact]
		public void CanUseSetupResultAndIgnoreArguments_WhenUsingUnorderedBlock()
		{
            IFetcher fetcher = Repository.Mock<IFetcher>();

            fetcher.Expect(x => x.GetUsersWithCriteriaLike(null))
                .IgnoreArguments()
                .Return(new Student[] { new Student(), new Student() });

			Assert.Equal(2, fetcher.GetUsersWithCriteriaLike("foo").Length);
            fetcher.VerifyAllExpectations();
		}

		[Fact]
		public void CanUseSetupResultAndIgnoreArguments_WhenUsingOrderedBlock()
		{
			IFetcher fetcher = Repository.Mock<IFetcher>();

            fetcher.Expect(x => x.GetUsersWithCriteriaLike(null))
                .IgnoreArguments()
                .Return(new Student[] { new Student(), new Student() });

			Assert.Equal(2, fetcher.GetUsersWithCriteriaLike("foo").Length);
            fetcher.VerifyAllExpectations();
		}
	}

	public interface IFetcher
	{
		Student[] GetUsersWithCriteriaLike(string likeString);
	}

	public class Student
	{
	}
}