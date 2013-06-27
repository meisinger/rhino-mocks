using System;
using System.Web.Security;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_StevenS : IDisposable
	{
		private MembershipProvider myMembershipProvider;

		public FieldProblem_StevenS()
		{
            myMembershipProvider = MockRepository.GenerateStrictMock<MembershipProvider>();
		}

        public void Dispose()
        {
            myMembershipProvider.VerifyAllExpectations();
        }

        [Fact]
        public void LoadFromUserId()
        {
            myMembershipProvider.Expect(x => x.Name)
                .Repeat.Any()
                .Return("Foo");

            myMembershipProvider.Expect(x => x.GetUser("foo", false))
                .Return(null);

            myMembershipProvider.GetUser("foo", false);
        }

		[Fact]
		public void LoadFromUserId_Object()
		{
            myMembershipProvider.Expect(x => x.Name)
                .Repeat.Any()
                .Return("Foo");

            object foo = "foo";
            myMembershipProvider.Expect(x => x.GetUser(foo, false))
                .Return(null);

			myMembershipProvider.GetUser(foo, false);
            
		}
	}
}