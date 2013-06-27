#region license

// Copyright (c) 2007 Ivan Krivyakov (ivan@ikriv.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
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

namespace Rhino.Mocks.Tests
{
    
    public class StrictMockTests
    {
        public class TestClass : MarshalByRefObject
        {
            public TestClass(string unused)
            {
                throw new InvalidCastException("Real method should never be called"); 
            }

            public void Method() 
            { 
                throw new InvalidCastException("Real method should never be called"); 
            }

            public int MethodReturningInt()
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public string MethodReturningString()
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public string MethodGettingParameters(int intParam, string stringParam)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public void MethodAcceptingTestClass(TestClass other)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public int GenericMethod<T>(string parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public T GenericMethodReturningGenericType<T>(string parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public T GenericMethodWithGenericParam<T>( T parameter )
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public string StringProperty
            {
                get
                {
                    throw new InvalidCastException("Real method should never be called");
                }
                set
                {
                    throw new InvalidCastException("Real method should never be called");
                }
            }
        }

        public class GenericTestClass<T> : MarshalByRefObject
        {
            public int Method(T parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public U GenericMethod<U>(T parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }
        }

        [Fact]
        public void CanMockVoidMethod()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.Method());
            
            t.Method();
            t.VerifyAllExpectations();
        }

        [Fact]
        public void ThrowOnUnexpectedVoidMethod()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));
            
