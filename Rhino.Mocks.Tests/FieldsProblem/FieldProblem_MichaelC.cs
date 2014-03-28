using System;
using System.Collections.ObjectModel;
using Xunit;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_MichaelC
	{
		[Fact]
		public void EventRaiser_ShouldRaiseEvent_OnlyOnce()
		{
            int countOne = 0;
            int countTwo = 0;

            IWithEvent mock = MockRepository.Mock<IWithEvent>();

            mock.ExpectEvent(x => x.Load += null)
                .IgnoreArguments();

			mock.Load += delegate { countOne++; };
			mock.Load += delegate { countTwo++; };

			mock.Raise(x => x.Load += null, EventArgs.Empty);
			Assert.Equal(1, countOne);
			Assert.Equal(1, countTwo);

			mock.Raise(x => x.Load += null, EventArgs.Empty);
			Assert.Equal(2, countOne);
			Assert.Equal(2, countTwo);

			mock.Raise(x => x.Load += null, EventArgs.Empty);
			Assert.Equal(3, countOne);
			Assert.Equal(3, countTwo);
		}
	}
}