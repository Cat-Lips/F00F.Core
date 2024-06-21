using Godot;

namespace F00F;

public static class Const
{
    public static readonly float DefaultGravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    public const float Deg15 = Mathf.Pi / 12f;
    public const float Deg30 = Mathf.Pi / 6f;
    public const float Deg45 = Mathf.Pi / 4f;
    public const float Deg60 = Mathf.Pi / 3f;
    public const float Deg90 = Mathf.Pi / 2f;
    public const float Deg180 = Mathf.Pi;
    public const float Deg360 = Mathf.Tau;

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
