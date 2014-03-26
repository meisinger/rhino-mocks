
using System;
using System.Diagnostics;
using Rhino.Mocks.Exceptions;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_Shane
	{
		[Fact(Skip = "Test No Longer Valid (Ordering Removed)")]
		public void WillMerge_UnorderedRecorder_WhenRecorderHasSingleRecorderInside()
		{
            CustomerMapper mapper = new CustomerMapper();

            ICustomer customer = Repository.Mock<ICustomer>();
            customer.Expect(x => x.Id)
                .Return(0);

            customer.Expect(x => x.IsPreferred = true);

            Assert.Throws<ExpectationViolationException>(
                () => mapper.MarkCustomerAsPreferred(customer));
		}
	}

	public interface ICustomer
	{
		int Id { get; }

		bool IsPreferred { get; set; }
	}

	public class CustomerMapper
	{
		public void MarkCustomerAsPreferred(ICustomer customer)
		{
			customer.IsPreferred = true;

			int id = customer.Id;
		}
	}
}