
using Xunit;

namespace Rhino.Mocks.Tests
{
	
	public class WhenCalledTests
	{
		[Fact]
		public void Shortcut_to_arg_is_equal()
		{
			// minor hack to get this to work reliably, we reset the arg manager,
			// and restore on in the MockRepository ctor, so we do it this way
			new Repository();
			Assert.Equal(Arg.Is(1), Arg<int>.Is.Equal(1));
		}

		[Fact]
		public void Can_use_when_called_to_exceute_code_when_exceptation_is_matched_without_stupid_delegate_sig_overhead()
		{
			var wasCalled = false;
			var stub = Repository.Mock<IDemo>();

			stub.Stub(x => x.StringArgString(Arg.Is("")))
				.Return("blah")
				.WhenCalled(delegate { wasCalled = true; });

			Assert.Equal("blah", stub.StringArgString(""));
			Assert.True(wasCalled);
		}

		[Fact]
		public void Can_modify_return_value()
		{
			var stub = Repository.Mock<IDemo>();
			stub.Stub(x => x.StringArgString(Arg.Is("")))
				.Return("blah")
				.Intercept(x => x.ReturnValue = "arg");

			Assert.Equal("arg", stub.StringArgString(""));
		}

		[Fact]
		public void Can_inspect_method_arguments()
		{
			var stub = Repository.Mock<IDemo>();

			stub.Stub(x => x.StringArgString(null))
				.IgnoreArguments()
				.Return("blah")
                .Intercept(x => Assert.Equal("foo", x.Arguments[0]));

			Assert.Equal("blah", stub.StringArgString("foo"));
		}
	}
}