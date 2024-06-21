using Godot;

namespace F00F;

public static class TileMapExtensions
{
    public static Rect2 GetWorldRect(this TileMapLayer source)
    {
        var tiles = source.GetUsedRect();
        var tileSize = source.TileSet.TileSize;
        var worldSize = tiles.Size * tileSize;
        var worldPosition = tiles.Position * tileSize;
        return new(worldPosition, worldSize);
    }
}