        	Assert.Throws<ExpectationViolationException>(
        		"TestClass.Method(); Expected #0, Actual #1.",
				() => t.Method());
        }

        [Fact]
        public void CanMockMethodReturningInt()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.MethodReturningInt())
                .Return(42);

            Assert.Equal(42, t.MethodReturningInt());
            t.VerifyAllExpectations();
        }

        [Fact]
        public void CanMockMethodReturningString()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.MethodReturningString())
                .Return("foo");

            Assert.Equal("foo", t.MethodReturningString());
            t.VerifyAllExpectations();
        }

        [Fact]
        public void CanMockMethodGettingParameters()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.MethodGettingParameters(42, "foo"))
                .Return("bar");

            Assert.Equal("bar", t.MethodGettingParameters(42, "foo"));
            t.VerifyAllExpectations();
        }

        [Fact]
        public void CanRejectIncorrectParameters()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.MethodGettingParameters(42, "foo"))
                .Return("bar");

            Assert.Throws<ExpectationViolationException>(
        		"TestClass.MethodGettingParameters(19, \"foo\"); Expected #0, Actual #1.\r\nTestClass.MethodGettingParameters(42, \"foo\"); Expected #1, Actual #0.",
				() => t.MethodGettingParameters(19, "foo"));
        }

        [Fact]
        public void CanMockPropertyGet()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.StringProperty)
                .Return("foo");

            Assert.Equal("foo", t.StringProperty);
            t.VerifyAllExpectations();
        }

        [Fact]
        public void CanMockPropertySet()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.StringProperty = "foo");

            t.StringProperty = "foo";
            t.VerifyAllExpectations();
        }

        [Fact]
        public void CanRejectIncorrectPropertySet()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.StringProperty = "foo");

            Assert.Throws<ExpectationViolationException>(
        		"TestClass.set_StringProperty(\"bar\"); Expected #0, Actual #1.\r\nTestClass.set_StringProperty(\"foo\"); Expected #1, Actual #0.",
				() => t.StringProperty = "bar");
        }

        [Fact]
        public void CanMockGenericClass()
        {
            //GenericTestClass<string> t = (GenericTestClass<string>)MockRepository
            //    .GenerateStrictMock(typeof(GenericTestClass<string>));
            GenericTestClass<string> t = MockRepository.GenerateStrictMockWithRemoting<GenericTestClass<string>>();

            t.Expect(x => x.Method("foo"))
                .Return(42);

            Assert.Equal(42, t.Method("foo"));
            t.VerifyAllExpectations();
        }

        [Fact]
        public void CanMockGenericMethod()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.GenericMethod<string>("foo"))
                .Return(42);

            Assert.Equal(42, t.GenericMethod<string>("foo"));
            t.VerifyAllExpectations();
        }

		[Fact]
		public void CanMockGenericMethod_WillErrorOnWrongType()
		{
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.GenericMethod<string>("foo"))
                .Return(42);
			
			Assert.Throws<ExpectationViolationException>(
				@"TestClass.GenericMethod<System.Int32>(""foo""); Expected #1, Actual #1.
TestClass.GenericMethod<System.String>(""foo""); Expected #1, Actual #0.",
				() => Assert.Equal(42, t.GenericMethod<int>("foo")));
		}

        [Fact]
        public void CanMockGenericMethodReturningGenericType()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.GenericMethodReturningGenericType<string>("foo"))
                .Return("bar");

            Assert.Equal("bar", t.GenericMethodReturningGenericType<string>("foo"));
            t.VerifyAllExpectations();
        }

        [Fact]
        public void CanMockGenericMethodWithGenericParam()
        {
            TestClass t = MockRepository.GenerateStrictMockWithRemoting<TestClass>();
            //TestClass t = (TestClass)MockRepository.GenerateStrictMock(typeof(TestClass));

            t.Expect(x => x.GenericMethodWithGenericParam<string>("foo"))
                .Return("bar");

            Assert.Equal("bar", t.GenericMethodWithGenericParam("foo"));
            t.VerifyAllExpectations();
        }

        [Fact]
        public void CanMockGenericMethodInGenericClass()
        {
            GenericTestClass<string> t = MockRepository.GenerateStrictMock<GenericTestClass<string>>();

            t.Expect(x => x.GenericMethod<int>("foo"))
                .Return(42);

            Assert.Equal(42, t.GenericMethod<int>("foo"));
            t.VerifyAllExpectations();
        }

		[Fact]
		public void CanMockAppDomain()
		{
			AppDomain appDomain = MockRepository.GenerateStrictMock<AppDomain>();

            appDomain.Expect(x => x.BaseDirectory)
                .Return("/home/user/ayende");
			
			Assert.Equal(appDomain.BaseDirectory, "/home/user/ayende" );
            appDomain.VerifyAllExpectations();
		}

		[Fact]
    	public void NotCallingExpectedMethodWillCauseVerificationError()
    	{
			AppDomain appDomain = MockRepository.GenerateStrictMock<AppDomain>();

            appDomain.Expect(x => x.BaseDirectory)
                .Return("/home/user/ayende");

            Assert.Throws<ExpectationViolationException>(
                @"AppDomain.get_BaseDirectory(); Expected #1, Actual #0.",
                () => appDomain.VerifyAllExpectations());
    	}

        [Fact]
        public void CanMockMethodAcceptingTestClass()
        {
            TestClass t1 = MockRepository.GenerateStrictMock<TestClass>();
            TestClass t2 = MockRepository.GenerateStrictMock<TestClass>();

            t1.Expect(x => x.MethodAcceptingTestClass(t2));

            t1.MethodAcceptingTestClass(t2);
            t1.VerifyAllExpectations();
        }

        [Fact]
        // can't use ExpectedException since expected message is dynamic
        public void CanMockMethodAcceptingTestClass_WillErrorOnWrongParameter()
        {
            string t2Text = "@";
            string t3Text = "@";

            try
            {

                TestClass t1 = MockRepository.GenerateStrictMock<TestClass>();
                TestClass t2 = MockRepository.GenerateStrictMock<TestClass>();
                TestClass t3 = MockRepository.GenerateStrictMock<TestClass>();
                
                t2Text = t2.ToString();
                t3Text = t3.ToString();

                t1.Expect(x => x.MethodAcceptingTestClass(t2));

                t1.MethodAcceptingTestClass(t3);
                t1.VerifyAllExpectations();

                Assert.False(true, "Expected ExpectationViolationException");
            }
            catch (ExpectationViolationException ex)
            {
                string msg =
                    string.Format("TestClass.MethodAcceptingTestClass({0}); Expected #0, Actual #1.\r\n" +
                                  "TestClass.MethodAcceptingTestClass({1}); Expected #1, Actual #0.",
                                  t3Text, t2Text);

                Assert.Equal(msg, ex.Message);
            }
        }

        [Fact]
        public void StrictMockGetTypeReturnsMockedType()
        {
            TestClass t = MockRepository.GenerateStrictMock<TestClass>();
            Assert.Same(typeof(TestClass), t.GetType());
        }

        [Fact]
        public void StrictMockGetHashCodeWorks()
        {
            TestClass t = MockRepository.GenerateStrictMock<TestClass>();
            t.GetHashCode();
        }

        [Fact]
        public void StrictMockToStringReturnsDescription()
        {
            TestClass t = MockRepository.GenerateStrictMock<TestClass>();

            int hashCode = t.GetHashCode();
            string toString = t.ToString();
            Assert.Equal(String.Format("RemotingMock_{0}<TestClass>", hashCode), toString);
        }

        [Fact]
        public void StrictMockEquality()
        {
            TestClass t = MockRepository.GenerateStrictMock<TestClass>();

            Assert.False(t.Equals(null));
            Assert.False(t.Equals(42));
            Assert.False(t.Equals("foo"));
            Assert.True(t.Equals(t));
        }
    }
}
