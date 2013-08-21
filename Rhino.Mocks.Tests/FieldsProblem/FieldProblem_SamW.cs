using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_SamW
    {
        [Fact]
        public void UsingArrayAndOutParam()
        {
            string b;

            ITest test = Repository.Mock<ITest>();
            test.Expect(x => x.ArrayWithOut(Arg<string[]>.List.IsIn("data"), out Arg<string>.Out("SuccessWithOut2").Dummy))
                .Return("SuccessWithOut1");

            Console.WriteLine(test.ArrayWithOut(new string[] { "data" }, out b));
            Console.WriteLine(b);
        }


        public interface ITest
        {
            string ArrayWithOut(string[] a, out string b);
        }
    }
}
