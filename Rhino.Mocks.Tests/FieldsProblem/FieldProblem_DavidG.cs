using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_DavidG
	{
		[Fact]
		public void GenericConstrainedMethod()
		{
            IStore1 store1 = Repository.Mock<IStore1>();
            IStore2 store2 = Repository.Mock<IStore2>();
			Assert.NotNull(store2);
			Assert.NotNull(store1);
		}
	}

	public interface IStore1
	{
		R[] TestMethod<R>();
	}

	public interface IStore2
	{
		R[] TestMethod<R>() where R : DomainObject<R>;
	}

	public class MyTestObject : DomainObject<MyTestObject>
	{
	}

	public abstract class DomainObject<T> where T : DomainObject<T>
	{
	}
}
