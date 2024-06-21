using Godot;

namespace F00F;

public static class NetworkExtensions
{
    public static int PeerId(this Node source)
        => source.Multiplayer.GetUniqueId();

    public static int AuthId(this Node source)
        => source.GetMultiplayerAuthority();

    public static bool IsAuth(this Node source)
        => source.IsMultiplayerAuthority();

    public static void SetAuthId(this Node source, int pid)
        => source.SetMultiplayerAuthority(pid);

    public static Error RpcId(this Node source, StringName method, params Variant[] @args)
        => source.RpcId(source.AuthId(), method, args);
}
