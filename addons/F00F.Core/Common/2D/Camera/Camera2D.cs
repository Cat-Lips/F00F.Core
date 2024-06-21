using Godot;

namespace F00F;

[Tool]
public partial class Camera2D : Godot.Camera2D
{
    public TileMapLayer TileMap { get; set => this.Set(ref field, value, OnTileMapSet); }

    private AutoAction FitWorldRect;
    private void OnTileMapSet()
    {
        if (TileMap is null) return;
        if (Editor.IsEditor) return;

        (this.FitWorldRect ??= new(FitWorldRect)).Run();

        void FitWorldRect()
            => this.FitRect(TileMap.GetWorldRect());
    }
}
