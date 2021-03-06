﻿#region license
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
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
    public class MultiMocks
    {
        [Fact]
        public void CanCreateAStrictMultiMockFromTwoInterfacesNonGeneric()
        {
            IDemo demo = MockRepository.MockMulti<IDemo>(new[] { typeof(IDisposable) });
            CanCreateAStrictMultiMockFromTwoInterfacesCommon(demo);
        }

        [Fact]
        public void CanCreateAStrictMultiMockFromTwoInterfacesGeneric()
        {
            IDemo demo = MockRepository.MockMulti<IDemo>(new[] { typeof(IDisposable) });
            CanCreateAStrictMultiMockFromTwoInterfacesCommon(demo);
        }

        private static void CanCreateAStrictMultiMockFromTwoInterfacesCommon(IDemo demo)
        {
            demo.Expect(x => x.ReturnIntNoArgs())
                .Return(1);

            IDisposable disposable = demo as IDisposable;
            
            Assert.NotNull(disposable);
            disposable.Expect(x => x.Dispose());
            
            Assert.Equal(1, demo.ReturnIntNoArgs());
            disposable.Dispose();

            demo.VerifyAllExpectations();
            // disposable.VerifyAllExpectations();
        }
        
        [Fact]
        public void ClearStrictCollectionAndDisposesItNonGeneric()
        {
            CollectionBase collection = MockRepository.PartialMulti<CollectionBase>(new[] { typeof(IDisposable) });

            ClearStrictCollectionAndDisposesItCommon(collection);
        }

        [Fact]
        public void ClearStrictCollectionAndDisposesItGeneric()
        {
            CollectionBase collection = MockRepository.PartialMulti<CollectionBase>(new[] { typeof(IDisposable) });

            ClearStrictCollectionAndDisposesItCommon(collection);
        }

        private static void ClearStrictCollectionAndDisposesItCommon(CollectionBase collection)
        {
            collection.Expect(x => x.Clear());

            IDisposable disposable = (IDisposable)collection;
            disposable.Expect(x => x.Dispose());

            CleanCollection(collection);

            collection.VerifyAllExpectations();
            //disposable.VerifyAllExpectations();
        }

        private static void CleanCollection(CollectionBase collection)
        {
            collection.Clear();
            IDisposable disposable = collection as IDisposable;
            if(disposable!=null)
                disposable.Dispose();
        }
        
        [Fact]
        public void CanCreateAStrictMultiMockFromClassAndTwoInterfacesNonGeneric()
        {
            XmlReader reader = MockRepository.PartialMulti<XmlReader>(new[] { typeof(ICloneable), typeof(IHasXmlNode) });

            CanCreateAStrictMultiMockFromClassAndTwoInterfacesCommon(reader);
        }

        [Fact]
        public void CanCreateAStrictMultiMockFromClassAndTwoInterfacesGeneric()
        {
            XmlReader reader = MockRepository.PartialMulti<XmlReader>(new[] { typeof(ICloneable), typeof(IHasXmlNode) });

            CanCreateAStrictMultiMockFromClassAndTwoInterfacesCommon(reader);
        }

        private static void CanCreateAStrictMultiMockFromClassAndTwoInterfacesCommon(XmlReader reader)
        {
            reader.Expect(x => x.AttributeCount)
                .Return(3);

            ICloneable cloneable = reader as ICloneable;
            Assert.NotNull( cloneable );

            cloneable.Expect(x => x.Clone())
                .Return(reader);

            IHasXmlNode hasXmlNode = reader as IHasXmlNode;
            Assert.NotNull( hasXmlNode );

            XmlNode node = new XmlDocument();
            hasXmlNode.Expect(x => x.GetNode())
                .Return(node);
            
            Assert.Equal( 3, reader.AttributeCount );
            Assert.Equal( node, hasXmlNode.GetNode() );

            Assert.Same( cloneable, cloneable.Clone() );

            reader.VerifyAllExpectations();
            // cloneable.VerifyAllExpectations();
            // hasXmlNode.VerifyAllExpectations();
        }

        [Fact]
        public void CanCreateAStrictMultiMockWithConstructorArgsNonGeneric()
        {
            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = MockRepository.Mock<IFormatProvider>();

            StringWriter mockedWriter = MockRepository.MockMulti<StringWriter>(
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );

            CommonConstructorArgsTest(stringBuilder, formatProvider, mockedWriter, MockType.Strict);
        }

        [Fact]
        public void CanCreateAStrictMultiMockWithConstructorArgsGeneric()
        {
            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = MockRepository.Mock<IFormatProvider>();

            StringWriter mockedWriter = MockRepository.MockMulti<StringWriter>(
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );

            CommonConstructorArgsTest(stringBuilder, formatProvider, mockedWriter, MockType.Strict);
        }

        [Fact]
        public void CanCreateADynamicMultiMockFromTwoInterfacesNonGeneric()
        {
            object o = MockRepository.MockMulti<IDemo>(new[] { typeof(IEditableObject) });

            IDemo demo = o as IDemo;
            IEditableObject editable = o as IEditableObject;

            CanCreateADynamicMultiMockFromTwoInterfacesCommon(demo, editable);
        }

        [Fact]
        public void CanCreateADynamicMultiMockFromTwoInterfacesGeneric()
        {
            IDemo demo = MockRepository.MockMulti<IDemo>(new[] { typeof(IEditableObject) });
            IEditableObject editable = demo as IEditableObject;

            CanCreateADynamicMultiMockFromTwoInterfacesCommon(demo, editable);
        }

        private static void CanCreateADynamicMultiMockFromTwoInterfacesCommon(IDemo demo, IEditableObject editable)
        {
            Assert.NotNull(demo);
            Assert.NotNull(editable);

            // Set expectation on one member on each interface
            demo.Expect(x => x.ReadOnly)
                .Return("foo");

            editable.Expect(x => x.BeginEdit());
            
            // Drive two members on each interface to check dynamic nature
            Assert.Equal("foo", demo.ReadOnly);
            demo.VoidNoArgs();

            editable.BeginEdit();
            editable.EndEdit();

            demo.VerifyAllExpectations();
            // editable.VerifyAllExpectations();
        }

        [Fact]
        public void CanCreateADynamicMultiMockWithConstructorArgsNonGeneric()
        {            
            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = MockRepository.Mock<IFormatProvider>();

            StringWriter mockedWriter = MockRepository.MockMulti<StringWriter>(
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );

            CommonConstructorArgsTest(stringBuilder, formatProvider, mockedWriter, MockType.Dynamic);
        }

        [Fact]
        public void CanCreateADynamicMultiMockWithConstructorArgsGeneric()
        {
            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = MockRepository.Mock<IFormatProvider>();

            StringWriter mockedWriter = MockRepository.MockMulti<StringWriter>(
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );

            CommonConstructorArgsTest(stringBuilder, formatProvider, mockedWriter, MockType.Dynamic);
        }

        [Fact]
        public void CanCreateAPartialMultiMockFromClassAndTwoInterfacesNonGeneric()
        {
            XmlReader reader = MockRepository.PartialMulti<XmlReader>(new[] { typeof(ICloneable), typeof(IHasXmlNode) });

            CanCreateAPartialMultiMockFromClassAndTwoInterfacesCommon(reader);
        }

        [Fact]
        public void CanCreateAPartialMultiMockFromClassAndTwoInterfacesGeneric()
        {
            XmlReader reader = MockRepository.PartialMulti<XmlReader>(new[] { typeof(ICloneable), typeof(IHasXmlNode) });

            CanCreateAPartialMultiMockFromClassAndTwoInterfacesCommon(reader);
        }

        private static void CanCreateAPartialMultiMockFromClassAndTwoInterfacesCommon(XmlReader reader)
        {
            reader.Expect(x => x.AttributeCount)
                .Return(3);

            ICloneable cloneable = reader as ICloneable;
            Assert.NotNull(cloneable);

            cloneable.Expect(x => x.Clone())
                .Return(reader);

            IHasXmlNode hasXmlNode = reader as IHasXmlNode;
            Assert.NotNull(hasXmlNode);

            XmlNode node = new XmlDocument();
            hasXmlNode.Expect(x => x.GetNode())
                .Return(node);
            
            Assert.Equal(3, reader.AttributeCount);
            Assert.Equal(node, hasXmlNode.GetNode());

            Assert.Same(cloneable, cloneable.Clone());

            reader.VerifyAllExpectations();
            // cloneable.VerifyAllExpectations();
            // hasXmlNode.VerifyAllExpectations();
        }
        
        [Fact]
        public void CanCreateAPartialMultiMockWithConstructorArgsNonGeneric()
        {
            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = MockRepository.Mock<IFormatProvider>();

            StringWriter mockedWriter = MockRepository.PartialMulti<StringWriter>(
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );

            CommonConstructorArgsTest(stringBuilder, formatProvider, mockedWriter, MockType.Partial);
        }

        [Fact]
        public void CanCreateAPartialMultiMockWithConstructorArgsGeneric()
        {
            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = MockRepository.Mock<IFormatProvider>();

            StringWriter mockedWriter = MockRepository.PartialMulti<StringWriter>(
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );
            
            CommonConstructorArgsTest(stringBuilder, formatProvider, mockedWriter, MockType.Partial);
        }
        
        [Fact]
        public void CannotMultiMockUsingClassesAsExtras()
        {
            Assert.Throws<ArgumentException>(
                () => MockRepository.PartialMulti<XmlReader>(new[] { typeof(XmlWriter) }));
        }
        
        public interface IMulti
        {
            void OriginalMethod1();
            void OriginalMethod2();
        }

        public class MultiClass : IMulti
        {
            // NON-virtual method
            public void OriginalMethod1() { }
            // VIRTUAL method
            public virtual void OriginalMethod2() { }
        }

        public interface ISpecialMulti : IMulti
        {
            void ExtraMethod();
        }

        [Fact]
        public void RepeatedInterfaceMultiMocks()
        {
            object o = MockRepository.PartialMulti<MultiClass>(new[] { typeof(ISpecialMulti) });

            Assert.True(o is MultiClass, "Object should be MultiClass");
            Assert.True(o is IMulti, "Object should be IMulti");
            Assert.True(o is ISpecialMulti, "Object should be ISpecialMulti");
        }
        
        private enum MockType { Strict, Dynamic, Partial }

        // Helper class to provide a common set of tests for constructor-args based
        // multi-mocks testing.  Exercises a mocked StringWriter (which should also be an IDataErrorInfo)
        // constructed with a mocked IFormatProvider.  The test checks the semantics
        // of the mocked StringWriter to compare it with the expected semantics.
        private static void CommonConstructorArgsTest(StringBuilder stringBuilder, IFormatProvider formatProvider, StringWriter mockedWriter, MockType mockType)
        {
            string stringToWrite = "The original string";
            string stringToWriteLine = "Extra bit";

            IDataErrorInfo errorInfo = mockedWriter as IDataErrorInfo;
            Assert.NotNull(errorInfo);

            // Configure expectations for mocked writer
            mockedWriter.Expect(x => x.FormatProvider)
                .CallOriginalMethod();

            mockedWriter.Expect(x => x.Write((string)null))
                .IgnoreArguments()
                .CallOriginalMethod();

            mockedWriter.Expect(x => x.Flush())
                .Repeat.Any()
                .CallOriginalMethod();

            mockedWriter.Expect(x => x.Close());

            errorInfo.Expect(x => x.Error)
                .Return(null);

            errorInfo.Expect(x => x.Error)
                .Return("error!!!");
            
            // Ensure that arguments arrived okay
            // Is the format provider correct
            Assert.Same(formatProvider, mockedWriter.FormatProvider);
            // Does writing to the writer forward to our stringbuilder from the constructor?
            mockedWriter.Write(stringToWrite);
            mockedWriter.Flush();

            // Let's see what mode our mock is running in.
            // We have not configured WriteLine at all, so:
            //  a) if we're running as a strict mock, it'll fail
            //  b) if we're running as a dynamic mock, it'll no-op
            //  c) if we're running as a partial mock, it'll work
            try
            {
                mockedWriter.WriteLine(stringToWriteLine);
            }
            catch (ExpectationViolationException)
            {
                Assert.Equal(MockType.Strict, mockType);
            }

            string expectedStringBuilderContents = null;
            switch (mockType)
            {
                case MockType.Dynamic:
                case MockType.Strict:
                    expectedStringBuilderContents = stringToWrite;
                    break;
                case MockType.Partial:
                    expectedStringBuilderContents = stringToWrite + stringToWriteLine + Environment.NewLine;
                    break;
            }

            Assert.Equal(expectedStringBuilderContents, stringBuilder.ToString());

            mockedWriter.Close();
            Assert.Null(errorInfo.Error);
            Assert.Equal("error!!!", errorInfo.Error);

            if (MockType.Strict != mockType)
            {
                mockedWriter.VerifyAllExpectations();
                //errorInfo.VerifyAllExpectations();
            }
        }
    }
}
