namespace F00F.NEW;

public partial class SessionStatus : SlideBar
{
    private StatusBar StatusBar => field ??= (StatusBar)GetNode("Content/StatusBar");

    protected sealed override void OnReady()
    {
        OnTreeEntered();
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;

        void OnTreeEntered()
        {
            SetState();
            Network.Instance.StateChanged += SetState;
            Network.Instance.CombinedStatus += SetStatus;
        }

        void OnTreeExiting()
        {
            Network.Instance.StateChanged -= SetState;
            Network.Instance.CombinedStatus -= SetStatus;
        }

        void SetState()
        {
            Visible = Network.Instance.HasState;
            TitleText = Network.Instance.SessionType();
            TitleTextColor = Network.Instance.SessionStatus().FontColor() ?? default;
            TitleBackColor = Network.Instance.SessionStatus().BackColor() ?? default;
        }

        void SetStatus(Status status, string msg)
            => StatusBar.SetStatus(status, msg);
    }
}
