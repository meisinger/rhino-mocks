
using System.IO;
using Xunit;
using Rhino.Mocks;
using Rhino.Mocks.Loggers;

namespace Rhino.Mocks.Tests
{
	public class TraceWriterWithStackTraceExpectationWriterFixture
	{
		[Fact]
		public void WillPrintLogInfoWithStackTrace()
		{
			TraceWriterWithStackTraceLogger expectationWriter = new TraceWriterWithStackTraceLogger();
			StringWriter writer = new StringWriter();
			expectationWriter.AlternativeWriter = writer;

			RhinoMocks.Logger = expectationWriter;

			IDemo mock = MockRepository.Mock<IDemo>();
            mock.Expect(x => x.VoidNoArgs());

			mock.VoidNoArgs();
            
			Assert.Contains("WillPrintLogInfoWithStackTrace",
				writer.GetStringBuilder().ToString());

            mock.VerifyExpectations();
		}
	}
}