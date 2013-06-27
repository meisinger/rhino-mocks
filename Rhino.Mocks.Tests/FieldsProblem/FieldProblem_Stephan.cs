using System;
using System.Collections.Generic;
using Xunit;
using Rhino.Mocks;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_Stefan
	{
		// This test fixture relates to ploblems when ignoring arguments on generic method calls when the type is a struct (aka value type).
		// With reference types - such as String - there is no problem.
		// It has nothing to do with ordering or not -> but if you do not use an ordered mock recorder, then the error msg is not helpful.


		[Fact]
		public void ShouldIgnoreArgumentsOnGenericCallWhenTypeIsStruct()
		{
			// setup
			ISomeService m_SomeServiceMock = MockRepository.GenerateStrictMock<ISomeService>();
			SomeClient sut = new SomeClient(m_SomeServiceMock);

            m_SomeServiceMock.Expect(x => x.DoSomething<string>(null, null))
                .IgnoreArguments();

            m_SomeServiceMock.Expect(x => x.DoSomething<DateTime>(null, default(DateTime)))
                .IgnoreArguments();
            
            // test
			sut.DoSomething();

			// verification
            m_SomeServiceMock.VerifyAllExpectations();

			// cleanup
			m_SomeServiceMock = null;
			sut = null;
		}

		[Fact]
		public void UnexpectedCallToGenericMethod()
		{
			ISomeService m_SomeServiceMock = MockRepository.GenerateStrictMock<ISomeService>();

            m_SomeServiceMock.Expect(x => x.DoSomething<string>(null, "foo"));

            Assert.Throws<ExpectationViolationException>(
				@"ISomeService.DoSomething<System.Int32>(null, 5); Expected #0, Actual #1.
ISomeService.DoSomething<System.String>(null, ""foo""); Expected #1, Actual #0.",
				() => m_SomeServiceMock.DoSomething<int>(null, 5));
		}

		[Fact]
		public void IgnoreArgumentsAfterDo()
		{
            bool didDo = false;

			IDemo demo = MockRepository.GenerateDynamicMock<IDemo>();
            demo.Expect(x => x.VoidNoArgs())
                .IgnoreArguments()
                .Do(SetToTrue(out didDo));
			
			demo.VoidNoArgs();
			Assert.True(didDo, "Do has not been executed!");

            demo.VerifyAllExpectations();
		}
		
		private delegate void PlaceHolder();

        private PlaceHolder SetToTrue(out bool didDo)
        {
			didDo = true;
            return delegate { };
        }
	}

	public interface ISomeService
	{
		void DoSomething<T>(string key, T someObj);
	}


	internal class SomeClient
	{
		private readonly ISomeService m_SomeSvc;

		public SomeClient(ISomeService someSvc)
		{
			m_SomeSvc = someSvc;
		}

		public void DoSomething()
		{
			m_SomeSvc.DoSomething<string>("string.test", "some string");

			m_SomeSvc.DoSomething<DateTime>("struct.test", DateTime.Now);

		}
	}
}