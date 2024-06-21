namespace F00F;

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
