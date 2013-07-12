using System;
using System.Collections;
using System.Linq;
using Xunit;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.Tests
{
    
    public class RecursiveMocks
    {
        [Fact]
        public void CanUseRecursiveMocks()
        {
            var session = MockRepository.GenerateMock<ISession>();
            session.Stub(x =>
                         x.CreateCriteria(typeof (Customer))
                             .List()
                ).Return(new[] {new Customer {Id = 1, Name = "ayende"}});

            Customer customer = session.CreateCriteria(typeof (Customer))
                .List()
                .Cast<Customer>()
                .First();

            Assert.Equal("ayende", customer.Name);
            Assert.Equal(1, customer.Id);
        }

        [Fact]
        public void CanUseRecursiveMocksSimpler()
        {
            var repository = MockRepository.GenerateMock<IMyService>();

            repository.Expect(x => x.Identity.Name)
                .Return("foo");

            Assert.Equal("foo", repository.Identity.Name);
        }

		[Fact]
        public void CanUseRecursiveMocksSimplerAlternateSyntax()
        {
            var repository = MockRepository.GenerateMock<IMyService>();

            repository.Expect(x => x.Identity.Name)
                .Return("foo");
            
            Assert.Equal("foo", repository.Identity.Name);
        }

		public interface ISession
        {
            ICriteria CreateCriteria(Type type);
        }

        public interface ICriteria
        {
            IList List();
        }

        public class Customer
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public interface IIdentity
        {
            string Name { get; set; }
        }

        public interface IMyService
        {
            IIdentity Identity { get; set; }
        }
    }
}
