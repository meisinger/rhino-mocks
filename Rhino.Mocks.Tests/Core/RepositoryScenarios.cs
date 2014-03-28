
using Xunit;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Exceptions;
using System;

namespace Rhino.Mocks.Tests.Core
{
    public class RepositoryScenarios
    {
        public delegate string StringScenario(string arg1, string arg2);
        public delegate void VoidScenario(string arg1, string arg2);

        public interface IScenarioEvent
        {
            event EventHandler<EventArgs> ScenarioEvent;
        }

        public interface IScenarioArgument
        {
            string Name { get; set; }
            int Age { get; set; }

            string MessageOut { get; }
            string MessageIn { set; }
        }

        public interface IScenarioObject
        {
            void VoidMethod();
            string StringMethod();
            string StringMethodEcho(string value);
            int IntegerMethodArgument(IScenarioArgument argument);
            T GenericMethod<T>();
        }

        public class ScenarioObject : IScenarioObject
        {
            public string Name { get; set; }

            public string NonVirtualStringMethod()
            {
                return string.Empty;
            }

            public virtual void VoidMethod()
            {
                throw new System.NotImplementedException();
            }

            public virtual string StringMethod()
            {
                throw new System.NotImplementedException();
            }

            public virtual string StringMethodEcho(string value)
            {
                return value;
            }

            public virtual int IntegerMethodArgument(IScenarioArgument argument)
            {
                return argument.Age;
            }

            public virtual T GenericMethod<T>()
            {
                throw new System.NotImplementedException();
            }
        }

        [Fact]
        public void Mock_Class_Can_Be_Created_From_Repository()
        {
            var mock = MockRepository.Partial<ScenarioObject>();
            Assert.NotNull(mock);
            Assert.True(mock is ScenarioObject);
        }

        [Fact]
        public void Mock_Class_Created_From_Repository_Is_Of_Type_IMockInstance()
        {
            var mock = MockRepository.Partial<ScenarioObject>();
            var instance = mock as IMockInstance;
            Assert.NotNull(instance);
            Assert.True(mock is ScenarioObject);
        }

