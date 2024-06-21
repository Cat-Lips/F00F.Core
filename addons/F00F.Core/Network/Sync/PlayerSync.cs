using Godot;

namespace F00F;

public partial class PlayerSync : MultiplayerSynchronizer
{
    public sealed override void _Ready()
    {
        var player = GetParent();
        var local = player.IsLocal();
        this.CallDeferred(() => player.SetPhysicsProcess(local));
    }
}
