#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using Xunit;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class UsingEvents
	{
        private IEventRaiser raiser;

		public UsingEvents()
		{
		}

		[Fact]
		public void VerifyingThatEventWasAttached()
		{
            IWithEvents events = Repository.Mock<IWithEvents>();
            events.Expect(x => x.Blah += new EventHandler(events_Blah));

			MethodThatSubscribeToEventBlah(events);

            events.VerifyExpectations();
		}

		public void MethodThatSubscribeToEventBlah(IWithEvents events)
		{
			events.Blah += new EventHandler(events_Blah);
		}

		[Fact]
		public void VerifyingThatAnEventWasFired()
		{
            IEventSubscriber subscriber = Repository.Mock<IEventSubscriber>();
            IWithEvents events = new WithEvents();

            // This doesn't create an expectation because no method is called on subscriber!!
            events.Blah += new EventHandler(subscriber.Hanlder);
            subscriber.Expect(x => x.Hanlder(events, EventArgs.Empty));
            			
			events.RaiseEvent();

            subscriber.VerifyExpectations();
		}

		[Fact]
		public void VerifyingThatAnEventWasFiredThrowsForDifferentArgument()
		{
            IEventSubscriber subscriber = Repository.Mock<IEventSubscriber>();
			IWithEvents events = new WithEvents();
			
            // This doesn't create an expectation because no method is called on subscriber!!
			events.Blah += new EventHandler(subscriber.Hanlder);
            subscriber.Expect(x => x.Hanlder(events, new EventArgs()));
			
			Assert.Throws<ExpectationViolationException>(
				"IEventSubscriber.Hanlder(Rhino.Mocks.Tests.FieldsProblem.WithEvents, System.EventArgs); Expected #0, Actual #1.\r\nIEventSubscriber.Hanlder(Rhino.Mocks.Tests.FieldsProblem.WithEvents, System.EventArgs); Expected #1, Actual #0.",
				() => events.RaiseEvent());
		}

		[Fact]
		public void CanSetExpectationToUnsubscribeFromEvent()
		{
            IWithEvents events = Repository.Mock<IWithEvents>();

            events.Expect(x => x.Blah += new EventHandler(events_Blah));
            events.Expect(x => x.Blah -= new EventHandler(events_Blah));
			

			events.Blah += new EventHandler(events_Blah);
			events.Blah -= new EventHandler(events_Blah);

            events.VerifyExpectations();
		}

		[Fact]
		public void VerifyingExceptionIfEventIsNotAttached()
		{
			IWithEvents events = Repository.Mock<IWithEvents>();

            events.Expect(x => x.Blah += new EventHandler(events_Blah));

			Assert.Throws<ExpectationViolationException>(
                "IWithEvents.add_Blah(System.EventHandler); Expected #1, Actual #0.",
                () => events.VerifyExpectations());
		}

		[Fact]
		public void VerifyingThatCanAttackOtherEvent()
		{
            IWithEvents events = Repository.Mock<IWithEvents>();

            events.Expect(x => x.Blah += new EventHandler(events_Blah))
                .IgnoreArguments();

			events.Blah += new EventHandler(events_Blah_Other);

            events.VerifyExpectations();
		}

        //[Fact]
        //public void BetterErrorMessageOnIncorrectParametersCount()
        //{
        //    IWithEvents events = Repository.Mock<IWithEvents>();

        //    raiser = events.Expect(x => x.Blah += null)
        //        .IgnoreArguments()
        //        .GetEventRaiser();

        //    events.Blah += delegate { };
			
        //    Assert.Throws<InvalidOperationException>(
        //        "You have called the event raiser with the wrong number of parameters. Expected 2 but was 0",
        //        () => raiser.Raise(null));
        //}

        //[Fact]
        //public void BetterErrorMessageOnIncorrectParameters()
        //{
        //    IWithEvents events = Repository.Mock<IWithEvents>();

        //    raiser = events.Expect(x => x.Blah += null)
        //        .IgnoreArguments()
        //        .GetEventRaiser();

        //    events.Blah += delegate { };

        //    Assert.Throws<InvalidOperationException>(
        //        "Parameter #2 is System.Int32 but should be System.EventArgs",
        //        () => raiser.Raise("", 1));
        //}

        //[Fact]
        //public void RaiseEvent()
        //{
        //    IWithEvents eventHolder = Repository.Mock<IWithEvents>();

        //    raiser = eventHolder
        //        .Expect(x => x.Blah += null)
        //        .IgnoreArguments()
        //        .GetEventRaiser();

        //    eventHolder.Expect(x => x.RaiseEvent())
        //        .Do(new System.Threading.ThreadStart(UseEventRaiser));

        //    IEventSubscriber eventSubscriber = Repository.Mock<IEventSubscriber>();
        //    eventSubscriber.Expect(x => x.Hanlder(this, EventArgs.Empty));

        //    eventHolder.Blah += new EventHandler(eventSubscriber.Hanlder);
        //    eventHolder.RaiseEvent();

        //    eventHolder.VerifyExpectations();
        //    eventSubscriber.VerifyExpectations();
        //}

        //[Fact]
        //public void UsingEventRaiserCreate()
        //{
        //    IWithEvents eventHolder = Repository.Mock<IWithEvents>();

        //    //IEventRaiser eventRaiser = EventRaiser.Create(eventHolder, "Blah");
        //    IEventRaiser eventRaiser = eventHolder.GetEventRaiser(x => x.Blah += null);

        //    bool called = false;
        //    eventHolder.Blah += delegate
        //    {
        //        called = true;
        //    };

        //    eventRaiser.Raise(this, EventArgs.Empty);

        //    Assert.True(called);
        //    eventHolder.VerifyExpectations();
        //}

        //[Fact]
        //public void RaiseEventUsingExtensionMethod() 
        //{
        //    IWithEvents eventHolder = Repository.Mock<IWithEvents>();

        //    bool called = false;
        //    eventHolder.Blah += delegate {
        //        called = true;
        //    };
            
        //    eventHolder.Raise(stub => stub.Blah += null, this, EventArgs.Empty);
            
        //    Assert.True(called);
        //}

        //[Fact]
        //public void UsingEventRaiserFromExtensionMethod() 
        //{
        //    IWithEvents eventHolder = (IWithEvents)MockRepository.GenerateStub(typeof(IWithEvents));

        //    IEventRaiser eventRaiser = eventHolder
        //        .GetEventRaiser(stub => stub.Blah += null);
			
        //    bool called = false;
        //    eventHolder.Blah += delegate {
        //        called = true;
        //    };
            
        //    eventRaiser.Raise(this, EventArgs.Empty);

        //    Assert.True(called);
        //    eventHolder.VerifyAllExpectations();
        //}

        private void events_Blah_Other(object sender, EventArgs e)
        {
        }

        private void events_Blah(object sender, EventArgs e)
        {
        }

        private void UseEventRaiser()
        {
            raiser.Raise(this, EventArgs.Empty);
        }
	}

	public interface IWithEvents
	{
		event EventHandler Blah;
		void RaiseEvent();
	}

	public interface IEventSubscriber
	{
		void Hanlder(object sender, EventArgs e);
	}

	public class WithEvents : IWithEvents
	{
		public event System.EventHandler Blah;

		public void RaiseEvent()
		{
			if (Blah != null)
				Blah(this, EventArgs.Empty);
		}
	}
}