        [Fact]
        public void Mock_Class_Can_Have_An_Expectation_Set_On_Method_With_Void_Return_Type()
        {
            var mock = MockRepository.Partial<ScenarioObject>();
            var options = mock.Expect(x => x.VoidMethod());
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Class_Can_Have_An_Expectation_Set_On_Method_With_A_Return_Type()
        {
            var mock = MockRepository.Partial<ScenarioObject>();
            var options = mock.Expect(x => x.StringMethod());
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Exception_Is_Raised_When_An_Expectation_Is_Set_On_NonVirtual_Method()
        {
            var mock = MockRepository.Partial<ScenarioObject>();
            Assert.Throws<System.InvalidOperationException>(
                () => mock.Expect(x => x.NonVirtualStringMethod()));
        }

        [Fact]
        public void Mock_Delegate_Can_Be_Create_From_Repository()
        {
            var mock = MockRepository.Mock<StringScenario>();
            Assert.NotNull(mock);
            Assert.True(mock is StringScenario);
        }

        [Fact]
        public void Mock_Delegate_Created_From_Repository_Is_Of_Type_IMockInstance()
        {
            var mock = MockRepository.Mock<StringScenario>();
            var instance = mock.Target as IMockInstance;
            Assert.NotNull(instance);
            Assert.True(mock is StringScenario);
        }

        [Fact]
        void Mock_Delegate_With_Void_Return_Type_Can_Have_An_Expectation_Set()
        {
            var mock = MockRepository.Mock<VoidScenario>();
            var options = mock.Expect(x => x("mike", "meisinger"));
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Delegate_With_Return_Type_Can_Have_An_Expectation_Set()
        {
            var mock = MockRepository.Mock<StringScenario>();
            var options = mock.Expect(x => x("mike", "meisinger"));
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Instance_Is_Assigned_Invocation_Proxy_Once_Proxy_For_A_Mock_Class_Is_Created()
        {
            var mock = MockRepository.Partial<ScenarioObject>();

            var instance = mock as IMockInstance;
            Assert.NotNull(instance.ProxyInstance);

            mock.Expect(x => x.StringMethod())
                .Return("meisinger");

            Assert.NotNull(instance.ProxyInstance);

            var result = mock.StringMethod();
            Assert.Equal("meisinger", result);
        }

        [Fact]
        public void Mock_Instance_Has_Method_Invoked_When_Expectation_Indicates_Original_Method_Should_Be_Called()
        {
            var mock = MockRepository.Partial<ScenarioObject>();
            mock.Expect(x => x.StringMethod())
                .CallOriginalMethod();

            Assert.Throws<System.NotImplementedException>(() => mock.StringMethod());
        }

        [Fact]
        public void Mock_Instance_Does_Not_Return_Expected_Value_When_Expectation_Indicates_Original_Method_Should_Be_Called()
        {
            var mock = MockRepository.Partial<ScenarioObject>();
            mock.Expect(x => x.StringMethodEcho("meisinger"))
                .CallOriginalMethod();

            var result = mock.StringMethodEcho("meisinger");
            Assert.Equal("meisinger", result);
        }

        [Fact]
        public void Mock_Interface_Can_Be_Created_From_Repository()
        {
            var mock = MockRepository.Mock<IScenarioObject>();
            Assert.NotNull(mock);
            Assert.True(mock is IScenarioObject);
        }

        [Fact]
        public void Mock_Interface_Created_From_Repository_Is_Of_Type_IMockInstance()
        {
            var mock = MockRepository.Mock<IScenarioObject>();
            var instance = mock as IMockInstance;
            Assert.NotNull(instance);
            Assert.True(mock is IScenarioObject);
        }

        [Fact]
        public void Mock_Interface_Can_Have_An_Expectation_Set_On_Method_With_Void_Return_Type()
        {
            var mock = MockRepository.Mock<IScenarioObject>();
            var options = mock.Expect(x => x.VoidMethod());
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Interface_Can_Have_An_Expectation_Set_On_Method_With_A_Return_Type()
        {
            var mock = MockRepository.Mock<IScenarioObject>();
            var options = mock.Expect(x => x.StringMethod());
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Interface_Can_Have_A_Generic_Expectation_Set_On_Method_With_A_Return_Type()
        {
            var mock = MockRepository.Mock<IScenarioObject>();
            var options = mock.Expect(x => x.GenericMethod<int>());
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Instance_Is_Assigned_Invocation_Proxy_Once_Proxy_For_A_Mock_Interface_Is_Created()
        {
            var mock = MockRepository.Mock<IScenarioObject>();
            
            var instance = mock as IMockInstance;
            Assert.NotNull(instance.ProxyInstance);

            mock.Expect(x => x.StringMethod())
                .Return("meisinger");

            Assert.NotNull(instance.ProxyInstance);

            var result = mock.StringMethod();
            Assert.Equal("meisinger", result);
        }

        [Fact]
        public void Mock_Instance_Can_Distinguish_Between_Duplicate_Method_Expectation_With_Unique_Arguments()
        {
            var mock = MockRepository.Partial<ScenarioObject>();

            mock.Expect(x => x.StringMethodEcho("one"))
                .Return("value one");

            mock.Expect(x => x.StringMethodEcho("two"))
                .Return("value two");

            var resultOne = mock.StringMethodEcho("one");
            var resultTwo = mock.StringMethodEcho("two");

            Assert.Equal("value one", resultOne);
            Assert.Equal("value two", resultTwo);
        }

        [Fact]
        public void Mock_Instance_Can_Distinguish_Between_Duplicate_Method_Expectation_With_Complex_Arguments()
        {
            var wasCalled = false;
            var argumentMock = MockRepository.Mock<IScenarioArgument>();
            var mock = MockRepository.Partial<ScenarioObject>();

            argumentMock.ExpectProperty(x => x.Age)
                .Return(15);

            mock.Expect(x => x.IntegerMethodArgument(argumentMock))
                .WhenCalled<IScenarioArgument>(x =>
                {
                    if (x.Age == 15)
                        wasCalled = true;
                })
                .Return(24);

            mock.Expect(x => x.IntegerMethodArgument(argumentMock))
                .CallOriginalMethod();

            var resultOne = mock.IntegerMethodArgument(argumentMock);
            var resultTwo = mock.IntegerMethodArgument(argumentMock);

            Assert.Equal(24, resultOne);
            Assert.Equal(15, resultTwo);
            Assert.True(wasCalled);
        }

        [Fact]
        public void Mock_Instance_Can_Distinguish_Between_Method_Expectation_With_Generic_Arguments()
        {
            var mock = MockRepository.Partial<ScenarioObject>();
            
            mock.Expect(x => x.StringMethodEcho(Arg<string>.Is.Anything))
                .Return("rhino")
                .Repeat.Any();

            var resultTwo = mock.StringMethodEcho("mike");
            var resultOne = mock.StringMethodEcho("ayende");

            Assert.Equal("rhino", resultOne);
            Assert.Equal("rhino", resultTwo);
        }

        [Fact]
        public void Mock_Instance_Is_Called_Twice_When_Expectation_Is_Set_To_Repeat_Twice()
        {
            var mock = MockRepository.Partial<ScenarioObject>();

            mock.Expect(x => x.StringMethodEcho("ayende"))
                .Return("rahien")
                .Repeat.Twice();

            mock.Expect(x => x.StringMethodEcho("mike"))
                .Return("meisinger");

            var resultOne = mock.StringMethodEcho("ayende");
            var resultTwo = mock.StringMethodEcho("mike");
            var resultThree = mock.StringMethodEcho("ayende");

            Assert.Equal("rahien", resultOne);
            Assert.Equal("meisinger", resultTwo);
            Assert.Equal("rahien", resultThree);

            mock.VerifyExpectations();
        }

        [Fact]
        public void Mock_Instance_Is_Called_With_Invalid_Arguments_When_Expectation_Is_Set_To_Ignore_Arguments()
        {
            var mock = MockRepository.Partial<ScenarioObject>();

            mock.Expect(x => x.StringMethodEcho("ayende"))
                .IgnoreArguments()
                .Return("rahien")
                .Repeat.Twice();

            mock.Expect(x => x.StringMethodEcho("mike"))
                .IgnoreArguments()
                .Return("meisinger");

            var resultOne = mock.StringMethodEcho("invalid_1");
            var resultTwo = mock.StringMethodEcho("invalid_2");
            var resultThree = mock.StringMethodEcho("invalid_3");

            Assert.Equal("rahien", resultOne);
            Assert.Equal("rahien", resultTwo);
            Assert.Equal("meisinger", resultThree);

            mock.VerifyExpectations();
        }

        [Fact]
        public void Mock_Instance_Maintains_Separate_Expectations()
        {
            var mockOne = MockRepository.Partial<ScenarioObject>();
            var mockTwo = MockRepository.Partial<ScenarioObject>();

            mockOne.Expect(x => x.StringMethod())
                .Return("one")
                .Repeat.Any();

            mockTwo.Expect(x => x.StringMethod())
                .Return("two")
                .Repeat.Any();

            var resultTwo = mockTwo.StringMethod();
            var resultOne = mockOne.StringMethod();
            
            Assert.Equal("one", resultOne);
            Assert.Equal("two", resultTwo);
        }

        [Fact]
        public void Mock_Interface_With_Event_Can_Have_Expectation_Set_Against_Event()
        {
            var mock = MockRepository.Mock<IScenarioEvent>();
            mock.ExpectEvent(x => x.ScenarioEvent += null);
        }

        [Fact]
        public void Verification_Throws_Exception_When_Expectations_Set_Against_Mock_Class_Are_Not_Met()
        {
            var mock = MockRepository.Partial<ScenarioObject>();

            mock.Expect(x => x.StringMethodEcho(Arg.Text.StartsWith("m")))
                .Return("one")
                .Repeat.Times(3);

            var first = mock.StringMethodEcho("mike");
            var second = mock.StringMethodEcho("meisinger");

            Assert.Equal("one", first);
            Assert.Equal("one", second);

            Assert.Throws<ExpectationViolationException>(
                () => mock.VerifyExpectations());
        }

        [Fact]
        public void Verification_Throws_Exception_When_Expectations_Set_Against_Mock_Delegate_Are_Not_Met()
        {
            var mock = MockRepository.Mock<StringScenario>();
            mock.Expect(x => x("mike", "meisinger"))
                .Return("one")
                .Repeat.Times(3);

            var first = mock("mike", "meisinger");
            var second = mock("mike", "meisinger");

            Assert.Equal("one", first);
            Assert.Equal("one", second);

            Assert.Throws<ExpectationViolationException>(() => mock.VerifyExpectations());
        }

        [Fact]
        public void When_Asserting_A_Method_Was_Called_With_Expectation_No_Exception_Is_Thrown_When_Method_Was_Called()
        {
            var mock = MockRepository.Mock<IScenarioObject>();
            mock.Expect(x => x.StringMethodEcho("mike"))
                .Return("true");

            var value = mock.StringMethodEcho("mike");
            Assert.Equal("true", value);

            mock.AssertWasCalled(x => x.StringMethodEcho("mike"));
        }

        [Fact]
        public void When_Asserting_A_Method_Was_Called_With_No_Expectation_No_Exception_Is_Thrown_When_Method_Was_Called()
        {
            var mock = MockRepository.Mock<IScenarioObject>();
            mock.StringMethodEcho("mike");

            mock.AssertWasCalled(x => x.StringMethodEcho("mike"));
        }

        [Fact]
        public void Expectation_Throws_Exception_When_Setting_Expectation_To_Throw()
        {
            var mock = MockRepository.Mock<IScenarioObject>();

            mock.Expect(x => x.StringMethod())
                .Throws<System.InvalidTimeZoneException>();

            Assert.Throws<System.InvalidTimeZoneException>(() => mock.StringMethod());
        }

        [Fact]
        public void Can_Create_Expectation_For_A_Property_With_Default_Behavior()
        {
            var mock = MockRepository.Mock<IScenarioArgument>();

            mock.ExpectProperty(x => x.Name)
                .Return("mike");

            var actual = mock.Name;
            Assert.Equal("mike", actual);

            mock.VerifyExpectations();
        }

        [Fact]
        public void Property_With_Default_Behavior_Ignores_Set_Values_If_Expectation_Has_Return_Value()
        {
            var mock = MockRepository.Mock<IScenarioArgument>();

            mock.ExpectProperty(x => x.Name)
                .Return("returned_value");

            mock.Name = "ignored";

            var actual = mock.Name;
            Assert.Equal("returned_value", actual);

            mock.VerifyExpectations();
        }

        [Fact]
        public void Property_With_Default_Behavior_Tracks_Return_Values_When_Different_Expectations_Are_Set()
        {
            var mock = MockRepository.Mock<IScenarioArgument>();

            mock.ExpectProperty(x => x.Name = "first")
                .Return("First");

            mock.ExpectProperty(x => x.Name = "second")
                .Return("Second");

            mock.Name = "first";
            var first = mock.Name;

            mock.Name = "second";
            var second = mock.Name;

            Assert.Equal("First", first);
            Assert.Equal("Second", second);

            mock.VerifyExpectations();
        }

        [Fact(Skip = "Feature Not Working (Expect remove)")]
        public void Property_With_Default_Behavior_And_A_Get_Expectation_Defaults_To_The_Return_Value()
        {
            var mock = MockRepository.Mock<IScenarioArgument>();

            mock.ExpectProperty(x => x.Name)
                .Return("Default");

            mock.ExpectProperty(x => x.Name = "first")
                .Return("First");

            mock.ExpectProperty(x => x.Name = "second")
                .Return("Second");

            mock.Name = "first";
            var first = mock.Name;

            mock.Name = "unexpected";
            var unexpected = mock.Name;

            mock.Name = "second";
            var second = mock.Name;

            Assert.Equal("First", first);
            Assert.Equal("Second", second);
            Assert.Equal("Default", unexpected);

            mock.VerifyExpectations();
        }

        [Fact]
        public void Property_With_Default_Behavior_Works_With_Dynamic_Property_Handling()
        {
            var mock = MockRepository.Mock<IScenarioArgument>();

            mock.ExpectProperty(x => x.Name = "first")
                .Return("First");

            mock.ExpectProperty(x => x.Name = "second")
                .Return("Second");

            mock.Name = "first";
            var first = mock.Name;

            mock.Name = "unexpected";
            var unexpected = mock.Name;

            mock.Name = "second";
            var second = mock.Name;

            Assert.Equal("First", first);
            Assert.Equal("Second", second);
            Assert.Equal("unexpected", unexpected);

            mock.VerifyExpectations();
        }

        [Fact]
        public void Setting_A_Return_Value_For_Write_Only_Property_Throws_Exception()
        {
            var mock = MockRepository.Mock<IScenarioArgument>();

            Assert.Throws<System.InvalidOperationException>(() =>
                mock.ExpectProperty(x => x.MessageIn = "in")
                    .Return("invalid"));
        }

        [Fact]
        public void Can_Create_Expectation_For_Event()
        {
            var mock = MockRepository.Mock<IScenarioEvent>();

            var eventValue = 0;
            var noneWasCalled = false;
            var someWasCalled = false;

            mock.ExpectEvent(x => x.ScenarioEvent += Arg<EventHandler<EventArgs>>.Is.Anything);

            mock.ScenarioEvent += (s, e) => { eventValue += 1; };

            mock.Raise(x => x.ScenarioEvent += null, new EventArgs());
            mock.Raise(x => x.ScenarioEvent += null, new EventArgs());

            Assert.False(noneWasCalled);
            Assert.False(someWasCalled);
            Assert.Equal(2, eventValue);
            mock.VerifyExpectations();
        }

        [Fact]
        public void Can_Get_Arguments_For_Call_Made_On_Method()
        {
            var mock = MockRepository.Mock<IScenarioObject>();
            mock.StringMethodEcho("mike");
            mock.StringMethodEcho("meisinger");

            var actuals = mock.GetArgumentsForCallsMadeOn(x => 
                x.StringMethodEcho(Arg<string>.Is.Anything));

            Assert.Equal(2, actuals.Length);

            var first = actuals[0];
            var firstArguments = first.Arguments;
            Assert.Equal(1, firstArguments.Length);
            Assert.Equal("mike", firstArguments[0]);

            var second = actuals[1];
            var secondArguments = second.Arguments;
            Assert.Equal(1, secondArguments.Length);
            Assert.Equal("meisinger", secondArguments[0]);
        }
    }
}
