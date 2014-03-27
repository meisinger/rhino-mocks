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
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using Xunit;
using Rhino.Mocks.Exceptions;
using System.Security.Policy;

namespace Rhino.Mocks.Tests.Remoting
{

	/// <summary>
	/// Test scenarios where mock objects are called from different
	/// application domain.
	/// </summary>
	public class ContextSwitchTests : IDisposable
	{
		private AppDomain otherDomain;
		private ContextSwitcher contextSwitcher;

		public ContextSwitchTests()
		{
            var assemblyName = Assembly.GetExecutingAssembly().FullName;
            var typeName = typeof(ContextSwitcher).FullName;

            var info = new AppDomainSetup();
            info.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            info.ApplicationName = Guid.NewGuid().ToString();

            otherDomain = AppDomain.CreateDomain(info.ApplicationName, null, info, 
                new PermissionSet(PermissionState.Unrestricted), new StrongName[0]);

            contextSwitcher = (ContextSwitcher)otherDomain.CreateInstanceAndUnwrap(assemblyName,
                typeName, false, BindingFlags.Default, null, new object[0], null, null, null);
		}

        public void Dispose()
		{
			AppDomain.Unload(otherDomain);
		}

		[Fact]
		public void MockInterface()
		{
			IDemo demo = Repository.Mock<IDemo>();

            demo.Expect(x => x.ReturnIntNoArgs())
                .Return(54);

            demo.Expect(x => x.VoidStringArg("54"));
			
			contextSwitcher.DoStuff(demo);
            demo.VerifyAllExpectations();
		}

		[Fact]
		public void MockInterfaceWithSameName()
		{
			IDemo demo = Repository.Mock<IDemo>();
            Other.IDemo remotingDemo = Repository.Mock<Other.IDemo>();

            demo.Expect(x => x.ReturnIntNoArgs())
                .Return(54);

            demo.Expect(x => x.VoidStringArg("54"));

            remotingDemo.Expect(x => x.ProcessString("in"));
			
			contextSwitcher.DoStuff(demo);
			contextSwitcher.DoStuff(remotingDemo);

            demo.VerifyAllExpectations();
            remotingDemo.VerifyAllExpectations();
		}

		[Fact]
		public void MockInterfaceExpectException()
		{
            IDemo demo = Repository.Mock<IDemo>();

            demo.Expect(x => x.ReturnIntNoArgs())
                .Throws<InvalidOperationException>();

			Assert.Throws<InvalidOperationException>(
				() => contextSwitcher.DoStuff(demo));
		}

		[Fact]
		public void MockInterfaceUnexpectedCall()
		{
			IDemo demo = Repository.Mock<IDemo>();

            demo.Expect(x => x.ReturnIntNoArgs())
                .Return(34);

            demo.Expect(x => x.VoidStringArg("bang"));
            contextSwitcher.DoStuff(demo);
			
			Assert.Throws<ExpectationViolationException>(
				() => demo.VerifyExpectations(true));
		}

		[Fact]
		public void MockClass()
		{
			RemotableDemoClass demo = Repository.Mock<RemotableDemoClass>();

            demo.Expect(x => x.Two())
                .Return(44);

			Assert.Equal(44, contextSwitcher.DoStuff(demo));
            demo.VerifyAllExpectations();
		}

		public void MockClassExpectException()
		{
            RemotableDemoClass demo = Repository.Mock<RemotableDemoClass>();

            demo.Expect(x => x.Two())
                .Throws<InvalidOperationException>();

			Assert.Throws<InvalidOperationException>(
				() => contextSwitcher.DoStuff(demo));
		}

		[Fact]
		public void MockClassUnexpectedCall()
		{
            RemotableDemoClass demo = Repository.Mock<RemotableDemoClass>();

            demo.Expect(x => x.Prop)
                .Return(11);

            contextSwitcher.DoStuff(demo);

			Assert.Throws<ExpectationViolationException>(
				() => demo.VerifyExpectations(true));
		}
	}
}
