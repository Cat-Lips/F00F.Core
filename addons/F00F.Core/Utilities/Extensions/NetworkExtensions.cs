using Godot;

namespace F00F;

public static class NetworkExtensions
{
    public static int PeerId(this Node source)
        => source.Multiplayer.GetUniqueId();

    public static int AuthId(this Node source)
        => source.GetMultiplayerAuthority();

    public static bool IsLocal(this Node source)
        => source.IsMultiplayerAuthority();

    public static bool IsRemote(this Node source)
        => !source.IsMultiplayerAuthority();

    public static bool IsServer(this Node source)
        => source.AuthId() is Network.ServerAuthority;

    public static void SetAuthId(this Node source, int pid)
        => source.SetMultiplayerAuthority(pid);

    public static Error RpcId(this Node source, StringName method, params Variant[] @args)
        => source.RpcId(source.AuthId(), method, args);

    private static long id;
    public static void SetSafeName<T>(this T source, int peer, string name = null) where T : Node
        => source.Name = $"{peer}.{name ?? typeof(T).Name}.{++id}";

    public static bool IsLocal(this Node source, int pid)
        => pid == source.PeerId();

    public static bool IsRemote(this Node source, int pid)
        => pid != source.PeerId();

    public static bool IsServer(this Node source, int pid)
        => pid is Network.ServerAuthority;
}
