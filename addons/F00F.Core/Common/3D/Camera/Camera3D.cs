using System;
using Godot;

namespace F00F;

public enum CameraMode
{
    Free,
    Follow,
    Select,
}

[Tool]
public partial class Camera3D : Godot.Camera3D
{
    #region Private

    private ITarget iTarget = null;
    private Node3D myTarget = null;
    private Node3D MyFollowTarget => iTarget?.Target ?? myTarget;
    private bool HasTarget => myTarget is not null;

    private float CurrentZoom = 1;
    private Transform3D ResetPosition;

    private float OrbitTimer;
    private Vector3 CurrentLookAt;
    private Vector3 CurrentAnchor;
    private Vector3 CurrentOffset;

    #endregion

    public event Action TargetSet;
    public event Action SelectModeSet;

    public event Action<Node3D> Select;

    #region Export

    [Export] public CameraInput Input { get; set => this.Set(ref field, value ?? new()); }
    [Export] public CameraConfig Config { get; set => this.Set(ref field, value ?? new()); }

    [Export] public Node3D Target { get; set => this.Set(ref field, value, TargetSet); }
    [Export] public bool SelectMode { get; set => this.Set(ref field, value, SelectModeSet); }

    public CameraMode CameraMode =>
        SelectMode ? CameraMode.Select :
        Target.NotNull() ? CameraMode.Follow :
        CameraMode.Free;

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        Input ??= new();
        Config ??= new();

        Editor.Disable(this);
        if (Editor.IsEditor) return;

        ResetPosition = GlobalTransform;
        SetPhysicsProcess(false);

        InitInput();
        OnTargetSet();
        OnSelectModeSet();
        TargetSet += OnTargetSet;
        SelectModeSet += OnSelectModeSet;
        Select += OnTargetSelected;

        void InitInput()
            => MyInput.ShowMouseSet += () => SelectMode = MyInput.ShowMouse;

        void OnTargetSet()
        {
            DetachTarget();
            iTarget = (myTarget = Target) as ITarget;
            AttachTarget();
            SelectMode = false;

            void DetachTarget()
            {
                if (myTarget is null) return;
                if (myTarget is IActive a) a.Active = false;
                myTarget.TreeExiting -= ClearTarget;
            }

            void AttachTarget()
            {
                if (myTarget is null) return;
                if (myTarget is IActive a) a.Active = !SelectMode;
                myTarget.TreeExiting += ClearTarget;
            }

            void ClearTarget()
                => Target = null;
        }

        void OnSelectModeSet()
        {
            if (SelectMode)
            {
                if (myTarget is IActive a) a.Active = false;
                MyInput.ShowMouse = true;
            }
            else
            {
                if (myTarget is IActive a) a.Active = true;
                GetViewport().GuiReleaseFocus();
                MyInput.ShowMouse = false;
            }
        }

        void OnTargetSelected(Node3D target)
        {
            if (IsTarget(target)) Target = target;
            else if (IsTarget(Target)) Target = null;

            bool IsTarget(Node3D x)
                => x is ITarget;
        }
    }

    public sealed override void _Process(double _delta)
    {
        var delta = (float)_delta;

        if (HasTarget)
            FollowTarget();
        else FreeCam();

        void FollowTarget()
        {
            UpdateZoom();
            UpdateOrbit();
            FollowTarget();

            void UpdateZoom()
            {
                if (SelectMode) return;
                if (Input.ZoomIn()) CurrentZoom -= Config.Speed * delta;
                if (Input.ZoomOut()) CurrentZoom += Config.Speed * delta;
                if (CurrentZoom < 0) CurrentZoom = 0;
            }

            void UpdateOrbit()
            {
                if (OrbitTimer < 0)
                    return;

                OrbitTimer -= delta;

                if (OrbitTimer < 0)
                {
                    CurrentLookAt = Config.LookAt;
                    CurrentAnchor = Config.Anchor;
                    CurrentOffset = Config.Offset;
                }
            }

            void FollowTarget()
            {
                var source = GlobalTransform;
                var target = MyFollowTarget.GlobalTransform;
                var lookAt = target.TranslatedLocal(CurrentLookAt).Origin;
                var anchor = target.TranslatedLocal(CurrentAnchor);
                var offset = anchor.TranslatedLocal(CurrentOffset * CurrentZoom);

                var weight = Config.Speed * delta;
                GlobalTransform = source.InterpolateWith(offset, weight);
                LookAt(lookAt);
            }
        }

        void FreeCam()
        {
            if (SelectMode) return;

            var speed = GetSpeed();
            var movement = Input.GetMovement();
            TranslateObjectLocal(movement * speed * delta);

            float GetSpeed()
            {
                var speed = Config.Speed;
                if (Input.Fast1()) speed *= 10;
                if (Input.Fast2()) speed *= 10;
                if (Input.Fast3()) speed *= 10;
                return speed;
            }
        }
    }

    public sealed override void _PhysicsProcess(double delta)
    {
        PerformRayCast();
        SetPhysicsProcess(false);

        void PerformRayCast()
        {
            var screenPos = GetViewport().GetMousePosition();
            var rayStart = ProjectRayOrigin(screenPos);
            var rayNormal = ProjectRayNormal(screenPos);
            var hitTarget = this.RayCastHitBody(rayStart, rayNormal, Far);
            this.CallDeferred(() => Select?.Invoke(hitTarget));
        }
    }

    public sealed override void _UnhandledInput(InputEvent e)
    {
        if (this.Handle(Input.SelectMode(), ToggleSelectMode)) return;

        switch (CameraMode)
        {
            case CameraMode.Free:
                if (this.Handle(Input.MouseRotate(this, e))) return;
                if (this.Handle(Input.Reset(), ResetCameraPosition, ResetZoom)) return;
                break;
            case CameraMode.Select:
                if (this.Handle(Input.Select(), RequestRayCast)) return;
                break;
            case CameraMode.Follow:
                if (this.Handle(Input.Reset(), ResetOrbitTimer, ResetZoom)) return;
                if (Config.AllowOrbit && this.Handle(Input.MouseOrbit(this, e, MyFollowTarget, ref CurrentLookAt)))
                {
                    OrbitTimer = Config.OrbitTime;
                    CurrentOffset = MyFollowTarget.ToLocal(GlobalPosition);
                    CurrentAnchor = Vector3.Zero;
                    return;
                }
                break;
        }

        void ResetZoom()
            => CurrentZoom = 1;

        void ResetOrbitTimer()
            => OrbitTimer = 0;

        void ResetCameraPosition()
            => GlobalTransform = ResetPosition;

        void RequestRayCast()
            => SetPhysicsProcess(true);

        void ToggleSelectMode()
            => SelectMode = !SelectMode;
    }

    #endregion
}
