using Godot;

namespace F00F;

[Tool]
public partial class PortEdit : SpinBox
{
    public new int Value
    {
        get => (int)base.Value;
        set => base.Value = value;
    }

    public PortEdit()
    {
        MinValue = Network.MinPort;
        MaxValue = Network.MaxPort;
        Value = Network.GamePort;
    }
}
