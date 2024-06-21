using System;
using Godot;

namespace F00F;

public enum CameraMode
{
    Free,
    Select,
    Follow,
    Orbit,
}

[Tool]
public partial class Camera3D : Godot.Camera3D
{
    #region Private

    private ITarget iTarget = null;
    private Node3D myTarget = null;
    private Node3D MyFollowTarget => iTarget?.Target ?? myTarget;
    private bool HasTarget => myTarget is not null;

    private float Zoom { get; set; } = 1;
    private Transform3D ResetPosition { get; set; }

    #endregion

    public event Action InputSet;
    public event Action ConfigSet;
    public event Action TargetSet;
    public event Action OrbitModeSet;
    public event Action SelectModeSet;

    public event Action<Node3D> Select;

    #region Export

    private static class Default
    {
        public static CameraInput Input => new CameraInputAll();
        public static CameraConfig Config => new();
    }

    [Export] public CameraInput Input { get; set => this.Set(ref field, value ?? Default.Input, InputSet); }
    [Export] public CameraConfig Config { get; set => this.Set(ref field, value ?? Default.Config, ConfigSet); }

    [Export] public Node3D Target { get; set => this.Set(ref field, value, TargetSet); }
    [Export] public bool OrbitMode { get; set => this.Set(ref field, value, OrbitModeSet); }
    [Export] public bool SelectMode { get; set => this.Set(ref field, value, SelectModeSet); }

    public CameraMode CameraMode =>
        SelectMode ? CameraMode.Select :
        Target.IsNull() ? CameraMode.Free :
        OrbitMode ? CameraMode.Orbit : CameraMode.Follow;

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        Input ??= Default.Input;
        Config ??= Default.Config;

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
            OrbitMode = false;
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

    public sealed override void _Process(double delta)
    {
        if (HasTarget)
            FollowTarget();
        else FreeCam();

        void FollowTarget()
        {
            //UpdateZoom();
            FollowTarget();

            void UpdateZoom()
            {
                if (SelectMode) return;
                if (Input.ZoomIn()) Zoom -= Config.Speed * (float)delta;
                if (Input.ZoomOut()) Zoom += Config.Speed * (float)delta;
                if (Zoom < 0) Zoom = 0;
            }

            void FollowTarget()
            {
                if (OrbitMode)
                {
                    // FIXME:  Zoom, maintain distance?
                    LookAt(MyFollowTarget.GlobalPosition);
                }
                else
                {
                    UpdateZoom();

                    var source = GlobalTransform;
                    var target = MyFollowTarget.GlobalTransform;
                    var lookAt = target.TranslatedLocal(Config.LookAt).Origin;
                    var anchor = target.TranslatedLocal(Config.Anchor);
                    var offset = anchor.TranslatedLocal(Config.Offset * Zoom);

                    var weight = Config.Speed * (float)delta;
                    GlobalTransform = source.InterpolateWith(offset, weight);
                    LookAt(lookAt);
                }
            }
        }

        void FreeCam()
        {
            if (SelectMode) return;

            var speed = GetSpeed();
            var movement = Input.GetMovement();
            TranslateObjectLocal(movement * speed * (float)delta);

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
        if (this.Handle(Input.ToggleSelectMode(), ToggleSelectMode)) return;

        switch (CameraMode)
        {
            case CameraMode.Free:
                if (this.Handle(Input.MouseRotate(this, e))) return;
                if (this.Handle(Input.ResetCameraPosition(), ResetCameraPosition, ResetZoom)) return;
                break;
            case CameraMode.Select:
                if (this.Handle(Input.SelectJustPressed(), RequestRayCast)) return;
                break;
            case CameraMode.Follow:
                if (this.Handle(Input.ToggleOrbitMode(), ToggleOrbitMode)) return;
                break;
            case CameraMode.Orbit:
                if (this.Handle(Input.MouseOrbit(this, e, MyFollowTarget.GlobalPosition))) return;
                if (this.Handle(Input.ToggleOrbitMode(), ToggleOrbitMode)) return;
                break;
        }

        void RequestRayCast()
            => SetPhysicsProcess(true);

        void ToggleOrbitMode()
            => OrbitMode = !OrbitMode;

        void ToggleSelectMode()
            => SelectMode = !SelectMode;

        void ResetCameraPosition()
        {
            if (myTarget is null)
                GlobalTransform = ResetPosition;
        }

        void ResetZoom()
        {
            if (myTarget is not null)
                Zoom = 1;
        }
    }

    #endregion
}
