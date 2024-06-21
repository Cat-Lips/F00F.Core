using Godot;

namespace F00F;

public partial class PlayerSync : MultiplayerSynchronizer
{
    public sealed override void _Ready()
    {
        var player = GetParent();
        var isAuth = player.IsAuth();
        this.CallDeferred(() => player.SetPhysicsProcess(isAuth));
    }
}
