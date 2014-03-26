namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System;
	using System.Data;
	using Exceptions;
	using Xunit;
	using Rhino.Mocks;

	public class FieldProblem_Andrew
	{
		[Fact]
		public void Will_get_unexpect_error()
		{
            var stubConnection = Repository.Mock<IDbConnection>();
            var mockCommand = Repository.Mock<IDbCommand>();
			mockCommand.Expect(c => c.Connection = stubConnection);
			mockCommand.Expect(c => c.Connection = null);
			mockCommand.Stub(c => c.ExecuteNonQuery())
                .Throws<TestException>();

			var executor = new Executor(stubConnection);
			try
			{
				executor.ExecuteNonQuery(mockCommand);
				Assert.False(true, "exception was expected");
			}
			catch (TestException)
			{
			}

			Assert.Throws<ExpectationViolationException>(
                () => mockCommand.VerifyAllExpectations());
		}
	}

	public class TestException : Exception
	{
	}


	public class Executor
	{
		private IDbConnection _connection;

		public Executor(IDbConnection connection)
		{
			this._connection = connection;
		}

		public int ExecuteNonQuery(IDbCommand command)
		{
			try
			{
				command.Connection = this._connection;
				return command.ExecuteNonQuery();
			}
			finally
			{
				//command.Connection = null;
				if (this._connection.State != ConnectionState.Closed) this._connection.Close();
			}
		}
	}
}