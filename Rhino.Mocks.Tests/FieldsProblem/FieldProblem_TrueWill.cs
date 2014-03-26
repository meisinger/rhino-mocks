
using System;
using Xunit;
using Rhino.Mocks;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_TrueWill
    {
        [Fact]
        public void ReadWritePropertyBug1()
        {
            ISomeThing thing = Repository.Mock<ISomeThing>();
            thing.Number = 21;

            thing.Stub(x => x.Name)
                .Return("Bob");

            Assert.Equal(thing.Number, 21);
            // Fails - calling Stub on anything after
            // setting property resets property to default.
        }

        [Fact(Skip = "Test No Longer Valid")]
        public void ReadWritePropertyBug2()
        {
            ISomeThing thing = Repository.Mock<ISomeThing>();
        	Assert.Throws<InvalidOperationException>(
        		() => thing.Stub(x => x.Number).Return(21));
            // InvalidOperationException :
            // Invalid call, the last call has been used...
            // This broke a test on a real project when a
            // { get; } property was changed to { get; set; }.
        }
    }

    public interface ISomeThing
    {
        string Name { get; }

        int Number { get; set; }
    }

}