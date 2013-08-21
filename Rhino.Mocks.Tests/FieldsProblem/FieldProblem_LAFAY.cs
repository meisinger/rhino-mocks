using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_LAFAY
    {
        private IDemo demo;
        
		public FieldProblem_LAFAY()
        {
            demo = Repository.Mock<IDemo>();
        }

        [Fact]
        public void ExpectTwoCallsReturningMarshalByRef()
        {
            MarshalByRefToReturn res1 = new MarshalByRefToReturn();
            MarshalByRefToReturn res2 = new MarshalByRefToReturn();

            demo.Expect(x => x.ReturnMarshalByRefNoArgs())
                .Return(res1);

            demo.Expect(x => x.ReturnMarshalByRefNoArgs())
                .Return(res2);

            demo.ReturnMarshalByRefNoArgs();
            demo.ReturnMarshalByRefNoArgs();
        }

        public interface IDemo
        {
            MarshalByRefToReturn ReturnMarshalByRefNoArgs();
        }

        public class MarshalByRefToReturn : MarshalByRefObject
        {
            public override string ToString()
            {
                return "test";
            }
        }
    }
}