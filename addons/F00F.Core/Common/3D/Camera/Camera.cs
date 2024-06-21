using System;
using Godot;
using static Godot.Input;

// TODO: https://yosoyfreeman.github.io/article/godot/tutorial/achieving-better-mouse-input-in-godot-4-the-perfect-camera-controller/

namespace F00F
{
    public enum CameraMode
    {
        Free,
        Follow,
        Fixed, // Broken
    }

    [Tool]
    public partial class Camera : Camera3D
    {
        private Vector3? FixPosition { get; set; }
        private Vector3? TargetPosition { get; set; }
        private Transform3D ResetPosition { get; set; }
        private bool Fixed => CameraMode is CameraMode.Fixed;
        private bool Follow => CameraMode is CameraMode.Follow;
        private bool FreeLook => CameraMode is CameraMode.Free;

        #region Export

        private bool _selectMode;
        [Export] public bool SelectMode { get => _selectMode; set => this.Set(ref _selectMode, value, SelectModeSet); }
        public event Action SelectModeSet;

        private CameraMode _cameraMode;
        [Export] public CameraMode CameraMode { get => _cameraMode; set => this.Set(ref _cameraMode, value, CameraModeSet); }
        public event Action CameraModeSet;

        private Node3D _target;
        [Export] public Node3D Target { get => _target; set => this.Set(ref _target, value, TargetSet); }
        public event Action TargetSet;

        private CameraConfig _config;
        [Export] private CameraConfig Config { get => _config; set => this.Set(ref _config, value ?? new()); }

        private CameraInput _input;
        [Export] private CameraInput Input { get => _input; set => this.Set(ref _input, value ?? new()); }

        #endregion

        public event Action<CollisionObject3D> Select;

        public void SetTarget(Node3D target, CameraMode mode, CameraMode alt = CameraMode.Free)
        {
            Target = target;
            CameraMode = target is null ? alt : mode;
        }

        #region Godot

        public override void _Ready()
        {
            Input ??= new();
            Config ??= new();
            Editor.Disable(this);
            if (Engine.IsEditorHint()) return;

            Node3D target = null;
            ResetPosition = GlobalTransform;

            OnTargetSet();
            OnSelectModeSet();
            OnCameraModeSet();
            TargetSet += OnTargetSet;
            SelectModeSet += OnSelectModeSet;
            CameraModeSet += OnCameraModeSet;

            void OnTargetSet()
            {
                DetachTarget();
                target = Target;
                AttachTarget();

                void DetachTarget()
                {
                    if (target is null) return;

                    target.Set("Active", false);
                    TargetPosition = null;
                    FixPosition = null;
                }

                void AttachTarget()
                {
                    if (target is null) return;

                    target.Set("Active", !FreeLook);
                    TargetPosition = target.GlobalPosition;
                    FixPosition = Fixed ? TargetPosition - GlobalPosition : null;
                }
            }

            void OnSelectModeSet()
            {
                if (SelectMode)
                {
                    SetPhysicsProcess(true);
                    Target?.Set("Active", false);
                    MouseMode = MouseModeEnum.Visible;
                }
                else
                {
                    SetPhysicsProcess(false);
                    Target?.Set("Active", !FreeLook);
                    MouseMode = MouseModeEnum.Captured;
                }
            }

            void OnCameraModeSet()
            {
                Target?.Set("Active", !FreeLook);
                FixPosition = Fixed ? TargetPosition - GlobalPosition : null;
            }
        }

        public override void _Process(double delta)
        {
            ProcessMovement();

            void ProcessMovement()
            {
                switch (CameraMode)
                {
                    case CameraMode.Free: Free(); break;
                    case CameraMode.Fixed: Fixed(); break;
                    case CameraMode.Follow: Follow(); break;
                }

                void Free()
                {
                    if (!SelectMode)
                        TranslateObjectLocal(Input.GetMovement() * GetSpeed() * (float)delta);

                    float GetSpeed()
                    {
                        var speed = Config.Speed;
                        if (Input.Fast1()) speed *= 10;
                        if (Input.Fast2()) speed *= 10;
                        if (Input.Fast3()) speed *= 10;
                        return speed;
                    }
                }

                void Fixed()
                    => Follow(FixPosition);

                void Follow(in Vector3? fixPosition = null)
                {
                    if (Target is null) return;

                    var source = GlobalTransform;
                    var target = Target.GlobalTransform;
                    var targetPosition = target.Origin;
                    var targetVelocity = targetPosition - TargetPosition.Value;
                    var targetDirection = targetVelocity.Normalized();

                    var offset = target.TranslatedLocal(fixPosition ?? Config.Offset);
                    var lookAt = targetPosition + Config.Look;

                    GlobalTransform = source.InterpolateWith(offset, Config.Speed * (float)delta);
                    LookAt(lookAt);
                }
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            ProcessSelect();

            void ProcessSelect()
            {
                if (!SelectMode) return;
                if (Select is null) return;
                if (!Input.SelectJustPressed()) return;
                if (GetViewport().GuiGetHoveredControl() is not null) return;

                var screenPos = GetViewport().GetMousePosition();
                var rayStart = ProjectRayOrigin(screenPos);
                var rayNormal = ProjectRayNormal(screenPos);
                var hitTarget = this.RayCastHitBody(rayStart, rayNormal, Far);
                this.CallDeferred(() => Select(hitTarget));
            }
        }

        public override void _UnhandledInput(InputEvent e)
        {
            ProcessInput();

            void ProcessInput()
            {
                if (this.Handle(Input.ToggleSelectMode(), ToggleSelectMode) || SelectMode) return;

                switch (CameraMode)
                {
                    case CameraMode.Free: Free(); break;
                    case CameraMode.Fixed: Fixed(); break;
                    case CameraMode.Follow: Follow(); break;
                }

                void Free()
                {
                    if (this.Handle(Input.ResetCameraPosition(), ResetCameraPosition)) return;
                    if (this.Handle(this.MouseRotate(e, Config.Sensitivity, Config.PitchLimit))) return;
                }

                void Fixed()
                {
                    if (this.Handle(this.MouseRotateAround(e, TargetPosition ?? GlobalPosition, Config.Sensitivity, Config.PitchLimit))) return;
                    FixPosition = TargetPosition - GlobalPosition;
                }

                void Follow()
                {
                }
            }

            void ToggleSelectMode()
                => SelectMode = !SelectMode;

            void ResetCameraPosition()
                => GlobalTransform = ResetPosition;
        }

        #endregion
    }
}
