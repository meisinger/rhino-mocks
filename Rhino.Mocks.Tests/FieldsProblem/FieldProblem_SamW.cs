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

            ITest test = MockRepository.GenerateStrictMock<ITest>();
            test.Expect(x => x.ArrayWithOut(new string[] { "data" }, out b))
                .OutRef("SuccessWithOut2")
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
