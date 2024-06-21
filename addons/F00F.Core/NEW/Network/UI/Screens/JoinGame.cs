using Godot;

namespace F00F.NEW;

[Tool]
public partial class JoinGame : HostJoin
{
    public ulong SessionId { get; set => this.Set(ref field, value, () => ValidateChange()); }

    public void TryJoin() => TryProceed();

    #region Private

    protected sealed override bool _CanProceed()
        => SessionId > 0;

    protected sealed override void OnProceed()
        => Network.ConnectClient(new(PlayerName, SessionId));

    #endregion
}
