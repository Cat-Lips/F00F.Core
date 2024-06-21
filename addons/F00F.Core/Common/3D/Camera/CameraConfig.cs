using Godot;

namespace F00F
{
    [Tool, GlobalClass]
    public partial class CameraConfig : CustomResource
    {
        #region Export

        [ExportGroup("Camera")]
        [Export] public float Speed { get; set; } = 5;
        [Export] public float Sensitivity { get; set; } = .001f;
        [Export(PropertyHint.Range, "0,90,radians_as_degrees")] public float PitchLimit { get; set; } = Mathf.DegToRad(75);

        [ExportGroup("Targeting")]
        [Export] public Vector3 Anchor { get; set; }
        [Export] public Vector3 Offset { get; set; } = Vector3.Up * 3 + Vector3.Back * 5;
        [Export] public Vector3 LookAt { get; set; } = Vector3.Forward * 5;

        //[ExportGroup("Tracking", "Tracking")]
        //[Export] public float TrackingNearClip { get; set; } = 10; // Tracked items closer than X meters will not be shown

        #endregion
    }
}
