using Godot;

namespace F00F;

using static TestArenaConfig.Enum;

[Tool]
public partial class TestArenaConfig : CustomResource
{
    #region Enums

    public static class Enum
    {
        public enum ShapeType { Plane, Polygon, Trimesh }
        public enum RibsOrBeams { Ribs, Beams }
        public enum RampOrSteps { Ramp, Steps }
    }

    #endregion

    #region Defaults

    public static class Default
    {
        public const int FloorSize = 100;
        public const ShapeType FloorShape = ShapeType.Polygon;

        public const bool ShowStunts = true;
        public const float StuntsScale = 10f;

        public const RibsOrBeams ShowRibsOrBeams = RibsOrBeams.Ribs;
        public const RampOrSteps ShowRampOrSteps = RampOrSteps.Ramp;

        public const float BeamGap = .1f;
        public const float BeamWidth = .025f;
        public const float BeamHeight = .05f;

        public const int StepCount = 10;
        public const float StepDepth = .1f;
        public const float StepHeight = .025f;
    }

    #endregion

    public readonly AutoAction FloorSizeSet = new();
    public readonly AutoAction FloorShapeSet = new();

    public readonly AutoAction ShowStuntsSet = new();
    public readonly AutoAction StuntsScaleSet = new();

    public readonly AutoAction ShowRibsOrBeamsSet = new();
    public readonly AutoAction ShowRampOrStepsSet = new();

    public readonly AutoAction BeamConfigChanged = new();
    public readonly AutoAction StepConfigChanged = new();

    #region Exports

    [ExportGroup("Floor")]
    [Export(PropertyHint.Range, "0,100,or_greater")] public int FloorSize { get; set => this.Set(ref field, value, FloorSizeSet.Run); } = Default.FloorSize;
    [Export] public ShapeType FloorShape { get; set => this.Set(ref field, value, FloorShapeSet.Run); } = Default.FloorShape;

    [ExportGroup("Stunts")]
    [Export] public bool ShowStunts { get; set => this.Set(ref field, value, notify: true, ShowStuntsSet.Run); } = Default.ShowStunts;
    [Export(PropertyHint.Range, "1,10,or_greater")] public float StuntsScale { get; set => this.Set(ref field, value, StuntsScaleSet.Run); } = Default.StuntsScale;

    [ExportGroup("Stunts")]
    [Export] public RibsOrBeams ShowRibsOrBeams { get; set => this.Set(ref field, value, notify: true, ShowRibsOrBeamsSet.Run); } = Default.ShowRibsOrBeams;
    [ExportSubgroup("Configure Beams...")]
    [Export(PropertyHint.Range, ".01,.1")] public float BeamGap { get; set => this.Set(ref field, value, BeamConfigChanged.Run); } = Default.BeamGap;
    [Export(PropertyHint.Range, ".01,.1")] public float BeamWidth { get; set => this.Set(ref field, value, BeamConfigChanged.Run); } = Default.BeamWidth;
    [Export(PropertyHint.Range, ".01,.1")] public float BeamHeight { get; set => this.Set(ref field, value, BeamConfigChanged.Run); } = Default.BeamHeight;

    [ExportGroup("Stunts")]
    [Export] public RampOrSteps ShowRampOrSteps { get; set => this.Set(ref field, value, notify: true, ShowRampOrStepsSet.Run); } = Default.ShowRampOrSteps;
    [ExportSubgroup("Configure Steps...")]
    [Export(PropertyHint.Range, "1,10")] public int StepCount { get; set => this.Set(ref field, value, StepConfigChanged.Run); } = Default.StepCount;
    [Export(PropertyHint.Range, ".01,.1")] public float StepDepth { get; set => this.Set(ref field, value, StepConfigChanged.Run); } = Default.StepDepth;
    [Export(PropertyHint.Range, ".01,.1")] public float StepHeight { get; set => this.Set(ref field, value, StepConfigChanged.Run); } = Default.StepHeight;

    #endregion
}
