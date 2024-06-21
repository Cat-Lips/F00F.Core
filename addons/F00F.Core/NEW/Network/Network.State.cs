namespace F00F.NEW;

public enum NetworkState
{
    NoConnection,

    StartingServer,
    ServerActive,
    ServerError,

    ClientConnecting,
    ClientConnected,
    ClientError,

    ClientDisconnected,
}
