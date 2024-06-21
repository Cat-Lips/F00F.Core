using System;
using Godot;
using static Godot.Input;

namespace F00F
{
    public interface IActive
    {
        public bool Active { get; set; }
    }

    public interface ITarget
    {
        public Node3D Target => null;
    }

    [Tool]
    public partial class Camera : Camera3D
    {
        private float Zoom { get; set; } = 1;
        private Transform3D ResetPosition { get; set; }

        #region Export

        private bool _selectMode;
        [Export] public bool SelectMode { get => _selectMode; set => this.Set(ref _selectMode, value, SelectModeSet); }
        public event Action SelectModeSet;

        private Node3D _target;
        [Export] public Node3D Target { get => _target; set => this.Set(ref _target, value, TargetSet); }
        public event Action TargetSet;

        private CameraConfig _config;
        [Export] public CameraConfig Config { get => _config; set => this.Set(ref _config, value ?? new(), ConfigSet); }
        public event Action ConfigSet;

        private CameraInput _input;
        public CameraInput Input { get => _input; set => this.Set(ref _input, value ?? new(), InputSet); }
        public event Action InputSet;

        private Node3D _follow;
        private Node3D Follow => _follow ?? _target;
        public void SetTarget(Node3D target, Node3D follow = null)
        {
            Target = target;
            _follow = follow;
        }

        #endregion

        public event Action<CollisionObject3D> Select;

        #region Godot

        public override void _Ready()
        {
            Input ??= new();
            Config ??= new();
            Editor.Disable(this);
            if (Editor.IsEditor) return;

            Node3D target = null;
            ResetPosition = GlobalTransform;

            OnTargetSet();
            OnSelectModeSet();
            TargetSet += OnTargetSet;
            SelectModeSet += OnSelectModeSet;
            Select += OnTargetSelected;

            void OnTargetSet()
            {
                DetachTarget();
                target = Target;
                AttachTarget();
                SelectMode = false;

                void DetachTarget()
                {
                    if (target is null) return;
                    if (target is IActive a) a.Active = false;
                    else target.Set(nameof(IActive.Active), false);
                    _follow = null;
                }

                void AttachTarget()
                {
                    if (target is null) return;
                    if (target is IActive a) a.Active = !SelectMode;
                    else target.Set(nameof(IActive.Active), !SelectMode);
                    if (target is ITarget t) _follow = t.Target;
                }
            }

            void OnSelectModeSet()
            {
                if (SelectMode)
                {
                    SetPhysicsProcess(true);
                    if (target is IActive a) a.Active = false;
                    else target?.Set(nameof(IActive.Active), false);
                    MouseMode = MouseModeEnum.Visible;
                }
                else
                {
                    SetPhysicsProcess(false);
                    if (target is IActive a) a.Active = true;
                    else target?.Set(nameof(IActive.Active), true);
                    MouseMode = MouseModeEnum.Captured;
                }
            }

            void OnTargetSelected(CollisionObject3D target)
            {
                if (IsTarget(target)) Target = target;
                else if (IsTarget(Target)) Target = null;

                bool IsTarget(Node3D x)
                    => x is ITarget;
            }
        }

        public override void _Process(double delta)
        {
            if (FollowTarget()) return;
            if (FreeCam()) return;

            bool FollowTarget()
            {
                if (Follow is null) return false;

                UpdateZoom();

                var source = GlobalTransform;
                var target = Follow.GlobalTransform;
                var anchor = target.TranslatedLocal(Config.Anchor);
                var lookAt = target.TranslatedLocal(Config.LookAt).Origin;
                var offset = anchor.TranslatedLocal(Config.Offset * Zoom);

                SmoothTransform();

                return true;

                void SmoothTransform()
                {
                    if (Input.RotateAroundTarget()) return;
                    var weight = Config.Speed * (float)delta;
                    GlobalTransform = source.InterpolateWith(offset, weight);
                    LookAt(lookAt);
                }
            }

            bool FreeCam()
            {
                if (SelectMode) return false;

                var speed = GetSpeed();
                var movement = Input.GetMovement();
                TranslateObjectLocal(movement * speed * (float)delta);

                return true;

                float GetSpeed()
                {
                    var speed = Config.Speed;
                    if (Input.Fast1()) speed *= 10;
                    if (Input.Fast2()) speed *= 10;
                    if (Input.Fast3()) speed *= 10;
                    return speed;
                }
            }

            void UpdateZoom()
            {
                if (SelectMode) return;
                if (Input.ZoomIn()) Zoom -= Config.Speed * (float)delta;
                if (Input.ZoomOut()) Zoom += Config.Speed * (float)delta;
                if (Zoom < 0) Zoom = 0;
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (!SelectMode) return;
            if (!Input.SelectJustPressed()) return;
            if (GetViewport().GuiGetHoveredControl() is not null) return;

            var screenPos = GetViewport().GetMousePosition();
            var rayStart = ProjectRayOrigin(screenPos);
            var rayNormal = ProjectRayNormal(screenPos);
            var hitTarget = this.RayCastHitBody(rayStart, rayNormal, Far);
            this.CallDeferred(() => Select(hitTarget));
        }

        public override void _UnhandledInput(InputEvent e)
        {
            if (HandleSelect()) return;
            if (!SelectMode) HandleMovement();

            bool HandleSelect()
                => this.Handle(Input.ToggleSelectMode(), ToggleSelectMode);

            void HandleMovement()
            {
                if (this.Handle(Input.ResetCameraPosition(), ResetCameraPosition, ResetZoom)) return;

                if (Follow is null)
                    this.Handle(this.MouseRotate(e, Config.Sensitivity, Config.PitchLimit));
                else if (Input.RotateAroundTarget())
                    this.Handle(this.MouseRotateAround(e, Follow.GlobalPosition, Config.Sensitivity, Config.PitchLimit));
            }

            void ToggleSelectMode()
                => SelectMode = !SelectMode;

            void ResetCameraPosition()
            {
                if (Follow is null)
                    GlobalTransform = ResetPosition;
            }

            void ResetZoom()
            {
                if (Follow is not null)
                    Zoom = 1;
            }
        }

        #endregion
    }
}
