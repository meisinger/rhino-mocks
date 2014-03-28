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
using Xunit;

namespace Rhino.Mocks.Tests
{
    public interface ISongBird
    {
        void Eat(string type, int quantity);
        string Sing();
        void Mate(ISongBird songBird);
        void MoveToCage(Cage cage);
    }

    public class Cage
    {
    }

    public class BirdVeterinary
    {
        private readonly Cage cage;

        public BirdVeterinary()
        {
            cage = new Cage();
        }

        public void Mate(ISongBird male, ISongBird female)
        {
            female.MoveToCage(cage);
            male.MoveToCage(cage);

            male.Eat("seeds", 250);
            female.Eat("seeds", 250);

            male.Mate(female);
            female.Mate(male);
        }
    }

    public abstract class ProcessorBase
    {
        public int Register;
        
        public virtual int Inc()
        {
            Register = Add(1);
            return Register;
        }

        public abstract int Add(int i);
    }
    
    public class IntegrationTests
    {
        public delegate bool CageDelegate(Cage cage);

        public Cage recordedCage;

        [Fact]
        public void UsingPartialMocks()
        {
            ProcessorBase proc = MockRepository.Partial<ProcessorBase>();

            proc.Expect(x => x.Add(1))
                .Return(1);

            proc.Expect(x => x.Add(1))
                .Return(2);

            proc.Inc();
            Assert.Equal(1, proc.Register);

            proc.Inc();
            Assert.Equal(2, proc.Register);

            proc.VerifyAllExpectations();
        }
        
        //[Fact]
        //public void ExampleUsingCallbacks()
        //{
        //    ISongBird maleBird = Repository.Mock<ISongBird>();
        //    ISongBird femaleBird = Repository.Mock<ISongBird>();

        //    maleBird.Expect(x => x.MoveToCage(null))
        //        .Callback(new CageDelegate(IsSameCage));

        //    femaleBird.Expect(x => x.MoveToCage(null))
        //        .Callback(new CageDelegate(IsSameCage));

        //    maleBird.Expect(x => x.Eat("seeds", 250));
        //    femaleBird.Expect(x => x.Eat("seeds", 250));

        //    maleBird.Expect(x => x.Mate(femaleBird));
        //    femaleBird.Expect(x => x.Mate(maleBird));

        //    BirdVeterinary vet = new BirdVeterinary();
        //    vet.Mate(maleBird, femaleBird);

        //    maleBird.VerifyAllExpectations();
        //    femaleBird.VerifyAllExpectations();
        //}

        [Fact]
        public void ExampleUsingParameterMatchingAndConstraints()
        {
            ISongBird bird = MockRepository.Mock<ISongBird>();

            bird.Expect(x => x.Eat("seeds", 500));

            bird.Expect(x => x.Sing())
                .Return("Chirp, Chirp");

            bird.Expect(x => x.Sing())
                .Throws(new Exception("No food, no song"));

            bird.Eat("seeds", 500);
            Assert.Equal("Chirp, Chirp", bird.Sing());

            try
            {
                bird.Sing();
                Assert.False(true, "Exception expected");
            }
            catch (Exception e)
            {
                Assert.Equal("No food, no song", e.Message);
            }
            
            bird.VerifyAllExpectations();
        }

        [Fact]
        public void UnorderedExecutionOfOrderedSequence()
        {
            ISongBird maleBird = MockRepository.Mock<ISongBird>();
            ISongBird femaleBird = MockRepository.Mock<ISongBird>();

            maleBird.Expect(x => x.Eat("seeds", 250));
            femaleBird.Expect(x => x.Eat("seeds", 250));

            maleBird.Expect(x => x.Mate(femaleBird));
            femaleBird.Expect(x => x.Mate(maleBird));

            maleBird.Mate(femaleBird);
            femaleBird.Mate(maleBird);

            maleBird.Eat("seeds", 250);
            femaleBird.Eat("seeds", 250);

            maleBird.VerifyAllExpectations();
            femaleBird.VerifyAllExpectations();
        }

        [Fact]
        public void OrderedExecutionOfUnorderedSequence()
        {
            ISongBird maleBird = MockRepository.Mock<ISongBird>();
            ISongBird femaleBird = MockRepository.Mock<ISongBird>();

            maleBird.Expect(x => x.Eat("seeds", 250));
            femaleBird.Expect(x => x.Eat("seeds", 250));

            maleBird.Expect(x => x.Mate(femaleBird));
            femaleBird.Expect(x => x.Mate(maleBird));

            femaleBird.Eat("seeds", 250);
            maleBird.Eat("seeds", 250);

            femaleBird.Mate(maleBird);
            maleBird.Mate(femaleBird);

            maleBird.VerifyAllExpectations();
            femaleBird.VerifyAllExpectations();
        }

        [Fact]
        public void SetupResultWithNestedOrdering()
        {
            ISongBird maleBird = MockRepository.Mock<ISongBird>();
            ISongBird femaleBird = MockRepository.Mock<ISongBird>();

            maleBird.Stub(x => x.Sing())
                .Return("");

            maleBird.Expect(x => x.Eat("seeds", 250));
            femaleBird.Expect(x => x.Eat("seeds", 250));

            maleBird.Expect(x => x.Mate(femaleBird));
            femaleBird.Expect(x => x.Mate(maleBird));

            maleBird.Sing();
            femaleBird.Eat("seeds", 250);
            maleBird.Sing();
            maleBird.Eat("seeds", 250);

            maleBird.Sing();
            femaleBird.Mate(maleBird);
            maleBird.Sing();
            maleBird.Mate(femaleBird);
            maleBird.Sing();

            maleBird.VerifyAllExpectations();
            femaleBird.VerifyAllExpectations();
        }

        private bool IsSameCage(Cage cageFromCallback)
        {
            //Can do any sort of valiation here
            if (this.recordedCage == null)
            {
                this.recordedCage = cageFromCallback;
                return true;
            }
            return this.recordedCage == cageFromCallback;
        }
    }
}
