
using System;
using System.Threading;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_Naraga
	{
		public interface IService
		{
			void Do(string msg);
		}

		[Fact]
		public void MultiThreadedReplay()
		{
			var service = Repository.Mock<IService>();
			for (int i = 0; i < 100; i++)
			{
				int i1 = i;
                service.Expect(x => x.Do("message" + i1));
			}
			
            int counter = 0;
            for (int i = 0; i < 100; i++)
            {
                var i1 = i;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    service.Do("message" + i1);
                    Interlocked.Increment(ref counter);
                });
            }

            while (counter != 100)
                Thread.Sleep(100);

            service.VerifyExpectations(true);
		}
	}
}