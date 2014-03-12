using System.Reflection;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_Oliver
	{
		public interface ITestGen<T>
		{
			int Foo { get; set; }
		}

		public interface ITestNormal
		{
			int Foo { get; set;}
		}

		[Fact]
		public void StubGenericInterface_CanReadWriteProperties()
		{
            ITestGen<int> test = Repository.Mock<ITestGen<int>>();

			test.Foo = 10;
			Assert.Equal(10, test.Foo);

            test.VerifyExpectations();
		}

		[Fact]
		public void StubInterface_CanReadWriteProperties()
		{
			ITestNormal test = Repository.Mock<ITestNormal>();

			test.Foo = 10;
			Assert.Equal(10, test.Foo);

            test.VerifyExpectations();
		}

		[Fact]
		public void MockGenericInterface_CanSetProperties()
		{
			ITestGen<int> test = Repository.Mock<ITestGen<int>>();

            test.ExpectProperty(x => x.Foo);

			test.Foo = 10;
			Assert.Equal(10, test.Foo);

            test.VerifyExpectations();
		}

		[Fact]
		public void MockNormalInterface_CanSetProperties()
		{
			ITestNormal test = Repository.Mock<ITestNormal>();

            test.ExpectProperty(x => x.Foo);

			test.Foo = 10;
			Assert.Equal(10, test.Foo);

            test.VerifyExpectations();
		}
	}
}
