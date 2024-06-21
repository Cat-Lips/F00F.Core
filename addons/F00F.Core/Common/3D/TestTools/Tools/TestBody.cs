using System;
using System.Linq;
using Godot;

namespace F00F;

[Tool]
public partial class TestBody : RigidBody3D
{
    #region Private

    private MeshInstance3D Mesh => field ??= GetNode<MeshInstance3D>("Mesh");
    private CollisionShape3D Shape => field ??= GetNode<CollisionShape3D>("Shape");

    private static readonly ShapeType[] ShapeTypes = Enum.GetValues<ShapeType>().Except([ShapeType.Random]).ToArray();
    private static ShapeType GetShapeType(ShapeType shape) => shape is ShapeType.Random ? ShapeTypes.GetRandom() : shape;

    #endregion

    public void Init(ShapeType shape, float size, float mass, float life)
    {
        this.size = size;
        this.mass = mass;
        this.life = life;

        Mesh.Mesh = NewMesh(shape);
        Shape.Shape = NewShape(shape);

        InitPhysics();
        SetScaleAndMass();
    }

    #region Godot

    public override void _Ready()
        => Editor.Disable(this);

    private float size, mass, life;
    public override void _Process(double _delta)
    {
        var delta = (float)_delta;

        if (IsAlive()) return;
        if (IsShrinking()) return;

        QueueFree();
        SetProcess(false);

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
        if (PhysicsMaterialOverride is null)
        {
            PhysicsMaterialOverride ??= new();
            PhysicsMaterialOverride.Bounce = Rng.NextSingle();
            PhysicsMaterialOverride.Friction = Rng.NextSingle();
        }
    }

    private void SetScaleAndMass()
    {
        var scale = Vector3.One * size;
        Mesh.Scale = scale;
        Shape.Scale = scale;
        Mass = Math.Max(mass * size, Const.Epsilon);
    }

    private static Mesh NewMesh(ShapeType shape) => shape switch
    {
        ShapeType.Cube => new BoxMesh(),
        ShapeType.Sphere => new SphereMesh(),
        ShapeType.Capsule => new CapsuleMesh(),
        ShapeType.Cylinder => new CylinderMesh(),
        _ => throw new NotImplementedException(),
    };

    private static Shape3D NewShape(ShapeType shape) => shape switch
    {
        ShapeType.Cube => new BoxShape3D(),
        ShapeType.Sphere => new SphereShape3D(),
        ShapeType.Capsule => new CapsuleShape3D(),
        ShapeType.Cylinder => new CylinderShape3D(),
        _ => throw new NotImplementedException(),
    };

    #endregion
}
