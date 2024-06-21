using Godot;

namespace F00F;

public partial class Camera
{
    private int _speed = 1;

    private float _freeYaw;
    private float _freePitch;

    private void InitFreeCam()
        => (_freePitch, _freeYaw, _) = GlobalRotation;

    private bool OnFreeCamInput()
    {
        return
            this.Handle(FreeCamInput.SpeedUp(), OnSpeedUp) ||
            this.Handle(FreeCamInput.SlowDown(), OnSlowDown);

        void OnSpeedUp()
            => ++_speed;

        void OnSlowDown()
            => _speed = (_speed - 1).ClampMin(1);
    }

    private void DoFreeCam(float delta)
    {
        if (SelectMode) return;
        if (!Config.FreeCamEnabled) return;

        var move = GetLocalMovement();
        var rot = UpdateGlobalRotation(MyInput.Instance.MouseDelta);

        var gpos = GlobalPosition;
        var basis = Basis.FromEuler(rot);
        var gform = new Transform3D(basis, gpos + basis * move);

        if (MyShape.NotNull())
        {
            MyWorld.SafeMoveAndSlide(MyShape, gform.Basis, gpos, ref gform.Origin, exclude: Target);
            GlobalTransform = gform;
        }
        else GlobalTransform = gform;

        Vector3 UpdateGlobalRotation(in Vector2 movement)
        {
            if (movement.NotZeroExact())
            {
                _freeYaw -= movement.X * Config.FreeCamSensitivity;
                _freePitch -= movement.Y * Config.FreeCamSensitivity * (Config.FreeCamInvertMouseY ? -1 : 1);
                _freePitch = Mathf.Clamp(_freePitch, Config.FreeCamMinPitch, Config.FreeCamMaxPitch);
            }

            return new(_freePitch, _freeYaw, 0f);
        }

        Vector3 GetLocalMovement()
        {
            var direction = FreeCamInput.GetDirection();
            if (direction.IsZeroExact()) return Vector3.Zero;

            var speed = Config.FreeCamMoveSpeed * _speed * delta;
            return direction * speed;
        }
    }
}
