using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_Andy
	{
		[Fact]
		public void MockingPropertyUsingBaseKeyword()
		{
			SubClass mock = MockRepository.Partial<SubClass>();

            mock.Expect(x => x.SubProperty)
                .Return("Foo")
                .Repeat.Any();

            mock.Expect(x => x.BaseProperty)
                .Return("Foo2")
                .Repeat.Any();

			Assert.Equal("Foo", mock.SubProperty);
			Assert.Equal("Foo2", mock.BaseProperty);

            mock.VerifyAllExpectations();
		}
	}

	public abstract class BaseClass
	{
		private string property = null;
		public virtual string BaseProperty
		{
			get { return property; }
			set { this.property = value; }
		}
	}

	public class SubClass : BaseClass
	{
		public virtual string SubProperty
		{
			get { return base.BaseProperty; }
		}

		public override string BaseProperty
		{
			get
			{
				return base.BaseProperty;
			}
			set
			{
				base.BaseProperty = value;
			}
		}

	}
}
