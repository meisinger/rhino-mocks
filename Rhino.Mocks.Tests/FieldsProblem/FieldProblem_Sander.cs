using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_Sander
    {
        [Fact]
        public void CanUseOutIntPtr()
        {
            IntPtr parameter;

            IFooWithOutIntPtr mock = MockRepository.Mock<IFooWithOutIntPtr>();
            mock.Expect(x => x.GetBar(out Arg<IntPtr>.Out(new IntPtr(3)).Dummy))
                .IgnoreArguments()
                .Return(5);
            
            Assert.Equal(5, mock.GetBar(out parameter));
            Assert.Equal(new IntPtr(3), parameter);
            mock.VerifyAllExpectations();
        }
    }

    public interface IFooWithOutIntPtr
    {
        int GetBar(out IntPtr parameter);
    }
}