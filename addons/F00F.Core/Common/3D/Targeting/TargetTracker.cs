using System;
using Godot;

namespace F00F;

using Camera = Godot.Camera3D;

[Tool]
public partial class TargetTracker : Node3D
{
    private Camera camera;
    private float screenAspect;
    private Vector2 screenCenter;
    private float offScreenImageOffset;

    private Func<Vector3> GetParentVelocity;

    private Control HUD => field ??= (Control)GetNode("%HUD");
    private TextureRect OnScreenView => field ??= (TextureRect)GetNode("%OnScreen");
    private TextureRect OffScreenView => field ??= (TextureRect)GetNode("%OffScreen");

    [Export] public bool ShowOnScreen { get; set; } = true;
    [Export] public bool ShowOffScreen { get; set; } = true;
    [Export] public float OffScreenMargin { get; set; } = 1;

    [Export] public bool ShowVelocityHighlights { get; set; } = true;
    [Export] public Color SlowHighlight { get; set; } = Colors.Green;
    [Export(PropertyHint.ColorNoAlpha)] public Color FastHighlight { get; set; } = Colors.Red;
    [Export(PropertyHint.Range, "0,100,or_greater")] public float SlowThreshold { get; set; } = 10f;
    [Export(PropertyHint.Range, "0,100,or_greater")] public float FastThreshold { get; set; } = 100f;

    public sealed override void _Ready()
    {
        Editor.Disable(this);
        if (Editor.IsEditor) return;

        var root = GetViewport();
        camera = root.GetCamera3D();

        InitScreen();
        InitOffsets();
        InitVelocity();

        ShowOnScreen = true;

        void InitScreen()
        {
            InitScreen();
            root.SizeChanged += InitScreen;

            void InitScreen()
            {
                screenCenter = root.GetDisplayRect().Size * .5f;
                screenAspect = screenCenter.Aspect();
            }
        }

        void InitOffsets()
        {
            var offScreenImage = OffScreenView.Texture?.GetImage();
            if (offScreenImage.IsNull()) { offScreenImageOffset = 0; return; }
            var offScreenOffset = offScreenImage.GetIndexOfFirstVisiblePixelFromTop();
            offScreenImageOffset = offScreenImage.GetSize().Max() * .5f - offScreenOffset;
            OffScreenView.PivotOffset = offScreenImage.GetSize() / 2;
        }

        void InitVelocity()
        {
            GetParentVelocity = GetVelocityFunc();

            Func<Vector3> GetVelocityFunc()
            {
                var parent = GetParentNode3D();
                return parent is RigidBody3D body ? (() => body.LinearVelocity)
                     : parent is CharacterBody3D player ? (() => player.Velocity)
                     : null;
            }
        }
    }

    public sealed override void _Process(double _delta)
    {
        TrackTarget();
        ApplyHighlights();

        void TrackTarget()
        {
            var gpos = GlobalPosition;
            var behindCamera = camera.ToLocal(gpos).IsBehind();

            if (!behindCamera && camera.IsPositionInFrustum(gpos))
            {
                OnScreenView.Visible = ShowOnScreen;
                OffScreenView.Visible = false;

                HUD.Position = camera.UnprojectPosition(gpos);
            }
            else
            {
                OnScreenView.Visible = false;
                OffScreenView.Visible = ShowOffScreen;

                var targetPos = camera.UnprojectPosition(gpos);
                var fromCenter = targetPos - screenCenter;
                if (behindCamera) fromCenter = -fromCenter;

                ClampAspect();
                ApplyMargins();

                void ClampAspect()
                {
                    var abs = fromCenter.Abs();
                    if (abs.Aspect() >= screenAspect)
                        fromCenter *= screenCenter.X / abs.X.Max(Const.Epsilon);
                    else fromCenter *= screenCenter.Y / abs.Y.Max(Const.Epsilon);
                }

                void ApplyMargins()
                {
                    var margin = offScreenImageOffset + OffScreenMargin;
                    fromCenter -= fromCenter.Normalized() * margin;
                }

                var pos = screenCenter + fromCenter;
                var rot = Vector2.Up.AngleTo(fromCenter);

                HUD.Position = pos;
                OffScreenView.Rotation = rot;
            }
        }

        void ApplyHighlights()
        {
            HUD.Modulate = ShowVelocityHighlights
                ? GetVelocityHighlight()
                : GetDefaultColor();

            Color GetVelocityHighlight()
            {
                if (GetParentVelocity is null)
                    return GetDefaultColor();

                var velocity = GetParentVelocity().LengthSquared();
                if (velocity < SlowThreshold) return GetDefaultColor();
                if (velocity > FastThreshold) return FastHighlight; // TODO:  Add glow effect
                return SlowHighlight.Lerp(FastHighlight, velocity / FastThreshold);
            }

            Color GetDefaultColor()
                => Colors.White;
        }
    }
}
