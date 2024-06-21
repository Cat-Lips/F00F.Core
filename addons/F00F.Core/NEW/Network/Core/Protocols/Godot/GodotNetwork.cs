using Godot;

namespace F00F.NEW;

[Tool]
internal partial class GodotNetwork : Node, INetworkProtocol
{
    public bool Exists => true;

    private GodotPeerExtension GodotPeer => field ??= new();

    public INetworkController Controller { get; set => this.Set(ref field, value, OnControllerSet); }

    private void OnControllerSet()
    {
        if (Controller.IsNull())
        {
            Network.Instance.StateChanged -= OnStateChanged;
            GodotPeer.Controller = null;
            ResetPeer();
        }
        else
        {
            Network.Instance.StateChanged += OnStateChanged;
            GodotPeer.Controller = Controller;
            OnStateChanged();
        }

        void OnStateChanged()
        {
            if (Network.Instance.IsActive)
                AssignPeer();
            else ResetPeer();
        }

        void AssignPeer()
        {
            if (Network.Instance.Multiplayer.MultiplayerPeer != GodotPeer)
                Network.Instance.Multiplayer.MultiplayerPeer = GodotPeer;
        }

        void ResetPeer()
        {
            if (Network.Instance.Multiplayer.MultiplayerPeer == GodotPeer)
                Network.Instance.Multiplayer.MultiplayerPeer = null;
        }
    }
}
