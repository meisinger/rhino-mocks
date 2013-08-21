using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Remoting
{
    internal class RemotingProxySelector : IRemotingProxyOperation
    {
        internal IMockInstance MockInstance { get; private set; }

        public void Process(RemotingProxy proxy)
        {
            MockInstance = proxy.MockInstance;
        }
    }
}
