using Godot;

namespace F00F;

[Tool]
public partial class ServerMenu : StatusMenu
{
    private Button StartServer => field ??= GetNode<Button>("%StartServer");
    private Button StopServer => field ??= GetNode<Button>("%StopServer");
    private LineEdit ServerAddress => field ??= GetNode<LineEdit>("%ServerAddress");
    private PortEdit ServerPort => field ??= GetNode<PortEdit>("%ServerPort");

    public ServerMenu()
        => Visible = Editor.IsEditor;

    public void Initialise(Network network)
    {
        var window = this.GetMainWindow();
        var title = window.Title;

        SetState();

        network.StateChanged += SetState;
        network.ServerStatus += SetStatus;

        StartServer.Pressed += OnStartServer;
        StopServer.Pressed += OnStopServer;

        void SetState()
        {
            var active = network.IsActive;

            Visible = !active || network.IsServer;

            StartServer.Visible = !active;
            StopServer.Visible = active;

            var editable = !active;

            ServerPort.Editable = editable;

            if (active && network.IsClient)
                ClearStatus();

            window.Title = network.IsServer
                ? $"{title} - *** HOST ***"
                : title;
        }

        void OnStartServer()
        {
            ClearStatus();
            network.StartServer(ServerPort.Value);
        }

        void OnStopServer()
            => network.StopServer();
    }
}
