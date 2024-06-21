using Godot;

namespace F00F.NEW;

[Tool]
public partial class HostGame : HostJoin
{
    #region Private

    private LineEdit _GameName => field ??= (LineEdit)GetNode("%GameName");
    private LineEdit _GameDesc => field ??= (LineEdit)GetNode("%GameDescription");
    private SessionScope _Scope => field ??= (SessionScope)GetNode("%SessionScope");
    private MaxPlayers _MaxPlayers => field ??= (MaxPlayers)GetNode("%MaxPlayers");

    #endregion

    public string GameName => _GameName.Text;
    public string GameDesc => _GameDesc.Text;
    public Network.SessionScope SessionScope => _Scope.Selected;
    public int MaxPlayers => _MaxPlayers.Value;

    #region Private

    protected sealed override void _LoadState()
    {
        _GameName.Text = cfg.Get(GameName);
        _GameDesc.Text = cfg.Get(GameDesc);
        _Scope.Selected = cfg.GetEnum(SessionScope);
        _MaxPlayers.Value = cfg.Get(MaxPlayers);
    }

    protected sealed override void _SaveState()
    {
        cfg.Set(GameName);
        cfg.Set(GameDesc);
        cfg.SetEnum(SessionScope);
        cfg.Set(MaxPlayers);
    }

    protected sealed override bool _CanProceed()
        => MaxPlayers > 0;

    protected sealed override void OnProceed()
        => Network.StartServer(
            new(PlayerName,
                GameName,
                GameDesc,
                SessionScope,
                MaxPlayers));

    #endregion
}
