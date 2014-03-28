using System;
using System.Collections.Generic;
using System.Text;
using ADODB;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_dyowee
	{
		[Fact]
		public void MockingRecordSet()
		{
            Recordset mock = MockRepository.Mock<ADODB.Recordset>();
			Assert.NotNull(mock);

            mock.Expect(x => x.ActiveConnection)
                .Return("test");

			Assert.Equal("test", mock.ActiveConnection);
            mock.VerifyAllExpectations();
		}
	}
}
