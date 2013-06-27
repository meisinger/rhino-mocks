using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_Rob
	{
		[Fact]
		public void CanFailIfCalledMoreThanOnceUsingDynamicMock()
		{
            IDemo demo = MockRepository.GenerateDynamicMock<IDemo>();

            demo.Expect(x => x.VoidNoArgs())
                .Repeat.Once();

            demo.Expect(x => x.VoidNoArgs())
                .Repeat.Never();

			Assert.Throws<ExpectationViolationException>(
                "IDemo.VoidNoArgs(); Expected #0, Actual #1.", 
                demo.VoidNoArgs);
		}

		[Fact]
		public void Ayende_View_On_Mocking()
		{
			ISomeSystem mockSomeSystem = MockRepository.GenerateStrictMock<ISomeSystem>();

            mockSomeSystem.Expect(x => x.GetFooFor<ExpectedBar>("foo"))
                .Return(new List<ExpectedBar>());

            ExpectedBarPerformer cut = new ExpectedBarPerformer(mockSomeSystem);

            Assert.Throws<ExpectationViolationException>(
                @"ISomeSystem.GetFooFor<Rhino.Mocks.Tests.FieldsProblem.UnexpectedBar>(""foo""); Expected #1, Actual #1.
ISomeSystem.GetFooFor<Rhino.Mocks.Tests.FieldsProblem.ExpectedBar>(""foo""); Expected #1, Actual #0.",
                () => cut.DoStuffWithExpectedBar("foo"));
		}
	}

	public interface ISomeSystem
	{
		List<TBar> GetFooFor<TBar>(string key) where TBar : Bar;
	}

	public class Bar { }
	public class ExpectedBar : Bar { }
	public class UnexpectedBar : Bar { }

	public class ExpectedBarPerformer
	{
		ISomeSystem system;
		
		public ExpectedBarPerformer(ISomeSystem system)
		{
			this.system = system;
		}

		public void DoStuffWithExpectedBar(string p)
		{
			IList<UnexpectedBar> list = system.GetFooFor<UnexpectedBar>(p);

		}
	}
}
