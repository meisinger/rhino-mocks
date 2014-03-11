
using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_Lopez
	{
		public interface GenericContainer<T>
		{
			T Item { get; set; }
		}

		[Fact]
		public void PropertyBehaviorForSinglePropertyTypeOfString()
		{
            GenericContainer<string> stringContainer = Repository.Mock<GenericContainer<string>>();

            stringContainer.ExpectProperty(x => x.Item);
            // in order to make this work "strickly"
            // an expectation for the set operation will be needed
            // dateTimeContainer.ExpectProperty(x => x.Item = Arg<string>.Is.Anything);
			
			for (int i = 1; i < 49; ++i)
			{
				string newItem = i.ToString();
                stringContainer.Item = newItem;

				Assert.Equal(newItem, stringContainer.Item);
			}

            stringContainer.VerifyExpectations();
		}

        [Fact]
		public void PropertyBehaviourForSinglePropertyTypeOfDateTime()
		{
            GenericContainer<DateTime> dateTimeContainer = Repository.Mock<GenericContainer<DateTime>>();

            dateTimeContainer.ExpectProperty(x => x.Item);
            // in order to make this work "strickly"
            // an expectation for the set operation will be needed
            // dateTimeContainer.ExpectProperty(x => x.Item = Arg<DateTime>.Is.Anything);

			for (int i = 1; i < 12; i++)
			{
				DateTime date = new DateTime(2007, i, i);
                dateTimeContainer.Item = date;

				Assert.Equal(date, dateTimeContainer.Item);
			}

            dateTimeContainer.VerifyExpectations();
		}

        [Fact]
		public void PropertyBehaviourForSinglePropertyTypeOfInteger()
		{
            GenericContainer<int> dateTimeContainer = Repository.Mock<GenericContainer<int>>();

            dateTimeContainer.ExpectProperty(x => x.Item);
            // in order to make this work "strickly"
            // an expectation for the set operation will be needed
            // dateTimeContainer.ExpectProperty(x => x.Item = Arg<int>.Is.Anything);

			for (int i = 1; i < 49; i++)
			{
				dateTimeContainer.Item = i;

				Assert.Equal(i, dateTimeContainer.Item);
			}

            dateTimeContainer.VerifyExpectations();
		}
	}
}