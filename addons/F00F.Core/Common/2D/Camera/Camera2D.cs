using Godot;

namespace F00F;

[Tool]
public partial class Camera2D : Godot.Camera2D
{
    #region Private

    private Camera2DInput Input { get; } = new();

    #endregion

    #region Export

    [Export] public Camera2DConfig Config { get; set => this.Set(ref field, value ?? new()); }
    [Export] public Node2D Target { get; set => this.Set(ref field, value); }

    #endregion

    public Vector2 Velocity { get; private set; }

    #region Godot

    protected virtual void OnReady() { }
    public sealed override void _Ready()
    {
        Editor.Disable(this);
        Config ??= new();
        OnReady();
    }

    public sealed override void _Process(double _delta)
    {
        var delta = (float)_delta;
        var movement = Input.Movement();

        if (Target is null)
            FreeLook();
        else
            FollowTarget();

        void FreeLook()
        {
            UpdateVelocity();
            UpdatePosition();

            void UpdateVelocity()
            {
                Velocity = GetVelocity(Velocity);

                Vector2 GetVelocity(Vector2 velocity)
                {
                    velocity += movement * Config.Acceleration * delta;
                    if (movement.X is 0) velocity.X = Calc.BetterLerp(velocity.X, 0, Config.Deceleration * delta);
                    if (movement.Y is 0) velocity.Y = Calc.BetterLerp(velocity.Y, 0, Config.Deceleration * delta);
                    velocity = velocity.Clamp(-Config.MaxSpeed, Config.MaxSpeed);
                    return velocity;
                }
            }

            void UpdatePosition()
            {
                Position = UpdatePosition(Position);

                Vector2 UpdatePosition(Vector2 position)
                {
                    SetPosition();
                    ClampPosition();
                    return position;

                    void SetPosition()
                        => position += Calc.BetterLerp(Vector2.Zero, Velocity, Config.Lerp * delta);

                    void ClampPosition()
                    {
                        var half = this.GetZoomSize() * .5f;

                        var top = LimitTop + half.Y;
                        var left = LimitLeft + half.X;
                        var right = LimitRight - half.X;
                        var bottom = LimitBottom - half.Y;

                        position.X = position.X.Clamp(left, right);
                        position.Y = position.Y.Clamp(top, bottom);
                    }
                }
            }
        }

        void FollowTarget()
            => Position = Target.Position;
    }

    #endregion
}
