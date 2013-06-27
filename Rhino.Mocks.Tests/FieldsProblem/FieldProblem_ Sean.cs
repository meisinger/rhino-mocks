using System.Security.Permissions;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem__Sean
	{
        //[Fact(Skip = "Not sure what the problem is, and don't know enough about CAS to try to figure it out")]
        [Fact]
		public void CanMockMethodWithEnvironmentPermissions()
		{
			IEmployeeRepository employeeRepository = MockRepository.GenerateStrictMock<IEmployeeRepository>();
			IEmployee employee = MockRepository.GenerateStrictMock<IEmployee>();

            employeeRepository.Expect(x => x.GetEmployeeDetails("ayende"))
                .Return(employee);

			IEmployee actual = employeeRepository.GetEmployeeDetails("ayende");
			Assert.Equal(employee, actual);
		}
	}

	public interface IEmployeeRepository
	{
		[EnvironmentPermission(SecurityAction.LinkDemand)]
		IEmployee GetEmployeeDetails(string username);
	}

	public interface IEmployee
	{
	}
}
