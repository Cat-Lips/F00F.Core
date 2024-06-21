using Godot;

namespace F00F;

using static CameraConfig.Enum;

[Tool, GlobalClass]
public partial class CameraConfig : CustomResource
{
    #region Private

    private const string MaxPitch = "0,90,radians_as_degrees";
    private const string MinPitch = "-90,0,radians_as_degrees";

    #endregion

    #region Enums

    public static class Enum
    {
        public enum ZoomMode { StartSlow, Linear, StartFast }
        public enum ShapeType { None, Sphere, Polygon }
    }

    #endregion

    #region Defaults

    public static class Default
    {
        private static readonly float MaxPitch = Mathf.DegToRad(89);
        private static readonly float MinPitch = Mathf.DegToRad(-89);

        public const bool FreeCamEnabled = true;
        public const float FreeCamMoveSpeed = 10;
        public const float FreeCamSensitivity = .002f;
        public const bool FreeCamInvertMouseY = false;
        public static readonly float FreeCamMaxPitch = MaxPitch;
        public static readonly float FreeCamMinPitch = MinPitch;

        public const bool TargetMatchUp = true;
        public const float TargetFollowSpeed = 6;
        public static readonly Vector3 TargetFollowOffset = Vector3.Up * 3f + Vector3.Back * 5f;
        public static readonly Vector3 TargetLookAtOffset = Vector3.Forward * 5f;

        public const bool ZoomEnabled = true;
        public const float ZoomMaxSpeed = 30f;
        public const float ZoomMaxDistance = 10f;
        public const ZoomMode ZoomMode = ZoomMode.StartSlow;

        public const bool LookBehindEnabled = true;
        public const float LookBehindSpeed = 30f;

        public const bool ReverseViewEnabled = true;
        public const float ReverseViewDeadZone = .1f;
        public const float ReverseViewSpeedTrigger = 1f;

        public const bool OrbitEnabled = true;
        public const bool OrbitReturnEnabled = true;
        public const float OrbitReturnDelay = 1f;
        public const float OrbitReturnSpeed = 1f;
        public const float OrbitSensitivity = .002f;
        public const bool OrbitInvertMouseY = false;
        public static readonly float OrbitMaxPitch = MaxPitch;
        public static readonly float OrbitMinPitch = MinPitch;

        public const bool ShakeEnabled = true;
        public const float ShakeRoughness = 20f;
        public const float ShakeScreenMultiplier = 1;
        public const float ShakeCameraMultiplier = 0;
        public static Noise ShakeNoise => NewShakeNoise();

        public const ShapeType ShapeType = ShapeType.Polygon;
        public const float ShapeExpand = .2f;

        private static FastNoiseLite NewShakeNoise() => new()
        {
            NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin,
            Frequency = .8f,
        };
    }

    #endregion

    #region Exports

    [ExportGroup("FreeCam", "FreeCam")]
    [Export] public bool FreeCamEnabled { get; set => this.Set(ref field, value, notify: true); } = Default.FreeCamEnabled;
    [Export] public float FreeCamMoveSpeed { get; set => this.Set(ref field, value); } = Default.FreeCamMoveSpeed;
    [Export] public float FreeCamSensitivity { get; set => this.Set(ref field, value); } = Default.FreeCamSensitivity;
    [Export] public bool FreeCamInvertMouseY { get; set => this.Set(ref field, value); } = Default.FreeCamInvertMouseY;
    [Export(PropertyHint.Range, MaxPitch)] public float FreeCamMaxPitch { get; set => this.Set(ref field, value); } = Default.FreeCamMaxPitch;
    [Export(PropertyHint.Range, MinPitch)] public float FreeCamMinPitch { get; set => this.Set(ref field, value); } = Default.FreeCamMinPitch;

    [ExportGroup("FollowCam", "Target")]
    [Export] public bool TargetMatchUp { get; set => this.Set(ref field, value); } = Default.TargetMatchUp;
    [Export] public float TargetFollowSpeed { get; set => this.Set(ref field, value); } = Default.TargetFollowSpeed;
    [Export] public Vector3 TargetFollowOffset { get; set => this.Set(ref field, value); } = Default.TargetFollowOffset;
    [Export] public Vector3 TargetLookAtOffset { get; set => this.Set(ref field, value); } = Default.TargetLookAtOffset;

