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

    public static void ForEachCell(this TileMapLayer source, Action<Vector2I> action)
    {
        var rect = source.GetUsedRect();
        var pos = rect.Position;
        var size = rect.Size;
        for (var x = pos.X; x < size.X; ++x)
        {
            for (var y = pos.Y; y < size.Y; ++y)
            {
                action(new(x, y));
            }
        }
    }

    public static IEnumerable<Vector2I> GetNeighborCells(this TileMapLayer source, /*in */Vector2I cell)
    {
        var tiles = source.TileSet;
        var types = NeighborCells(tiles.TileShape, tiles.TileOffsetAxis);
        return types.Select(x => source.GetNeighborCell(cell, x));
    }

    public static TScene Get<TScene>(this TileMapLayer source, in Vector2I cell) where TScene : Node2D
    {
        var pos = source.MapToLocal(cell);
        return source.GetChildren<TScene>().SingleOrDefault(x => x.Position == pos); // TODO: Find a better way
    }

    public static bool IsSameTile(this TileMapLayer source, Node2D first, Node2D second)
        => source.IsSameTile(first.Position, second.Position);

    public static bool IsSameTile(this TileMapLayer source, in Vector2 first, in Vector2 second)
        => source.LocalToMap(first) == source.LocalToMap(second);

    #region Tiles

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

    #endregion

    #region Tags

    public static bool HasTag(this TileMapLayer source, in Vector2I cell, string tag)
    {
        if (tag.IsNullOrEmpty()) return false;
        var data = source.GetCellTileData(cell);
        return data is not null && data.HasCustomData(tag);
    }

    public static bool HasTag(this TileMapLayer source, in Vector2I cell, params IEnumerable<string> tags)
    {
        if (tags.IsNullOrEmpty()) return false;
        var data = source.GetCellTileData(cell);
        return data is not null && tags.Any(data.HasCustomData);
    }

    public static IEnumerable<string> GetTags(this TileMapLayer source, Vector2I cell, params IEnumerable<string> tags)
    {
        if (tags.IsNullOrEmpty()) yield break;
        var data = source.GetCellTileData(cell);
        if (data is null) yield break;
        foreach (var tag in tags)
        {
            if (data.HasCustomData(tag))
                yield return tag;
        }
    }

    #endregion

    #region Data

    public static bool TryGetData(this TileMapLayer source, in Vector2I cell, string key, out Variant value)
    {
        var data = source.GetCellTileData(cell);
        if (data?.HasCustomData(key) is true)
        {
            value = data.GetCustomData(key);
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public static IEnumerable<Variant> GetData(this TileMapLayer source, Vector2I cell, params IEnumerable<string> keys)
    {
        if (keys.IsNullOrEmpty()) yield break;
        var data = source.GetCellTileData(cell);
        if (data is null) yield break;
        foreach (var key in keys)
        {
            if (data.HasCustomData(key))
                yield return data.GetCustomData(key);
        }
    }

    #endregion

    #region Private

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

    #endregion
}
