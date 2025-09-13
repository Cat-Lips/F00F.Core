using System;
using Godot;

namespace F00F;

[Tool]
public partial class WorldBounds : StaticBody3D, IBounds
{
    public event Action SizeChanged;

    #region Export

    [Export(PropertyHint.Link)] public Vector2 Size { get; set => this.Set(ref field, value.ClampMin(0), OnSizeChanged); }

    #endregion

    #region Extras

    public float Left { get; private set; }
    public float Back { get; private set; }
    public float Right { get; private set; }
    public float Front { get; private set; }

    #endregion

    #region Private

    private Node3D WallLeft => field ??= GetNode<Node3D>("Left");
    private Node3D WallBack => field ??= GetNode<Node3D>("Back");
    private Node3D WallRight => field ??= GetNode<Node3D>("Right");
    private Node3D WallFront => field ??= GetNode<Node3D>("Front");

    private void OnSizeChanged()
    {
        this.OnReady(() =>
        {
            var offset = Size * .5f;

            WallLeft.Position = Vector3.Left * offset.X;
            WallBack.Position = Vector3.Back * offset.Y;
            WallRight.Position = Vector3.Right * offset.X;
            WallFront.Position = Vector3.Forward * offset.Y;

            Left = WallLeft.Position.X;
            Back = WallBack.Position.Z;
            Right = WallRight.Position.X;
            Front = WallFront.Position.Z;

            SizeChanged?.Invoke();
        });
    }

    #endregion
}