    [ExportSubgroup("Speed Zoom", "Zoom")]
    [Export] public bool ZoomEnabled { get; set => this.Set(ref field, value, notify: true); } = Default.ZoomEnabled;
    [Export] public ZoomMode ZoomMode { get; set => this.Set(ref field, value); } = Default.ZoomMode;
    [Export] public float ZoomMaxSpeed { get; set => this.Set(ref field, value); } = Default.ZoomMaxSpeed;
    [Export] public float ZoomMaxDistance { get; set => this.Set(ref field, value); } = Default.ZoomMaxDistance;

    [ExportSubgroup("Look Behind", "LookBehind")]
    [Export] public bool LookBehindEnabled { get; set => this.Set(ref field, value, notify: true); } = Default.LookBehindEnabled;
    [Export] public float LookBehindSpeed { get; set => this.Set(ref field, value); } = Default.LookBehindSpeed;

    [ExportSubgroup("Reverse View", "ReverseView")]
    [Export] public bool ReverseViewEnabled { get; set => this.Set(ref field, value, notify: true); } = Default.ReverseViewEnabled;
    [Export] public float ReverseViewDeadZone { get; set => this.Set(ref field, value); } = Default.ReverseViewDeadZone;
    [Export] public float ReverseViewSpeedTrigger { get; set => this.Set(ref field, value); } = Default.ReverseViewSpeedTrigger;

    [ExportSubgroup("Orbit Target", "Orbit")]
    [Export] public bool OrbitEnabled { get; set => this.Set(ref field, value, notify: true); } = Default.OrbitEnabled;
    [Export] public bool OrbitReturnEnabled { get; set => this.Set(ref field, value, notify: true); } = Default.OrbitReturnEnabled;
    [Export] public float OrbitReturnDelay { get; set => this.Set(ref field, value); } = Default.OrbitReturnDelay;
    [Export] public float OrbitReturnSpeed { get; set => this.Set(ref field, value); } = Default.OrbitReturnSpeed;
    [Export] public float OrbitSensitivity { get; set => this.Set(ref field, value); } = Default.OrbitSensitivity;
    [Export] public bool OrbitInvertMouseY { get; set => this.Set(ref field, value); } = Default.OrbitInvertMouseY;
    [Export(PropertyHint.Range, MaxPitch)] public float OrbitMaxPitch { get; set => this.Set(ref field, value); } = Default.OrbitMaxPitch;
    [Export(PropertyHint.Range, MinPitch)] public float OrbitMinPitch { get; set => this.Set(ref field, value); } = Default.OrbitMinPitch;

    [ExportGroup("Camera Shake", "Shake")]
    [Export] public bool ShakeEnabled { get; set => this.Set(ref field, value, notify: true); } = Default.ShakeEnabled;
    [Export] public float ShakeRoughness { get; set => this.Set(ref field, value); } = Default.ShakeRoughness;
    [Export] public float ShakeScreenMultiplier { get; set => this.Set(ref field, value.Clamp(0, 1)); } = Default.ShakeScreenMultiplier;
    [Export] public float ShakeCameraMultiplier { get; set => this.Set(ref field, value.Clamp(0, 1)); } = Default.ShakeCameraMultiplier;
    [Export] public Noise ShakeNoise { get; set => this.Set(ref field, value ?? Default.ShakeNoise); } = Default.ShakeNoise;

    [ExportGroup("Camera Shape", "Shape")]
    [Export] public ShapeType ShapeType { get; set => this.Set(ref field, value, ShapeChanged.Run); } = Default.ShapeType;
    [Export(PropertyHint.Range, "0,1")] public float ShapeExpand { get; set => this.Set(ref field, value.Clamp(0, 1), ShapeChanged.Run); } = Default.ShapeExpand;

    #endregion

    public readonly AutoAction ShapeChanged = new();

    public Shape3D GetShape(Godot.Camera3D camera) => ShapeType switch
    {
        ShapeType.None => null,
        ShapeType.Sphere => camera.GetSphereShape(ShapeExpand),
        ShapeType.Polygon => camera.GetPyramidShape(ShapeExpand),
        _ => throw new System.NotImplementedException(),
    };

    public bool ReverseViewSpeedTriggered(in Vector3 vel)
        => ReverseViewEnabled && vel.LengthSquared() > ReverseViewSpeedTrigger * ReverseViewSpeedTrigger;
}
