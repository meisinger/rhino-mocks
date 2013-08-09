
using Xunit;
using Rhino.Mocks.Core;
using Rhino.Mocks.Core.Extensions;
using Rhino.Mocks.Core.Interfaces;

namespace Rhino.Mocks.Tests.Core
{
    public class RepositoryScenarios
    {
        public delegate string StringScenario(string arg1, string arg2);
        public delegate void VoidScenario(string arg1, string arg2);

        public interface IScenarioEvent
        {
            event VoidScenario ScenarioEvent;
        }

        public interface IScenarioArgument
        {
            string Name { get; set; }
            int Age { get; set; }
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
            var mock = Repository.Mock<ScenarioObject>();
            Assert.NotNull(mock);
            Assert.True(mock is ScenarioObject);
        }

        [Fact]
        public void Mock_Class_Created_From_Repository_Is_Of_Type_IMockInstance()
        {
            var mock = Repository.Mock<ScenarioObject>();
            var instance = mock as IMockInstance;
            Assert.NotNull(instance);
            Assert.True(mock is ScenarioObject);
        }

        [Fact]
        public void Mock_Class_Can_Have_An_Expectation_Set_On_Method_With_Void_Return_Type()
        {
            var mock = Repository.Mock<ScenarioObject>();
            var options = mock.ExpectCall(x => x.VoidMethod());
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Class_Can_Have_An_Expectation_Set_On_Method_With_A_Return_Type()
        {
            var mock = Repository.Mock<ScenarioObject>();
            var options = mock.ExpectCall(x => x.StringMethod());
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Exception_Is_Raised_When_An_Expectation_Is_Set_On_NonVirtual_Method()
        {
            var mock = Repository.Mock<ScenarioObject>();
            Assert.Throws<System.InvalidOperationException>(
                () => mock.ExpectCall(x => x.NonVirtualStringMethod()));
        }

        [Fact]
        public void Mock_Delegate_Can_Be_Create_From_Repository()
        {
            var mock = Repository.Mock<StringScenario>();
            Assert.NotNull(mock);
            Assert.True(mock is StringScenario);
        }

        [Fact]
        public void Mock_Delegate_Created_From_Repository_Is_Of_Type_IMockInstance()
        {
            var mock = Repository.Mock<StringScenario>();
            var instance = mock.Target as IMockInstance;
            Assert.NotNull(instance);
            Assert.True(mock is StringScenario);
        }

        [Fact]
        void Mock_Delegate_With_Void_Return_Type_Can_Have_An_Expectation_Set()
        {
            var mock = Repository.Mock<VoidScenario>();
            var options = mock.ExpectCall(x => x("mike", "meisinger"));
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Delegate_With_Return_Type_Can_Have_An_Expectation_Set()
        {
            var mock = Repository.Mock<StringScenario>();
            var options = mock.ExpectCall(x => x("mike", "meisinger"));
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Instance_Is_Assigned_Invocation_Proxy_When_Expectation_Is_Set_On_Method_From_A_Mock_Class()
        {
            var mock = Repository.Mock<ScenarioObject>();

            var instance = mock as IMockInstance;
            Assert.Null(instance.ProxyInstance);

            mock.ExpectCall(x => x.StringMethod())
                .Return("meisinger");

            Assert.NotNull(instance.ProxyInstance);

            var result = mock.StringMethod();
            Assert.Equal("meisinger", result);
        }

        [Fact]
        public void Mock_Instance_Has_Method_Invoked_When_Expectation_Indicates_Original_Method_Should_Be_Called()
        {
            var mock = Repository.Mock<ScenarioObject>();
            mock.ExpectCall(x => x.StringMethod())
                .CallOriginalMethod();

            Assert.Throws<System.NotImplementedException>(() => mock.StringMethod());
        }

        [Fact]
        public void Mock_Instance_Does_Not_Return_Expected_Value_When_Expectation_Indicates_Original_Method_Should_Be_Called()
        {
            var mock = Repository.Mock<ScenarioObject>();
            mock.ExpectCall(x => x.StringMethodEcho("meisinger"))
                .Return("ayende")
                .CallOriginalMethod();

            var result = mock.StringMethodEcho("meisinger");
            Assert.Equal("meisinger", result);
        }

        [Fact]
        public void Mock_Interface_Can_Be_Created_From_Repository()
        {
            var mock = Repository.Mock<IScenarioObject>();
            Assert.NotNull(mock);
            Assert.True(mock is IScenarioObject);
        }

        [Fact]
        public void Mock_Interface_Created_From_Repository_Is_Of_Type_IMockInstance()
        {
            var mock = Repository.Mock<IScenarioObject>();
            var instance = mock as IMockInstance;
            Assert.NotNull(instance);
            Assert.True(mock is IScenarioObject);
        }

        [Fact]
        public void Mock_Interface_Can_Have_An_Expectation_Set_On_Method_With_Void_Return_Type()
        {
            var mock = Repository.Mock<IScenarioObject>();
            var options = mock.ExpectCall(x => x.VoidMethod());
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Interface_Can_Have_An_Expectation_Set_On_Method_With_A_Return_Type()
        {
            var mock = Repository.Mock<IScenarioObject>();
            var options = mock.ExpectCall(x => x.StringMethod());
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Interface_Can_Have_A_Generic_Expectation_Set_On_Method_With_A_Return_Type()
        {
            var mock = Repository.Mock<IScenarioObject>();
            var options = mock.ExpectCall(x => x.GenericMethod<int>());
            Assert.NotNull(mock);
            Assert.NotNull(options);
        }

        [Fact]
        public void Mock_Instance_Is_Assigned_Invocation_Proxy_When_Expectation_Is_Set_On_Method_From_A_Mock_Interface()
        {
            var mock = Repository.Mock<IScenarioObject>();
            
            var instance = mock as IMockInstance;
            Assert.Null(instance.ProxyInstance);

            mock.ExpectCall(x => x.StringMethod())
                .Return("meisinger");

            Assert.NotNull(instance.ProxyInstance);

            var result = mock.StringMethod();
            Assert.Equal("meisinger", result);
        }

        [Fact]
        public void Mock_Instance_Can_Distinguish_Between_Duplicate_Method_Expectation_With_Unique_Arguments()
        {
            var mock = Repository.Mock<ScenarioObject>();

            mock.ExpectCall(x => x.StringMethodEcho("one"))
                .Return("value one");

            mock.ExpectCall(x => x.StringMethodEcho("two"))
                .Return("value two");

            var resultOne = mock.StringMethodEcho("one");
            var resultTwo = mock.StringMethodEcho("two");

            Assert.Equal("value one", resultOne);
            Assert.Equal("value two", resultTwo);
        }

        [Fact]
        public void Mock_Instance_Can_Distinguish_Between_Duplicate_Method_Expectation_With_Complex_Arguments()
        {
            var argumentMock = Repository.Mock<IScenarioArgument>();
            var mock = Repository.Mock<ScenarioObject>();

            argumentMock.ExpectCall(x => x.Age = 15);
            argumentMock.ExpectCall(x => x.Age)
                .Return(15);

            mock.ExpectCall(x => x.IntegerMethodArgument(argumentMock))
                .Return(24);

            mock.ExpectCall(x => x.IntegerMethodArgument(argumentMock))
                .CallOriginalMethod();

            var resultOne = mock.IntegerMethodArgument(argumentMock);
            var resultTwo = mock.IntegerMethodArgument(argumentMock);

            Assert.Equal(24, resultOne);
            Assert.Equal(15, resultTwo);
        }

        [Fact]
        public void Mock_Instance_Can_Distinguish_Between_Method_Expectation_With_Generic_Arguments()
        {
            var mock = Repository.Mock<ScenarioObject>();
            
            mock.ExpectCall(x => x.StringMethodEcho(Mocks.Core.Arg<string>.Is.Anything))
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
            var mock = Repository.Mock<ScenarioObject>();

            mock.ExpectCall(x => x.StringMethodEcho("ayende"))
                .Return("rahien")
                .Repeat.Twice();

            mock.ExpectCall(x => x.StringMethodEcho("mike"))
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
            var mock = Repository.Mock<ScenarioObject>();

            mock.ExpectCall(x => x.StringMethodEcho("ayende"))
                .IgnoreArguments()
                .Return("rahien")
                .Repeat.Twice();

            mock.ExpectCall(x => x.StringMethodEcho("mike"))
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
            var mockOne = Repository.Mock<ScenarioObject>();
            var mockTwo = Repository.Mock<ScenarioObject>();

            mockOne.ExpectCall(x => x.StringMethod())
                .Return("one")
                .Repeat.Any();

            mockTwo.ExpectCall(x => x.StringMethod())
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
            var mock = Repository.Mock<IScenarioEvent>();
            mock.ExpectEvent(x => x.ScenarioEvent += null);
        }

        [Fact]
        public void Verification_Throws_Exception_When_Expectations_Set_Against_Mock_Class_Are_Not_Met()
        {
            var mock = Repository.Mock<ScenarioObject>();

            mock.ExpectCall(x => x.StringMethodEcho(Mocks.Core.Arg.Text.StartsWith("m")))
                .Return("one")
                .Repeat.Times(3);

            var first = mock.StringMethodEcho("mike");
            var second = mock.StringMethodEcho("meisinger");

            Assert.Equal("one", first);
            Assert.Equal("one", second);

            Assert.Throws<System.Exception>(
                "ScenarioObject.StringMethodEcho(starts with m); Expected # 3, Actual # 2.",
                () => mock.VerifyExpectations());
        }

        [Fact]
        public void Verification_Throws_Exception_When_Expectations_Set_Against_Mock_Delegate_Are_Not_Met()
        {
            var mock = Repository.Mock<StringScenario>();
            mock.ExpectCall(x => x("mike", "meisinger"))
                .Return("one")
                .Repeat.Times(3);

            var first = mock("mike", "meisinger");
            var second = mock("mike", "meisinger");

            Assert.Equal("one", first);
            Assert.Equal("one", second);

            Assert.Throws<System.Exception>(() => mock.VerifyExpectations());
        }

        [Fact]
        public void Expectation_Throws_Exception_When_An_Exception_Is_Set()
        {
            var mock = Repository.Mock<IScenarioObject>();

            mock.ExpectCall(x => x.StringMethod())
                .Throws<System.InvalidTimeZoneException>();

            Assert.Throws<System.InvalidTimeZoneException>(() => mock.StringMethod());
        }
    }
}
