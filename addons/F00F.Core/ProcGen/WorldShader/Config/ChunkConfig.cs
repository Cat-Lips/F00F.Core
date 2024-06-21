using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class ChunkConfig : CustomResource
{
    #region Default

    public static class Default
    {
        public const bool Lod = false;
        public static readonly int Size = Editor.IsEditor ? 64 : 256;
        public static readonly int Radius = Editor.IsEditor ? 9 : 21;
    }

    #endregion

    #region Export

    [ExportCategory("Chunks")]
    [Export] public bool Lod { get; set => this.Set(ref field, value); } = Default.Lod;
    [Export(PropertyHint.Range, "4,1024")] public int Size { get; set => this.Set(ref field, value.ToPo2(from: field)); } = Default.Size;
    [Export(PropertyHint.Range, "0,9,or_greater")] public int Radius { get; set => this.Set(ref field, value.ClampMin(0)); } = Default.Radius;

    #endregion
}
