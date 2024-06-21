using System;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

using static ColliderTest.Enums;
using Camera = Godot.Camera3D;

[Tool]
public partial class ColliderTest : Node
{
    #region Private

    private readonly Stopwatch Timer = new();

    private Node Parent => field ??= GetParent();
    private Camera Camera => field ??= GetViewport().GetCamera3D();

    private int BodyCount { get; set; }

    #endregion

    #region Enums

    public static class Enums
    {
        public enum BodyType
        {
            Random,
            RigidBody,
            CharacterBody,
        }

        private static BodyType[] BodyTypes { get; } = [.. Enum.GetValues<BodyType>().Except([BodyType.Random])];
        public static BodyType TypeOf(BodyType body) => body is BodyType.Random ? BodyTypes.PickRandom() : body;

        public static Node3D NewBody(BodyType body) => body switch
        {
            BodyType.Random => NewBody(TypeOf(body)),
            BodyType.RigidBody => Utils.New<TestBody>(),
            BodyType.CharacterBody => Utils.New<TestPlayer>(),
            _ => throw new NotImplementedException(),
        };

        public enum ShapeType
        {
            Cube,
            Sphere,
            Capsule,
            Cylinder,
            Random
        }

        public static ShapeType[] ShapeTypes { get; } = [.. Enum.GetValues<ShapeType>().Except([ShapeType.Random])];
        public static ShapeType TypeOf(ShapeType shape) => shape is ShapeType.Random ? ShapeTypes.PickRandom() : shape;

        public static Mesh NewMesh(ShapeType shape) => shape switch
        {
            ShapeType.Cube => new BoxMesh(),
            ShapeType.Sphere => new SphereMesh(),
            ShapeType.Capsule => new CapsuleMesh(),
            ShapeType.Cylinder => new CylinderMesh(),
            _ => throw new NotImplementedException(),
        };

        public static Shape3D NewShape(ShapeType shape) => shape switch
        {
            ShapeType.Cube => new BoxShape3D(),
            ShapeType.Sphere => new SphereShape3D(),
            ShapeType.Capsule => new CapsuleShape3D(),
            ShapeType.Cylinder => new CylinderShape3D(),
            _ => throw new NotImplementedException(),
        };
    }

    #endregion

    public event Action ShapeChanged;
    public event Action BodyChanged;
    public event Action SizeChanged;
    public event Action LifeChanged;
    public event Action MassChanged;
    public event Action ForceChanged;
    public event Action CCDChanged;

    public event Action<Node3D> BodyAdded;
    public event Action<Node3D> BodyRemoved;

    #region Export

    [Export] public ShapeType Shape { get; set => this.Set(ref field, value, ShapeChanged); } = ShapeType.Sphere;
    [Export] public BodyType Body { get; set => this.Set(ref field, value, BodyChanged); } = BodyType.RigidBody;
    [Export(PropertyHint.Range, "1,10")] public int Size { get; set => this.Set(ref field, value.ClampMin(1), SizeChanged); } = 1;
    [Export(PropertyHint.Range, "1,100")] public int Life { get; set => this.Set(ref field, value.ClampMin(1), LifeChanged); } = 25;
    [Export(PropertyHint.Range, "1,1000")] public int Mass { get; set => this.Set(ref field, value.ClampMin(1), MassChanged); } = 100;
    [Export(PropertyHint.Range, "1,100")] public int Force { get; set => this.Set(ref field, value.ClampMin(1), ForceChanged); } = 10;
    [Export] public bool CCD { get; set => this.Set(ref field, value, CCDChanged); }

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        MyInput.ActiveChanged += StopLaunch;
        if (Editor.IsEditor) return;
        InitDebug();
    }

    public sealed override void _UnhandledInput(InputEvent e)
    {
        if (MyInput.Active && e is InputEventMouseButton mouse)
        {
            if (mouse.ButtonIndex is MouseButton.Left)
            {
                if (mouse.Pressed)
                    StartLaunch();
                else Launch();

                this.Handled();
            }
        }
    }

    #endregion

    #region Private

    private void StartLaunch()
        => Timer.Restart();

    private void StopLaunch()
        => Timer.Stop();

    private void Launch()
    {
        if (!Timer.IsRunning) return;
        Timer.Stop();

        Init(NewBody(Body));

        void Init(Node3D body)
        {
            ((TestController)body.GetNode("Controller")).Init(Shape, Size, Life, Mass);
            body.Transform = Camera.Transform.TranslatedLocal(Vector3.Forward * Size);

            body.TreeEntered += () => ++BodyCount;
            body.TreeExiting += () => --BodyCount;

            body.TreeEntered += () => BodyAdded?.Invoke(body);
            body.TreeExiting += () => BodyRemoved?.Invoke(body);

            Parent.AddChild(body);

            if (body is TestBody a) ApplyForce(a);
            else if (body is TestPlayer b) AddVelocity(b);

            void ApplyForce(TestBody body)
            {
                body.ContinuousCd = CCD;
                body.ApplyCentralImpulse(body.Fwd() * Force());
            }

            void AddVelocity(TestPlayer player)
                => player.Velocity += player.Fwd() * Force();

            float Force()
            {
                var force = (float)Timer.Elapsed.TotalSeconds * this.Force;
                if (MyInput.IsKeyPressed(Key.Alt)) force *= this.Force;
                if (MyInput.IsKeyPressed(Key.Ctrl)) force *= this.Force;
                if (MyInput.IsKeyPressed(Key.Shift)) force *= this.Force;
                return Mass * Size * force;
            }
        }
    }

    private void InitDebug()
    {
        ValueWatcher.Instance.Sep();
        ValueWatcher.Instance.Add(nameof(ColliderTest), () => $"({BodyCount})");
        ValueWatcher.Instance.Add(" - Shape", UI.EnumEdit(Shape, x => Shape = x));
        ValueWatcher.Instance.Add(" - Body", UI.EnumEdit(Body, x => Body = x));
        ValueWatcher.Instance.Add(" - Size", UI.ValueEdit(Size, x => Size = x, range: (1, 10, null)));
        ValueWatcher.Instance.Add(" - Life", UI.ValueEdit(Life, x => Life = x, range: (1, 100, null)));
        ValueWatcher.Instance.Add(" - Mass", UI.ValueEdit(Mass, x => Mass = x, range: (1, 1000, null)));
        ValueWatcher.Instance.Add(" - Force", UI.ValueEdit(Force, x => Force = x, range: (1, 100, null)));
        ValueWatcher.Instance.Add(" - CCD", UI.Toggle(CCD, x => CCD = x, hint: "Continuous Collision Detection"));
    }

    #endregion
}
