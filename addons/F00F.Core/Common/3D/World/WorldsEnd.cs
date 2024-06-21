using Godot;

namespace F00F;

[Tool]
public partial class WorldsEnd : StaticBody3D
{
    #region Export

    [Export] public int Size { get; set => this.Set(ref field, value.ClampMin(0), OnSizeChanged); } = 100;

    #endregion

    #region Private

    private Node3D WallLeft => field ??= GetNode<Node3D>("Left");
    private Node3D WallBack => field ??= GetNode<Node3D>("Back");
    private Node3D WallRight => field ??= GetNode<Node3D>("Right");
    private Node3D WallFront => field ??= GetNode<Node3D>("Front");

    private void OnSizeChanged()
    {
        var offset = Size * .5f;

        WallLeft.Position = Vector3.Left * offset;
        WallBack.Position = Vector3.Back * offset;
        WallRight.Position = Vector3.Right * offset;
        WallFront.Position = Vector3.Forward * offset;
    }

    #endregion
}
