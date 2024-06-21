using System;
using Godot;

namespace F00F;

public partial class FreeCam
{
    static FreeCam()
        => MyInput.Init();

    private class MyInput : InputConfig
    {
        public static void Init() { }
        static MyInput() => Init<MyInput>();

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static readonly StringName Forward;
        public static readonly StringName Back;
        public static readonly StringName Left;
        public static readonly StringName Right;
        public static readonly StringName Up;
        public static readonly StringName Down;

        public static readonly StringName SpeedUp;
        public static readonly StringName SlowDown;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        private static class Default
        {
            public static readonly Key[] Forward = [Key.W, Key.Up];
            public static readonly Key[] Back = [Key.S, Key.Down];
            public static readonly Key[] Left = [Key.A, Key.Left];
            public static readonly Key[] Right = [Key.D, Key.Right];
            public static readonly Key[] Up = [Key.R, Key.Pageup];
            public static readonly Key[] Down = [Key.F, Key.Pagedown];

            public static readonly Enum[] SpeedUp = [MouseButton.WheelUp, Key.KpAdd];
            public static readonly Enum[] SlowDown = [MouseButton.WheelDown, Key.KpSubtract];
        }

        private MyInput() { }
    }
}
