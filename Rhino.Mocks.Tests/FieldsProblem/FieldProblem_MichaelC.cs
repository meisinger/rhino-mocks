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

            IWithEvent mock = MockRepository.GenerateStrictMock<IWithEvent>();

            IEventRaiser raiser = mock
                .Expect(x => x.Load += null)
                .IgnoreArguments()
                .Repeat.Twice()
                .GetEventRaiser();

			mock.Load += delegate { countOne++; };
			mock.Load += delegate { countTwo++; };

			raiser.Raise(this, EventArgs.Empty);
			Assert.Equal(1, countOne);
			Assert.Equal(1, countTwo);

			raiser.Raise(this, EventArgs.Empty);
			Assert.Equal(2, countOne);
			Assert.Equal(2, countTwo);

			raiser.Raise(this, EventArgs.Empty);
			Assert.Equal(3, countOne);
			Assert.Equal(3, countTwo);
		}
	}
}