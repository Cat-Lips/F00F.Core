using Godot;

namespace F00F;

using static CameraConfig.Enum;

public partial class Camera
{
    private float _orbitYaw;
    private float _orbitPitch;
    private float _orbitTimer;
    private bool _orbitLock;

    private float _lb;
    private bool _lookBehind;

    private void InitFollowCam()
    {
        _orbitYaw = 0;
        _orbitPitch = 0;
        _orbitTimer = 0;
        _orbitLock = false;

        _lb = 0;
        _lookBehind = false;
    }

    private bool OnFollowCamInput()
    {
        return
            this.Handle(FollowCamInput.OrbitLock(), OnOrbitLock);

        void OnOrbitLock()
        {
            _orbitLock = !_orbitLock;
            if (!_orbitLock) _orbitTimer = -1;
        }
    }

    private void DoFollowCam(float delta)
    {
        DoInput();

        var tt = ((Target as ITarget)?.Target ?? Target).GlobalTransform;
        var up = Config.TargetMatchUp ? tt.Up() : Vector3.Up;
        var hasVel = GetLateralVelocity(up, out var hVel, out var hVelDir);
        var fwd = GetForwardVelocity(hasVel, hVel, hVelDir);

        var gpos = tt.Origin;
        var gform = new Transform3D(Basis.LookingAt(fwd, up), gpos);

        var followOffset = Config.TargetFollowOffset;
        var lookAtOffset = Config.TargetLookAtOffset;

        ApplyZoom();
        ApplyOrbit();

        var targetPos = gform * followOffset;
        var lookAtPos = gform * lookAtOffset;

        gform = new Transform3D(Basis.LookingAt(lookAtPos - targetPos, up), targetPos);
        gform = GlobalTransform.InterpolateWith(gform, Config.TargetFollowSpeed * delta);

        if (MyShape.NotNull())
        {
            var castFrom = gpos.ClosestPointOnLine(lookAtPos, gform.Origin);
            MyWorld.SafeMoveAndCollide(MyShape, gform.Basis, castFrom, ref gform.Origin, exclude: Target);
            GlobalTransform = gform;
        }
        else GlobalTransform = gform;

        void DoInput()
        {
            var lookBehind = FollowCamInput.LookBehind();
            if (lookBehind && !_lookBehind) OnLookBehindOn();
            else if (!lookBehind && _lookBehind) OnLookBehindOff();
            _lookBehind = lookBehind;

            var lb = _lookBehind ? 1 : 0;
            _lb = Mathf.MoveToward(_lb, lb, Config.LookBehindSpeed * delta);

            void OnLookBehindOn() { }
            void OnLookBehindOff()
            {
                _orbitYaw = 0;
                _orbitPitch = 0;
                _orbitTimer = 0;
                _orbitLock = false;
            }
        }

        bool GetLateralVelocity(in Vector3 up, out Vector3 hVel, out Vector3 hVelDir)
        {
            var velocity =
                Target is CharacterBody3D cb ? cb.Velocity :
                Target is RigidBody3D rb ? rb.LinearVelocity : default;
            hVel = velocity.Slide(up);
            return hVel.TryNormalise(out hVelDir);
        }

        Vector3 GetForwardVelocity(bool hasVel, in Vector3 hVel, in Vector3 hVelDir)
        {
            var fwd = tt.Fwd();

            if (hasVel && Config.ReverseViewSpeedTriggered(hVel))
            {
                var hFwd = fwd.Slide(up);
                if (hVelDir.Dot(hFwd).Abs() > Config.ReverseViewDeadZone)
                    fwd = hVelDir;
            }

            return
               _lb is 0 ? fwd :
               _lb is 1 ? -fwd :
               fwd.Slerp(-fwd, _lb).Normalized();
        }

        void ApplyZoom()
        {
            if (!hasVel) return;
            if (!Config.ZoomEnabled) return;

            var zoom = Mathf.Lerp(0, Config.ZoomMaxDistance, Zoom());
            var followDir = followOffset.Normalized();
            var lookAtDir = lookAtOffset.Normalized();

            followOffset += followDir * zoom;
            lookAtOffset += lookAtDir * zoom;

            float Zoom()
            {
                var hSqrSpeed = hVel.LengthSquared();
                var maxSqrSpeed = Config.ZoomMaxSpeed * Config.ZoomMaxSpeed;
                var tSqrZoom = Mathf.Clamp(hSqrSpeed / maxSqrSpeed, 0, 1);

                return Config.ZoomMode switch
                {
                    ZoomMode.StartSlow => tSqrZoom,
                    ZoomMode.Linear => Mathf.Sqrt(tSqrZoom),
                    ZoomMode.StartFast => tSqrZoom * (2f - tSqrZoom),
                    _ => throw new System.NotImplementedException(),
                };
            }
        }

        void ApplyOrbit()
        {
            if (SelectMode) return;
            if (_lookBehind) return;
            if (!Config.OrbitEnabled) return;

            UpdateOrbit(MyInput.Instance.MouseDelta);

            if (_orbitYaw is 0 && _orbitPitch is 0) return;
            gform.Basis *= Basis.FromEuler(new(_orbitPitch, _orbitYaw, 0f));

            void UpdateOrbit(in Vector2 movement)
            {
                if (movement.IsZeroExact())
                {
                    if (Config.OrbitReturnEnabled && !_orbitLock)
                        ApplyOrbitReturn();
                }
                else
                {
                    if (Config.OrbitReturnEnabled)
                        ResetOrbitReturn();

                    UpdateOrbit(movement);
                }

                void ResetOrbitReturn()
                    => _orbitTimer = Config.OrbitReturnDelay;

                void ApplyOrbitReturn()
                {
                    if (_orbitTimer > 0)
                        _orbitTimer -= delta;

                    if (_orbitTimer < 0)
                    {
                        (_orbitYaw, _orbitPitch) = new Vector2(_orbitYaw, _orbitPitch)
                            .MoveToward(Vector2.Zero, Config.OrbitReturnSpeed * delta);

                        if (_orbitYaw is 0 && _orbitPitch is 0)
                            _orbitTimer = 0f;
                    }
                }

                void UpdateOrbit(in Vector2 movement)
                {
                    _orbitYaw -= movement.X * Config.OrbitSensitivity;
                    _orbitYaw = Mathf.Wrap(_orbitYaw, -Const.Pi, Const.Pi);
                    _orbitPitch -= movement.Y * Config.OrbitSensitivity * (Config.OrbitInvertMouseY ? -1 : 1);
                    _orbitPitch = Mathf.Clamp(_orbitPitch, Config.OrbitMinPitch, Config.OrbitMaxPitch);
                }
            }
        }
    }
}
