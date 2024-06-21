using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class CameraConfig : CustomResource
{
    #region Default

    public static class Default
    {
        public const float Speed = 5f;
        public const float Sensitivity = Const.SmallFloat;
        public static readonly float PitchLimit = Mathf.DegToRad(75f);

        public static readonly Vector3 Anchor = Vector3.Zero;
        public static readonly Vector3 Offset = Vector3.Up * 3f + Vector3.Back * 5f;
        public static readonly Vector3 LookAt = Vector3.Forward * 5f;

        public static readonly bool AllowOrbit = true;
        public static readonly float OrbitTime = -1f;
        public static readonly float OrbitPitchLimit = Mathf.DegToRad(75f);
    }

    #endregion

    #region Export

    [ExportGroup("Camera")]
    [Export] public float Speed { get; set; } = Default.Speed;
    [Export] public float Sensitivity { get; set; } = Default.Sensitivity;
    [Export(PropertyHint.Range, "0,90,radians_as_degrees")] public float PitchLimit { get; set; } = Default.PitchLimit;

    [ExportGroup("Targeting")]
    [Export] public Vector3 Anchor { get; set; } = Default.Anchor;
    [Export] public Vector3 Offset { get; set; } = Default.Offset;
    [Export] public Vector3 LookAt { get; set; } = Default.LookAt;

    [Export] public bool AllowOrbit { get; set; } = Default.AllowOrbit;
    [Export(PropertyHint.Range, "-1,1,or_greater")] public float OrbitTime { get; set; } = Default.OrbitTime;
    [Export(PropertyHint.Range, "0,90,radians_as_degrees")] public float OrbitPitchLimit { get; set; } = Default.OrbitPitchLimit;

    #endregion
}
