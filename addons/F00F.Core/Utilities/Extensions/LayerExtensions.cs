using System;
using Godot;

namespace F00F;

public static class LayerExtensions
{
    private static int Layer<T>(T layer) where T : struct, Enum
        => Convert.ToInt32(layer) + 1;

    #region Collision Layers (2D)

    public static void SetCollisionLayer(this CollisionObject2D source) => source.ClearCollisionLayer();
    public static void SetCollisionLayer<T>(this CollisionObject2D source, params T[] layers) where T : struct, Enum
    {
        source.ClearCollisionLayer();
        layers.ForEach(layer => source.SetCollisionLayer(layer, true));
    }

    public static void SetCollisionMask(this CollisionObject2D source) => source.ClearCollisionLayer();
    public static void SetCollisionMask<T>(this CollisionObject2D source, params T[] mask) where T : struct, Enum
    {
        source.ClearCollisionMask();
        mask.ForEach(layer => source.SetCollisionMask(layer, true));
    }

    public static void SetCollisionLayer<T>(this CollisionObject2D source, T layer, bool value) where T : struct, Enum
        => source.SetCollisionLayerValue(Layer(layer), value);

    public static void SetCollisionMask<T>(this CollisionObject2D source, T layer, bool value) where T : struct, Enum
        => source.SetCollisionMaskValue(Layer(layer), value);

    private static void ClearCollisionLayer(this CollisionObject2D source)
        => source.CollisionLayer = 0;

    private static void ClearCollisionMask(this CollisionObject2D source)
        => source.CollisionMask = 0;

    #endregion

    #region Collision Layers (3D)

    public static void SetCollisionLayer(this CollisionObject3D source) => source.ClearCollisionLayer();
    public static void SetCollisionLayer<T>(this CollisionObject3D source, params T[] layers) where T : struct, Enum
    {
        source.ClearCollisionLayer();
        layers.ForEach(layer => source.SetCollisionLayer(layer, true));
    }

    public static void SetCollisionMask(this CollisionObject3D source) => source.ClearCollisionMask();
    public static void SetCollisionMask<T>(this CollisionObject3D source, params T[] mask) where T : struct, Enum
    {
        source.ClearCollisionMask();
        mask.ForEach(layer => source.SetCollisionMask(layer, true));
    }

    public static void SetCollisionLayer<T>(this CollisionObject3D source, T layer, bool value) where T : struct, Enum
        => source.SetCollisionLayerValue(Layer(layer), value);

    public static void SetCollisionMask<T>(this CollisionObject3D source, T layer, bool value) where T : struct, Enum
        => source.SetCollisionMaskValue(Layer(layer), value);

    private static void ClearCollisionLayer(this CollisionObject3D source)
        => source.CollisionLayer = 0;

    private static void ClearCollisionMask(this CollisionObject3D source)
        => source.CollisionMask = 0;

    #endregion
}
