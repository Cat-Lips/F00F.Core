using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Godot.TileSet;

namespace F00F;
public static class TileMapExtensions
{
    public static Rect2I GetWorldRect(this TileMapLayer source)
    {
        var tiles = source.GetUsedRect();
        var tileSize = source.TileSet.TileSize;
        var worldSize = tiles.Size * tileSize;
        var worldPosition = tiles.Position * tileSize;
        return new(worldPosition, worldSize);
    }

    public static bool HasTile(this TileMapLayer source, in Vector2I cell)
        => source.GetCellSourceId(cell) is not -1;

    public static bool HasNoTile(this TileMapLayer source, in Vector2I cell)
        => source.GetCellSourceId(cell) is -1;

    public static bool IsTile(this TileMapLayer source, in Vector2I cell, int sourceId, int sceneId)
        => source.GetCellSourceId(cell) == sourceId && source.GetCellAlternativeTile(cell) == sceneId;

    public static bool IsTile(this TileMapLayer source, in Vector2I cell, int sourceId, in Vector2I spriteId)
        => source.GetCellSourceId(cell) == sourceId && source.GetCellAtlasCoords(cell) == spriteId;

    public static void SetTile(this TileMapLayer source, in Vector2I cell, int sourceId, int sceneId)
        => source.SetCell(cell, sourceId, Vector2I.Zero, sceneId);

    public static void SetTile(this TileMapLayer source, in Vector2I cell, int sourceId, in Vector2I spriteId)
        => source.SetCell(cell, sourceId, spriteId);

    public static void ClearTile(this TileMapLayer source, in Vector2I cell)
        => source.SetCell(cell);

    public static TScene GetTile<TScene>(this TileMapLayer source, in Vector2I cell) where TScene : Node2D
    {
        var pos = source.MapToLocal(cell);
        return source.GetChildren<TScene>().SingleOrDefault(x => x.Position == pos); // TODO: Find a better way
    }

    public static IEnumerable<Vector2I> GetNeighborCells(this TileMapLayer source, Vector2I cell)
    {
        var tiles = source.TileSet;
        var types = NeighborCells(tiles.TileShape, tiles.TileOffsetAxis);
        return types.Select(x => source.GetNeighborCell(cell, x));
    }

    // Taken from TileSet::is_existing_neighbor

    private static IEnumerable<CellNeighbor> NeighborCells(TileShapeEnum shape, TileOffsetAxisEnum offset)
    {
        return shape switch
        {
            TileShapeEnum.Square => SquareNeighbors,
            TileShapeEnum.Isometric => IsometricNeighbors,
            _ => offset switch
            {
                TileOffsetAxisEnum.Vertical => VOffsetNeighbors,
                TileOffsetAxisEnum.Horizontal => HOffsetNeighbors,
                _ => throw new NotImplementedException($"{nameof(NeighborCells)}({shape} {offset})"),
            },
        };
    }

    private static readonly IEnumerable<CellNeighbor> SquareNeighbors = [
        CellNeighbor.RightSide, CellNeighbor.BottomRightCorner, CellNeighbor.BottomSide, CellNeighbor.BottomLeftCorner,
        CellNeighbor.LeftSide, CellNeighbor.TopLeftCorner, CellNeighbor.TopSide, CellNeighbor.TopRightCorner];

    private static readonly IEnumerable<CellNeighbor> IsometricNeighbors = [
        CellNeighbor.RightCorner, CellNeighbor.BottomRightSide, CellNeighbor.BottomCorner, CellNeighbor.BottomLeftSide,
        CellNeighbor.LeftCorner, CellNeighbor.TopLeftSide, CellNeighbor.TopCorner, CellNeighbor.TopRightSide];

    private static readonly IEnumerable<CellNeighbor> VOffsetNeighbors = [
        CellNeighbor.BottomRightSide, CellNeighbor.BottomSide, CellNeighbor.BottomLeftSide,
        CellNeighbor.TopLeftSide, CellNeighbor.TopSide, CellNeighbor.TopRightSide];

    private static readonly IEnumerable<CellNeighbor> HOffsetNeighbors = [
        CellNeighbor.RightSide, CellNeighbor.BottomRightSide, CellNeighbor.BottomLeftSide,
        CellNeighbor.LeftSide, CellNeighbor.TopLeftSide, CellNeighbor.TopRightSide];
}
