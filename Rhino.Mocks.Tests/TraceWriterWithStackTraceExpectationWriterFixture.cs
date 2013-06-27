
using System.IO;
using Xunit;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.Tests
{
	public class TraceWriterWithStackTraceExpectationWriterFixture
	{
		[Fact]
		public void WillPrintLogInfoWithStackTrace()
		{
			TraceWriterWithStackTraceExpectationWriter expectationWriter = 
                new TraceWriterWithStackTraceExpectationWriter();
			StringWriter writer = new StringWriter();
			expectationWriter.AlternativeWriter = writer;

			RhinoMocks.Logger = expectationWriter;

			IDemo mock = MockRepository.GenerateStrictMock<IDemo>();
            mock.Expect(x => x.VoidNoArgs());

			mock.VoidNoArgs();
            
			Assert.Contains("WillPrintLogInfoWithStackTrace",
				writer.GetStringBuilder().ToString());

            mock.VerifyAllExpectations();
		}
	}
}