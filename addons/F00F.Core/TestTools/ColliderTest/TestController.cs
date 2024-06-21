using System;
using System.Linq;
using Godot;

namespace F00F;

using static ColliderTest.Enums;

public partial class TestController : Node
{
    private Node Parent => field ??= GetParent();
    private MeshInstance3D Mesh => field ??= Parent.GetNode<MeshInstance3D>("Mesh");
    private CollisionShape3D Shape => field ??= Parent.GetNode<CollisionShape3D>("Shape");

    public static ShapeType[] ShapeTypes { get; } = Enum.GetValues<ShapeType>().Except([ShapeType.Random]).ToArray();
    public static ShapeType TypeOf(ShapeType shape) => shape is ShapeType.Random ? ShapeTypes.PickRandom() : shape;

    private float size, life, mass;
    public void Init(ShapeType shape, float size, float life, float mass)
    {
        this.size = size;
        this.life = life;
        this.mass = mass;

        shape = TypeOf(shape);
        Mesh.Mesh = NewMesh(shape);
        Shape.Shape = NewShape(shape);

        InitPhysics();
        SetScaleAndMass();
    }

    #region Godot

    public sealed override void _Process(double _delta)
    {
        var delta = (float)_delta;

        if (IsAlive()) return;
        if (IsShrinking()) return;

        SetProcess(false);
        Parent.QueueFree();

        bool IsAlive()
            => (life -= delta) > 0;

        bool IsShrinking()
        {
            if ((size -= delta) > 0)
            {
                SetScaleAndMass();
                return true;
            }

            return false;
        }
    }

    #endregion

    #region Private

    private void InitPhysics()
    {
        if (Parent is RigidBody3D body)
        {
            body.PhysicsMaterialOverride ??= new()
            {
                Bounce = Rng.NextSingle(),
                Friction = Rng.NextSingle()
            };
        }
    }

    private void SetScaleAndMass()
    {
        Mesh.Scale = Vector3.One * size;
        Shape.Scale = Vector3.One * size;
        if (Parent is RigidBody3D body)
            body.Mass = Math.Max(mass * size, Const.Epsilon);
    }

    #endregion
}
