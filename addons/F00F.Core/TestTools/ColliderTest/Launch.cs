using System;
using System.Diagnostics;
using Godot;

namespace F00F;

using Camera = Godot.Camera3D;
using CustomScene = (Node3D Body, float Mass, float Radius);

[Tool]
public partial class Launch : Node
{
    #region Private

    private readonly Stopwatch Timer = new();

    private Node Parent => field ??= GetParent();
    private Camera Camera => field ??= GetViewport().GetCamera3D();

    #endregion

    public event Action<Node3D> BodyAdded;
    public event Action<Node3D> BodyRemoved;

    public int BodyCount { get; private set; }

    #region Export

    [Export] public int Force { get; set => this.Set(ref field, value.ClampMin(0)); } = 10;
    [Export] public bool UseCCD { get; set; }

    #endregion

    public Func<CustomScene> Scene { get; set; }

    #region Godot

    public sealed override void _Ready()
    {
        MyInput.ActiveChanged += StopLaunchTimer;
        if (Editor.IsEditor) return;
    }

    public sealed override void _UnhandledInput(InputEvent e)
    {
        if (MyInput.Active && e is InputEventMouseButton mouse)
        {
            if (mouse.ButtonIndex is MouseButton.Left)
            {
                if (mouse.Pressed)
                    StartLaunchTimer();
                else LaunchNow();

                this.Handled();
            }
        }
    }

    #endregion

    #region Private

    private void StartLaunchTimer()
        => Timer.Restart();

    private void StopLaunchTimer()
        => Timer.Stop();

    private void LaunchNow()
    {
        if (!Timer.IsRunning) return;
        Timer.Stop();

        Init(Scene?.Invoke());

        void Init(CustomScene? scene)
        {
            if (scene is null) return;
            var (body, mass, radius) = scene.Value;

            body.Transform = Camera.Transform.TranslatedLocal(Vector3.Forward * radius);

            body.TreeEntered += () => ++BodyCount;
            body.TreeExiting += () => --BodyCount;

            body.TreeEntered += () => BodyAdded?.Invoke(body);
            body.TreeExiting += () => BodyRemoved?.Invoke(body);

            Parent.AddChild(body);

            if (body is RigidBody3D _body) ApplyForce(_body);
            else if (body is CharacterBody3D player) AddVelocity(player);

            void ApplyForce(RigidBody3D body)
            {
                if (UseCCD) body.ContinuousCd = true;
                body.ApplyCentralImpulse(body.Fwd() * Force());
            }

            void AddVelocity(CharacterBody3D player)
                => player.Velocity += player.Fwd() * Force();

            float Force()
            {
                var force = (float)Timer.Elapsed.TotalSeconds * this.Force;
                if (MyInput.IsKeyPressed(Key.Alt)) force *= this.Force;
                if (MyInput.IsKeyPressed(Key.Ctrl)) force *= this.Force;
                if (MyInput.IsKeyPressed(Key.Shift)) force *= this.Force;
                return force * mass;
            }
        }
    }

    #endregion
}
