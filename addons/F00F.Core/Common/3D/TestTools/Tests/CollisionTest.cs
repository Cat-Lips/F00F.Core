using System.Diagnostics;
using Godot;

namespace F00F
{
    [Tool]
    public partial class CollisionTest : Node
    {
        private readonly Stopwatch Timer = new();

        #region Export

        [Export] public float Size { get; set; } = 1;
        [Export] public ShapeType Shape { get; set; } = ShapeType.Sphere;
        [Export] public float Duration { get; set; } = 10;
        [Export] public float MassMultiplier { get; set; } = 100;

        #endregion

        #region Godot

        public override void _UnhandledInput(InputEvent e)
        {
            if (e is InputEventMouseButton mouse)
            {
                if (mouse.ButtonIndex is MouseButton.Left)
                {
                    if (mouse.Pressed)
                        Timer.Restart();
                    else LaunchBody();
                }
            }

            void LaunchBody()
            {
                Timer.Stop();

                NewBody(out var body);
                PlaceBody(out var fwd);
                ApplyForce();

                void NewBody(out TestBody body)
                    => body = Utils.New<TestBody>(x => x.Init(Shape, Size, MassMultiplier, Duration));

                void PlaceBody(out Vector3 fwd)
                {
                    var source = GetViewport().GetCamera3D();
                    source.AddChild(body);
                    body.Position = Size * (fwd = source.Fwd());
                    body.Reparent(this);
                }

                void ApplyForce()
                {
                    var force = Timer.ElapsedTicks / body.Mass;
                    if (Input.IsKeyLabelPressed(Key.Alt)) force *= 10;
                    if (Input.IsKeyLabelPressed(Key.Ctrl)) force *= 10;
                    if (Input.IsKeyLabelPressed(Key.Shift)) force *= 10;
                    body.ApplyCentralForce(force * fwd);
                }
            }
        }

        #endregion
    }
}
