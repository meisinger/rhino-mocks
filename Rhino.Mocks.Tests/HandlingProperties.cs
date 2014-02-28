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
using System.Text;
using Xunit;

namespace Rhino.Mocks.Tests
{
    public class HandlingProperties
    {
        IDemo demo;
        
		public HandlingProperties()
        {
            demo = Repository.Mock<IDemo>();
        }

        [Fact]
        public void PropertyBehaviorForSingleProperty()
        {
            //demo.Expect(x => x.Prop)
            //    .PropertyBehavior();

            // property behavior should be automatic
            for (int i = 0; i < 49; i++)
            {
                demo.Prop = "ayende" + i;
                Assert.Equal("ayende" + i, demo.Prop);
            }

            demo.VerifyAllExpectations();
        }

        //[Fact]
        //public void ExceptionIfLastMethodCallIsNotProperty()
        //{
        //    Assert.Throws<InvalidOperationException>(
        //        "Last method call was not made on a setter or a getter",
        //        () => demo.Expect(x => x.EnumNoArgs())
        //                .PropertyBehavior());
        //}

        [Fact]
        public void ExceptionIfPropHasOnlyGetter()
        {
            Assert.Throws<InvalidOperationException>(
                "Property must be read/write",
                () => demo.ExpectProperty(x => x.ReadOnly));
        }

        [Fact]
        public void ExceptionIfPropHasOnlySetter()
        {
        	Assert.Throws<InvalidOperationException>(
                "Property must be read/write",
                () => demo.ExpectProperty(x => x.WriteOnly));
        }

        [Fact]
        public void IndexedPropertiesSupported()
        {
            IWithIndexers with = Repository.Mock<IWithIndexers>();

            //with.Expect(x => x[1])
            //    .PropertyBehavior();

            //with.Expect(x => x["", 1])
            //    .PropertyBehavior();

            with[1] = 10;
            with[10] = 100;
            Assert.Equal(10, with[1]);
            Assert.Equal(100, with[10]);

            with["1", 2] = "3";
            with["2", 3] = "5";
            Assert.Equal("3", with["1", 2]);
            Assert.Equal("5", with["2", 3]);

            with.VerifyAllExpectations();
        }

        [Fact(Skip = "Test No Longer Valid")]
        public void IndexPropertyWhenValueTypeAndNotFoundThrows()
        {
            IWithIndexers with = Repository.Mock<IWithIndexers>();

            with.ExpectProperty(x => x[1]);

            Assert.Throws<InvalidOperationException>(
                "Can't return a value for property Item because no value was set and the Property return a value type.",
                () => GC.KeepAlive(with[1]));
        }

        [Fact]
        public void IndexPropertyWhenRefTypeAndNotFoundReturnNull()
        {
            IWithIndexers with = Repository.Mock<IWithIndexers>();

            with.ExpectProperty(x => x["", 3]);
            Assert.Null(with["", 2]);
        }

        public interface IWithIndexers
        {
            int this[int x] { get; set; }

            string this[string n, int y] { get; set; } 
        }
    }
}
