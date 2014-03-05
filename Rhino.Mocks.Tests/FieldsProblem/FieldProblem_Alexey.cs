using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System.Data.SqlClient;
	using Exceptions;
	using Xunit;

	public interface ITestInterface
	{
		ITestInterface AddService<TService, TComponent>() where TComponent : TService;
	}

	public class TestInterface : MarshalByRefObject
	{
		public virtual TestInterface AddService<TService, TComponent>() where TComponent : TService
		{
			return this;
		}
	}

	public class FieldProblem_Alexey
	{
		[Fact]
		public void MockInterfaceWithGenericMethodWithConstraints()
		{
			ITestInterface mockObj = Repository.MockWithRemoting<ITestInterface>();

            mockObj.Expect(x => x.AddService<IDisposable, SqlConnection>())
                .Return(mockObj);

			mockObj.AddService<IDisposable, SqlConnection>();
            mockObj.VerifyAllExpectations();
		}

		[Fact]
		public void MockInterfaceWithGenericMethodWithConstraints_WhenNotValid()
		{
            ITestInterface mockObj = Repository.MockWithRemoting<ITestInterface>();

            mockObj.Expect(x => x.AddService<IDisposable, SqlConnection>())
                .Return(mockObj);

			Assert.Throws<ExpectationViolationException>(
				"ITestInterface.AddService<System.IDisposable, System.Data.SqlClient.SqlConnection>(); Expected #1, Actual #0.",
				() => mockObj.VerifyAllExpectations());
		}

		[Fact]
		public void MockInterfaceWithGenericMethodWithConstraints_WhenNotValid_UsingDynamicMock()
		{
            ITestInterface mockObj = Repository.MockWithRemoting<ITestInterface>();

            mockObj.Expect(x => x.AddService<IDisposable, SqlConnection>())
                .Return(mockObj);

			Assert.Throws<ExpectationViolationException>(
				"ITestInterface.AddService<System.IDisposable, System.Data.SqlClient.SqlConnection>(); Expected #1, Actual #0.",
				() => mockObj.VerifyAllExpectations());
		}

		[Fact]
		public void MockInterfaceWithGenericMethodWithConstraints_UsingDynamicMock()
		{
            ITestInterface mockObj = Repository.MockWithRemoting<ITestInterface>();
			
			mockObj.AddService<IDisposable, SqlConnection>();
            mockObj.VerifyAllExpectations();
		}
	}
}
