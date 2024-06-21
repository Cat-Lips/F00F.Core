using System;
using Godot;

namespace F00F;

[Tool]
public partial class Camera : Camera3D
{
    public enum CameraMode
    {
        Free,
        Select,
        Follow,
    }

    #region Private

    private Camera3D Camera3D => this;

    private readonly RayCam RayCamInput = new();
    private readonly FreeCam FreeCamInput = new();
    private readonly FollowCam FollowCamInput = new();

    #endregion

    public event Action ViewChanged;
    public event Action TargetChanged;

    #region Export

    [Export] public Node3D Target { get; set => this.Set(ref field, value, TargetChanged); }
    [Export] public CameraConfig Config { get; set => this.Set(ref field, value ?? new(), OnConfigSet); }

    #endregion

    public CameraMode Mode =>
        SelectMode ? CameraMode.Select :
        Target.NotNull() ? CameraMode.Follow :
        CameraMode.Free;

    #region Godot

    public sealed override void _Ready()
    {
        Config ??= new();
        Editor.Disable(this);
        if (Editor.IsEditor) return;

        InitView();
        InitDebug();
        InitCamera();
        InitSelectMode();

        void InitCamera()
        {
            InitCamera();
            TargetChanged += InitCamera;

            void InitCamera()
            {
                if (Target is null)
                    InitFreeCam();
                else InitFollowCam();
            }
        }
    }

    public sealed override void _Process(double _delta)
    {
        UpdateView();

        if (MyShape.IsNull())
            UpdateCamera((float)_delta);
    }

    public sealed override void _PhysicsProcess(double _delta)
    {
        UpdateSelect();

        if (MyShape.NotNull())
            UpdateCamera((float)_delta);
    }

    public sealed override void _UnhandledInput(InputEvent e)
    {
        if (e.IsMouseMotion()) return;
        if (OnRayCamInput()) return;

        switch (Mode)
        {
            case CameraMode.Free:
                OnFreeCamInput();
                break;
            case CameraMode.Follow:
                OnFollowCamInput();
                break;
        }
    }

    #endregion

    #region Private

    private void OnConfigSet()
    {
        Config.ShapeChanged.Action -= InitShape;
        Config.ShapeChanged.Action += InitShape;
    }

    private void UpdateCamera(float delta)
    {
        if (Target is null)
            DoFreeCam(delta);
        else DoFollowCam(delta);

        ApplyShake(delta);
    }

    #endregion
}
