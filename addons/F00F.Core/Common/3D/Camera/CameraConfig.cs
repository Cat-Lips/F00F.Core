using Godot;

namespace F00F
{
    [Tool, GlobalClass]
    public partial class CameraConfig : Resource
    {
        [ExportGroup("Camera")]
        [Export] public float Speed { get; set; } = 5;
        [Export] public float Sensitivity { get; set; } = .001f;
        [Export(PropertyHint.Range, "0,90,radians_as_degrees")] public float PitchLimit { get; set; } = Mathf.DegToRad(75);

        [ExportGroup("Targeting")]
        //[Export] public Vector3 Anchor { get; set; } /* Anchor point on target             */ = Vector3.Zero;
        [Export] public Vector3 Offset { get; set; } /* Offset of camera from anchor point */ = Vector3.Up * 3 + Vector3.Back * 5; // TODO: Direction of travel!
        [Export] public Vector3 Look { get; set; }   /* Look position relative to target   */ = Vector3.Forward * 5;

        //[ExportGroup("Tracking", "Tracking")]
        //[Export] public float TrackingNearClip { get; set; } = 10; // Tracked items closer than X meters will not be shown
    }
}
