using Godot;

namespace F00F;

public static class Const
{
    public static float Gravity { get; private set; } = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    public static void SetGravity(this Node source, float? gravity = null, Vector3? vector = null, bool? point = null, Vector3? linearDamp = null, Vector3? angularDamp = null)
    {
        var area = source.GetViewport().GetWorld3D().Space;
        if (gravity.HasValue) PhysicsServer3D.AreaSetParam(area, PhysicsServer3D.AreaParameter.Gravity, Gravity = gravity.Value);
        if (vector.HasValue) PhysicsServer3D.AreaSetParam(area, PhysicsServer3D.AreaParameter.GravityVector, vector.Value);
        if (point.HasValue) PhysicsServer3D.AreaSetParam(area, PhysicsServer3D.AreaParameter.GravityIsPoint, point.Value);
        if (linearDamp.HasValue) PhysicsServer3D.AreaSetParam(area, PhysicsServer3D.AreaParameter.LinearDamp, linearDamp.Value);
        if (angularDamp.HasValue) PhysicsServer3D.AreaSetParam(area, PhysicsServer3D.AreaParameter.AngularDamp, angularDamp.Value);
    }

    public static void SetZeroGravity(this Node source)
        => source.SetGravity(0, Vector3.Zero, true, Vector3.Zero, Vector3.Zero);

    public const float Deg15 = Mathf.Pi / 12f;
    public const float Deg30 = Mathf.Pi / 6f;
    public const float Deg45 = Mathf.Pi / 4f;
    public const float Deg60 = Mathf.Pi / 3f;
    public const float Deg90 = Mathf.Pi / 2f;
    public const float Deg180 = Mathf.Pi;
    public const float Deg360 = Mathf.Tau;

    public const float Pi = Mathf.Pi;
    public const float Tau = Mathf.Tau;
    public const float HalfPi = Mathf.Pi * .5f;

    public const float Epsilon = .000001f;
    public const float TinyFloat = Epsilon * 100; // .0001f;
    public const float SmallFloat = Epsilon * 1000; // .001f;

    public static readonly Color[] DefaultColors =
    [
        new(1, 0, 0), // Red
        new(0, 1, 0), // Green
        new(0, 0, 1), // Blue
        new(1, 1, 0), // Yellow (red+green)
        new(0, 1, 1), // Cyan (green+blue)
        new(1, 0, 1), // Purple (red+blue)
    ];

    public static class CanvasLayer
    {
        public const int GUI = 2;
        public const int Game = 1;
        public const int Popup = 3;
    }

    public static class TransferChannel
    {
        //public const int Game = 1;
        //public const int World = 2;
        //public const int Player = 3;
        //public const int GameData = 4;
        //public const int WorldData = 5;
        public const int PlayerData = 6;
    }
}
