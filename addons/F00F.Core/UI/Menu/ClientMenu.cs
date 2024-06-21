using Godot;

namespace F00F;

[Tool]
public partial class ClientMenu : StatusMenu
{
    private Button CreateClient => field ??= GetNode<Button>("%CreateClient");
    private Button CloseClient => field ??= GetNode<Button>("%CloseClient");
    private LineEdit ConnectAddress => field ??= GetNode<LineEdit>("%ConnectAddress");
    private PortEdit ConnectPort => field ??= GetNode<PortEdit>("%ConnectPort");

    public ClientMenu()
        => Visible = Editor.IsEditor;

    public void Initialise(Network network)
    {
        SetState();

        network.StateChanged += SetState;
        network.ClientStatus += SetStatus;

        CreateClient.Pressed += OnCreateClient;
        CloseClient.Pressed += OnCloseClient;

        void SetState()
        {
            var active = network.IsActive;

            Visible = !active || network.IsClient;

            CreateClient.Visible = !active;
            CloseClient.Visible = active;

            var editable = !active;

            ConnectAddress.Editable = editable;
            ConnectPort.Editable = editable;

            if (active && network.IsServer)
                ClearStatus();
        }

        void OnCreateClient()
        {
            ClearStatus();
            network.CreateClient(ConnectAddress.Text, ConnectPort.Value);
        }

        void OnCloseClient()
            => network.CloseClient();
    }
}
