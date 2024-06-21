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
    }

    #endregion

    #region Defaults

    public static class Default
    {
        public static int FloorSize => 100;
        public static float StuntsScale => 10f;
        public static ShapeType FloorShape => ShapeType.Polygon;
    }

    #endregion

    public readonly AutoAction FloorSizeSet = new();
    public readonly AutoAction FloorShapeSet = new();
    public readonly AutoAction StuntsScaleSet = new();

    #region Exports

    [Export(PropertyHint.Range, "0,100,or_greater")] public int FloorSize { get; set => this.Set(ref field, value, FloorSizeSet.Run); } = Default.FloorSize;
    [Export] public ShapeType FloorShape { get; set => this.Set(ref field, value, FloorShapeSet.Run); } = Default.FloorShape;
    [Export(PropertyHint.Range, "1,10,or_greater")] public float StuntsScale { get; set => this.Set(ref field, value, StuntsScaleSet.Run); } = Default.StuntsScale;

    #endregion
}
