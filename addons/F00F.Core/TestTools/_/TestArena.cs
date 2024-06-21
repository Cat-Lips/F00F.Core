//using Godot;

//namespace F00F;

//[Tool]
//public partial class TestArena : TestTerrain
//{
//    #region Enums

//    public new class Enum : TestTerrain.Enum
//    {
//        public enum FloorType
//        {
//            Flat,
//            Terrain,
//        }
//    }

//    #endregion

//    #region Defaults

//    public new class Default : TestTerrain.Default
//    {
//        public static bool ShowRamp => true;
//        public static Enum.FloorType FloorType => Enum.FloorType.Flat;
//    }

//    #endregion

//    #region Export

//    [Export] public bool ShowRamp { get; set => this.Set(ref field, value, OnShowRampSet); } = Default.ShowRamp;
//    [Export] public Enum.FloorType FloorType { get; set => this.Set(ref field, value, OnFloorTypeSet); } = Default.FloorType;
//    [Export] public Enum.ShapeType FloorShape { get => ShapeType; set => ShapeType = value; }

//    #endregion

//    #region Private

//    private CollisionShape3D Ramp => field ??= GetNode<CollisionShape3D>("Ramp");

//    private void OnShowRampSet()
//    {
//        Ramp.Visible = ShowRamp;
//        Ramp.Disabled = !ShowRamp;
//    }

//    private const int FlatScale = 0;
//    private const int TerrainScale = 25;

//    private void OnFloorTypeSet()
//    {
//        switch (FloorType)
//        {
//            case Enum.FloorType.Flat:
//                ShowRamp = true;
//                Amplitude = FlatScale;
//                break;
//            case Enum.FloorType.Terrain:
//                ShowRamp = false;
//                Amplitude = TerrainScale;
//                break;
//        }
//    }

//    #endregion
//}
