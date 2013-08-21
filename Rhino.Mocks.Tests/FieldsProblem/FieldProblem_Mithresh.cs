using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_Mithresh
	{
		[Fact]
		public void TestOutMethod()
		{
            int intTest = 0;

            ITest mockProxy = Repository.Mock<ITest>();

            mockProxy.Expect(x => x.Addnumber(out Arg<int>.Out(4).Dummy));

			mockProxy.Addnumber(out intTest);

			Assert.Equal(4, intTest);
		}

		public interface ITest
		{
			void Addnumber(out int Num);
		}
	}
}