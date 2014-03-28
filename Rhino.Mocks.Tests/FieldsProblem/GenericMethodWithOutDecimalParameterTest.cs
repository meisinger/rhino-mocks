
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class GenericMethodWithOutDecimalParameterTest
	{
		public interface IMyInterface
		{
			void GenericMethod<T>(out T parameter);
		}

		[Fact]
		public void GenericMethodWithOutDecimalParameter()
		{
            IMyInterface mock = MockRepository.Mock<IMyInterface>();

			decimal expectedOutParameter = 1.234M;
            //decimal emptyOutParameter;

            mock.Expect(x => x.GenericMethod(out Arg<decimal>.Out(expectedOutParameter).Dummy));

            decimal outParameter;
            mock.GenericMethod(out outParameter);
            Assert.Equal(expectedOutParameter, outParameter);
		}

		public static void Foo(out decimal d)
		{
			d = 1.234M;
		}

		public static void Foo(out int d)
		{
			d = 1;
		}
	}
}