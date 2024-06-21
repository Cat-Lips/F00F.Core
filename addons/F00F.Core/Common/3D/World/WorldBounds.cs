using System;
using Godot;

namespace F00F;

[Tool]
public partial class WorldBounds : StaticBody3D
{
    public event Action SizeChanged;

    #region Export

    [Export(PropertyHint.Link)] public Vector2 Size { get; set => this.Set(ref field, value.ClampMin(0), OnSizeChanged, SizeChanged); } = new(100, 100);

    #endregion

    #region Private

    private Node3D WallLeft => field ??= GetNode<Node3D>("Left");
    private Node3D WallBack => field ??= GetNode<Node3D>("Back");
    private Node3D WallRight => field ??= GetNode<Node3D>("Right");
    private Node3D WallFront => field ??= GetNode<Node3D>("Front");

    private void OnSizeChanged()
    {
        var offset = Size * .5f;

        WallLeft.Position = Vector3.Left * offset.X;
        WallBack.Position = Vector3.Back * offset.Y;
        WallRight.Position = Vector3.Right * offset.X;
        WallFront.Position = Vector3.Forward * offset.Y;
    }

    #endregion
}
