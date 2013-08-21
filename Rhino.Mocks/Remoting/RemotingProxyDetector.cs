
namespace Rhino.Mocks.Remoting
{
    internal class RemotingProxyDetector : IRemotingProxyOperation
    {
        internal bool Detected { get; private set; }
        
        public void Process(RemotingProxy proxy)
        {
            Detected = true;
        }
    }
}
