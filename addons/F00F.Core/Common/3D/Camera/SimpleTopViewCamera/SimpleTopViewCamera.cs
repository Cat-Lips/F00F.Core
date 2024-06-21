using System;
using Godot;

namespace F00F;

[Tool]
public partial class SimpleTopViewCamera : Godot.Camera3D
{
    public event Action HeightChanged;

    [Export] public float Height { get; set => this.Set(ref field, value, OnHeightChanged, HeightChanged); } = 130;

    private void OnHeightChanged()
        => Position = Position.With(y: Height);

    public SimpleTopViewCamera()
    {
        Fov = 30;
        Position = Vector3.Up * Height;
        Rotation = Vector3.Left * -Const.Deg90;
    }
}
