using System;
using Godot;
using static Godot.Input;

namespace F00F
{
    [Tool]
    public partial class Camera : Camera3D
    {
        #region Private

        private ITarget iTarget = null;
        private Node3D myTarget = null;
        private Node3D MyFollowTarget => iTarget?.Target ?? myTarget;
        private bool HasTarget => myTarget is not null;

        private float Zoom { get; set; } = 1;
        private Transform3D ResetPosition { get; set; }
        private bool RayCastRequested { get; set; }

        #endregion

        #region Export

        public enum TargetType
        {
            Chase,
            Follow,
            Fixed,
            Locked,
        }

        public event Action InputSet;
        public event Action ConfigSet;
        public event Action TargetSet;
        public event Action SelectModeSet;

        public CameraInput Input { get; set => this.Set(ref field, value, InputSet); }
        [Export] public CameraConfig Config { get; set => this.Set(ref field, value, ConfigSet); }
        [Export] public Node3D Target { get; set => this.Set(ref field, value, TargetSet); }
        [Export] public TargetType TargetAction { get; set; }
        [Export] public bool SelectMode { get; set => this.Set(ref field, value, SelectModeSet); }

        #endregion

        public event Action<CollisionObject3D> Select;

        #region Godot

        public override void _Ready()
        {
            Input ??= new();
            Config ??= new();
            Editor.Disable(this);
            if (Editor.IsEditor) return;

            ResetPosition = GlobalTransform;

            OnTargetSet();
            OnSelectModeSet();
            TargetSet += OnTargetSet;
            SelectModeSet += OnSelectModeSet;
            Select += OnTargetSelected;

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
                    myTarget.TreeExiting -= DetachTarget;
                }

                void AttachTarget()
                {
                    if (myTarget is null) return;
                    if (myTarget is IActive a) a.Active = !SelectMode;
                    myTarget.TreeExiting += DetachTarget;
                }
            }

            void OnSelectModeSet()
            {
                if (SelectMode)
                {
                    SetPhysicsProcess(true);
                    if (myTarget is IActive a) a.Active = false;
                    MouseMode = MouseModeEnum.Visible;
                }
                else
                {
                    SetPhysicsProcess(false);
                    if (myTarget is IActive a) a.Active = true;
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
            if (HasTarget)
                FollowTarget();
            else FreeCam();

            void FollowTarget()
            {
                UpdateZoom();

                switch (TargetAction)
                {
                    case TargetType.Chase: ChaseTarget(); break;
                    case TargetType.Follow: FollowTarget(); break;
                    case TargetType.Fixed: FixToTarget(); break;
                    case TargetType.Locked: LockToTarget(); break;
                }

                void UpdateZoom()
                {
                    if (SelectMode) return;
                    if (Input.ZoomIn()) Zoom -= Config.Speed * (float)delta;
                    if (Input.ZoomOut()) Zoom += Config.Speed * (float)delta;
                    if (Zoom < 0) Zoom = 0;
                }

                void ChaseTarget()
                {
                    var source = GlobalTransform;
                    var target = MyFollowTarget.GlobalTransform;
                    var anchor = target.TranslatedLocal(Config.Anchor);
                    var lookAt = target.TranslatedLocal(Config.LookAt).Origin;
                    var offset = anchor.TranslatedLocal(Config.Offset * Zoom);

                    var weight = Config.Speed * (float)delta;
                    GlobalTransform = source.InterpolateWith(offset, weight);
                    LookAt(lookAt);
                }

                void FollowTarget()
                {
                    // TODO
                }

                void FixToTarget()
                {
                    // TODO
                }

                void LockToTarget()
                {
                    // TODO
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

        public override void _PhysicsProcess(double delta)
        {
            if (RayCastRequested)
            {
                PerformRayCast();
                RayCastRequested = false;
            }

            void PerformRayCast()
            {
                var screenPos = GetViewport().GetMousePosition();
                var rayStart = ProjectRayOrigin(screenPos);
                var rayNormal = ProjectRayNormal(screenPos);
                var hitTarget = this.RayCastHitBody(rayStart, rayNormal, Far);
                this.CallDeferred(() => Select(hitTarget));
            }
        }

        public override void _UnhandledInput(InputEvent e)
        {
            if (this.Handle(Input.ToggleSelectMode(), ToggleSelectMode)) return;

            if (SelectMode)
            {
                if (this.Handle(Input.SelectJustPressed(), RequestRayCast)) return;
            }
            else
            {
                if (this.Handle(Input.ResetCameraPosition(), ResetCameraPosition, ResetZoom)) return;

                if (HasTarget)
                {
                    if (TargetAction is not TargetType.Chase)
                        this.Handle(this.MouseRotateAround(e, MyFollowTarget.GlobalPosition, Config.Sensitivity, Config.PitchLimit));
                }
                else
                {
                    this.Handle(this.MouseRotate(e, Config.Sensitivity, Config.PitchLimit));
                }
            }

            void ToggleSelectMode()
                => SelectMode = !SelectMode;

            void RequestRayCast()
                => RayCastRequested = true;

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
}
