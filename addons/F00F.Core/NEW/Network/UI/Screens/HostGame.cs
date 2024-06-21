using Godot;

namespace F00F.NEW;

[Tool]
public partial class HostGame : HostJoin
{
    #region Private

    private LineEdit _SessionName => field ??= (LineEdit)GetNode("%SessionName");
    private SessionScopeEnum _Scope => field ??= (SessionScopeEnum)GetNode("%SessionScope");
    private MaxPlayers _MaxPlayers => field ??= (MaxPlayers)GetNode("%MaxPlayers");

    #endregion

    public string SessionName => _SessionName.Text;
    public Network.SessionScope SessionScope => _Scope.Selected;
    public int MaxPlayers => _MaxPlayers.Value;

    #region Private

    protected sealed override void _LoadState()
    {
        _SessionName.Text = cfg.Get(SessionName);
        _Scope.Selected = cfg.GetEnum(SessionScope);
        _MaxPlayers.Value = cfg.Get(MaxPlayers);
    }

    protected sealed override void _SaveState()
    {
        cfg.Set(SessionName);
        cfg.SetEnum(SessionScope);
        cfg.Set(MaxPlayers);
    }

    protected sealed override bool _CanProceed()
        => MaxPlayers > 1;

    protected sealed override void OnProceed()
        => Network.Instance.StartServer(
            new(PlayerName,
                SessionName,
                SessionScope,
                MaxPlayers));

    #endregion
}
