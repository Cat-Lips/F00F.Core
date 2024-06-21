namespace F00F.NEW;

internal interface INetworkProtocol : INetworkCommon
{
    INetworkController Controller { get => null; set { } }
}
